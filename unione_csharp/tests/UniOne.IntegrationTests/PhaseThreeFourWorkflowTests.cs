using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniOne.Application.DTOs;
using UniOne.Domain.Enums;
using UniOne.Infrastructure.Persistence;

namespace UniOne.IntegrationTests;

public class PhaseThreeFourWorkflowTests : IClassFixture<TestApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly TestApplicationFactory _factory;

    public PhaseThreeFourWorkflowTests(TestApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Admin_CanCreateAndTransferStudent()
    {
        var client = CreateAdminClient();

        var createResponse = await client.PostAsJsonAsync("/api/v1/admin/students", NewStudent("transfer"));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var student = await createResponse.Content.ReadFromJsonAsync<StudentDto>(JsonOptions);
        student.Should().NotBeNull();
        student!.DepartmentId.Should().Be(1);

        var transferResponse = await client.PostAsJsonAsync(
            $"/api/v1/admin/students/{student.Id}/transfer",
            new TransferStudentDto { ToDepartmentId = 2, Note = "integration transfer" });

        transferResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UniOneDbContext>();
        var reloaded = await dbContext.Students.FindAsync(student.Id);
        var history = await dbContext.StudentDepartmentHistories.SingleAsync(h => h.StudentId == student.Id);

        reloaded!.DepartmentId.Should().Be(2);
        history.FromDepartmentId.Should().Be(1);
        history.ToDepartmentId.Should().Be(2);
        history.Note.Should().Be("integration transfer");
    }

    [Fact]
    public async Task Admin_CanCreateProfessorEmployeeAndCatalogRecords()
    {
        var client = CreateAdminClient();

        var professorResponse = await client.PostAsJsonAsync("/api/v1/admin/professors", NewProfessor("catalog"));
        professorResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var professor = await professorResponse.Content.ReadFromJsonAsync<ProfessorDto>(JsonOptions);
        professor.Should().NotBeNull();

        var employeeResponse = await client.PostAsJsonAsync("/api/v1/admin/employees", NewEmployee("catalog"));
        employeeResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var employee = await employeeResponse.Content.ReadFromJsonAsync<EmployeeDto>(JsonOptions);
        employee.Should().NotBeNull();

        var termResponse = await client.PostAsJsonAsync("/api/v1/admin/catalog/terms", NewTerm("2026-2027"));
        termResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var term = await termResponse.Content.ReadFromJsonAsync<AcademicTermDto>(JsonOptions);
        term.Should().NotBeNull();

        var courseResponse = await client.PostAsJsonAsync("/api/v1/admin/catalog/courses", NewCourse("CS201"));
        courseResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var course = await courseResponse.Content.ReadFromJsonAsync<CourseDto>(JsonOptions);
        course.Should().NotBeNull();

        var sectionResponse = await client.PostAsJsonAsync("/api/v1/admin/sections", new CreateSectionDto
        {
            CourseId = course!.Id,
            ProfessorId = professor!.Id,
            AcademicTermId = term!.Id,
            Capacity = 30,
            Room = "B-201",
            Schedule = """{"days":["Monday"],"startsAt":"09:00","endsAt":"10:30"}"""
        });

        sectionResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var section = await sectionResponse.Content.ReadFromJsonAsync<SectionDto>(JsonOptions);
        section.Should().NotBeNull();
        section!.CourseCode.Should().Be("CS201");
        section.ProfessorFullName.Should().Be("catalog Professor");
        employee!.DepartmentName.Should().Be("Computer Science");
    }

    [Fact]
    public async Task Admin_EnrollmentPreventsDuplicatesAndAddsOverflowToWaitlist()
    {
        var client = CreateAdminClient();
        var studentOne = await CreateStudentAsync(client, "enroll-one");
        var studentTwo = await CreateStudentAsync(client, "enroll-two");
        var professor = await CreateProfessorAsync(client, "enroll");
        var term = await CreateTermAsync(client, "2027-2028");
        var course = await CreateCourseAsync(client, "CS301");
        var section = await CreateSectionAsync(client, course.Id, professor.Id, term.Id, capacity: 1);

        var firstEnrollmentResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/enrollments",
            new CreateEnrollmentDto { StudentId = studentOne.Id, SectionId = section.Id });
        firstEnrollmentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var firstEnrollment = await firstEnrollmentResponse.Content.ReadFromJsonAsync<EnrollmentDto>(JsonOptions);
        firstEnrollment.Should().NotBeNull();

        var duplicateResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/enrollments",
            new CreateEnrollmentDto { StudentId = studentOne.Id, SectionId = section.Id });
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var waitlistResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/enrollments",
            new CreateEnrollmentDto { StudentId = studentTwo.Id, SectionId = section.Id });
        waitlistResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UniOneDbContext>();
            var waitlist = await dbContext.EnrollmentWaitlists.SingleAsync(w => w.StudentId == studentTwo.Id);
            waitlist.Position.Should().Be(1);
        }

        var dropResponse = await client.DeleteAsync($"/api/v1/admin/enrollments/{firstEnrollment!.Id}");
        dropResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var verificationScope = _factory.Services.CreateScope();
        var verificationDbContext = verificationScope.ServiceProvider.GetRequiredService<UniOneDbContext>();
        var promoted = await verificationDbContext.Enrollments.SingleAsync(e => e.StudentId == studentTwo.Id);
        promoted.SectionId.Should().Be(section.Id);
        promoted.Status.Should().Be(EnrollmentRecordStatus.Registered);
        (await verificationDbContext.EnrollmentWaitlists.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task Admin_EnrollmentRejectsMissingPrerequisites()
    {
        var client = CreateAdminClient();
        var student = await CreateStudentAsync(client, "prereq");
        var professor = await CreateProfessorAsync(client, "prereq");
        var term = await CreateTermAsync(client, "2028-2029");
        var prerequisiteCourse = await CreateCourseAsync(client, "CS101");
        var targetCourse = await CreateCourseAsync(client, "CS401");
        var section = await CreateSectionAsync(client, targetCourse.Id, professor.Id, term.Id, capacity: 20);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UniOneDbContext>();
            dbContext.CoursePrerequisites.Add(new()
            {
                CourseId = targetCourse.Id,
                PrerequisiteId = prerequisiteCourse.Id
            });
            await dbContext.SaveChangesAsync();
        }

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/enrollments",
            new CreateEnrollmentDto { StudentId = student.Id, SectionId = section.Id });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var verificationScope = _factory.Services.CreateScope();
        var verificationDbContext = verificationScope.ServiceProvider.GetRequiredService<UniOneDbContext>();
        (await verificationDbContext.Enrollments.AnyAsync(e => e.StudentId == student.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task Admin_EnrollmentRejectsSectionsOutsideRegistrationWindow()
    {
        var client = CreateAdminClient();
        var student = await CreateStudentAsync(client, "closed-window");
        var professor = await CreateProfessorAsync(client, "closed-window");
        var course = await CreateCourseAsync(client, "CS402");
        var term = await CreateTermAsync(
            client,
            "2029-2030",
            registrationStartsAt: DateTime.UtcNow.AddDays(10),
            registrationEndsAt: DateTime.UtcNow.AddDays(20));
        var section = await CreateSectionAsync(client, course.Id, professor.Id, term.Id, capacity: 20);

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/enrollments",
            new CreateEnrollmentDto { StudentId = student.Id, SectionId = section.Id });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Detail.Should().Contain("has not started");
    }

    [Fact]
    public async Task DepartmentAdmin_StudentListIsLimitedToAssignedDepartment()
    {
        var adminClient = CreateAdminClient();
        var scopedClient = CreateDepartmentAdminClient(departmentId: 1);

        var visibleStudent = await CreateStudentAsync(adminClient, "scope-visible", departmentId: 1);
        var hiddenStudent = await CreateStudentAsync(adminClient, "scope-hidden", departmentId: 2);

        var response = await scopedClient.GetAsync("/api/v1/admin/students");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var students = await response.Content.ReadFromJsonAsync<List<StudentDto>>(JsonOptions);
        students.Should().NotBeNull();
        students!.Select(s => s.Id).Should().Contain(visibleStudent.Id);
        students.Select(s => s.Id).Should().NotContain(hiddenStudent.Id);
    }

    [Fact]
    public async Task DepartmentAdmin_CannotReadOrCreateStudentsOutsideAssignedDepartment()
    {
        var adminClient = CreateAdminClient();
        var scopedClient = CreateDepartmentAdminClient(departmentId: 1);
        var hiddenStudent = await CreateStudentAsync(adminClient, "scope-denied", departmentId: 2);

        var getResponse = await scopedClient.GetAsync($"/api/v1/admin/students/{hiddenStudent.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var createResponse = await scopedClient.PostAsJsonAsync(
            "/api/v1/admin/students",
            NewStudent("scope-create-denied", departmentId: 2));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DepartmentAdmin_PeopleListsAreLimitedToAssignedDepartment()
    {
        var adminClient = CreateAdminClient();
        var scopedClient = CreateDepartmentAdminClient(departmentId: 1);

        var visibleProfessor = await CreateProfessorAsync(adminClient, "scope-prof-visible", departmentId: 1);
        var hiddenProfessor = await CreateProfessorAsync(adminClient, "scope-prof-hidden", departmentId: 2);
        var visibleEmployee = await CreateEmployeeAsync(adminClient, "scope-emp-visible", departmentId: 1);
        var hiddenEmployee = await CreateEmployeeAsync(adminClient, "scope-emp-hidden", departmentId: 2);

        var professorResponse = await scopedClient.GetAsync("/api/v1/admin/professors");
        var employeeResponse = await scopedClient.GetAsync("/api/v1/admin/employees");

        professorResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        employeeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var professors = await professorResponse.Content.ReadFromJsonAsync<List<ProfessorDto>>(JsonOptions);
        var employees = await employeeResponse.Content.ReadFromJsonAsync<List<EmployeeDto>>(JsonOptions);

        professors.Should().NotBeNull();
        professors!.Select(p => p.Id).Should().Contain(visibleProfessor.Id);
        professors.Select(p => p.Id).Should().NotContain(hiddenProfessor.Id);

        employees.Should().NotBeNull();
        employees!.Select(e => e.Id).Should().Contain(visibleEmployee.Id);
        employees.Select(e => e.Id).Should().NotContain(hiddenEmployee.Id);
    }

    [Fact]
    public async Task DepartmentAdmin_CannotReadOrCreatePeopleOutsideAssignedDepartment()
    {
        var adminClient = CreateAdminClient();
        var scopedClient = CreateDepartmentAdminClient(departmentId: 1);

        var hiddenProfessor = await CreateProfessorAsync(adminClient, "scope-prof-denied", departmentId: 2);
        var hiddenEmployee = await CreateEmployeeAsync(adminClient, "scope-emp-denied", departmentId: 2);

        var getProfessorResponse = await scopedClient.GetAsync($"/api/v1/admin/professors/{hiddenProfessor.Id}");
        var getEmployeeResponse = await scopedClient.GetAsync($"/api/v1/admin/employees/{hiddenEmployee.Id}");
        var createProfessorResponse = await scopedClient.PostAsJsonAsync(
            "/api/v1/admin/professors",
            NewProfessor("scope-prof-create-denied", departmentId: 2));
        var createEmployeeResponse = await scopedClient.PostAsJsonAsync(
            "/api/v1/admin/employees",
            NewEmployee("scope-emp-create-denied", departmentId: 2));

        getProfessorResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        getEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        createProfessorResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        createEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private HttpClient CreateAdminClient()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "admin@example.com", ["admin"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private HttpClient CreateDepartmentAdminClient(long departmentId)
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(
            TestAuthConstants.UserId,
            "department-admin@example.com",
            ["department_admin"],
            departmentScopes: [departmentId]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static CreateStudentDto NewStudent(string suffix, long facultyId = 1, long? departmentId = 1) => new()
    {
        Email = $"student-{suffix}@example.com",
        FirstName = suffix,
        LastName = "Student",
        NationalId = $"NAT-{suffix}",
        StudentNumber = $"STU-{suffix}",
        FacultyId = facultyId,
        DepartmentId = departmentId,
        AcademicYear = 1,
        Semester = Semester.First,
        EnrolledAt = new DateOnly(2026, 9, 1)
    };

    private static CreateProfessorDto NewProfessor(string suffix, long departmentId = 1) => new()
    {
        Email = $"professor-{suffix}@example.com",
        FirstName = suffix,
        LastName = "Professor",
        NationalId = $"NAT-P-{suffix}",
        StaffNumber = $"PROF-{suffix}",
        DepartmentId = departmentId,
        Specialization = "Computer Science",
        AcademicRank = AcademicRank.Lecturer,
        OfficeLocation = "A-100",
        HiredAt = new DateOnly(2020, 9, 1)
    };

    private static CreateEmployeeDto NewEmployee(string suffix, long departmentId = 1) => new()
    {
        Email = $"employee-{suffix}@example.com",
        FirstName = suffix,
        LastName = "Employee",
        NationalId = $"NAT-E-{suffix}",
        StaffNumber = $"EMP-{suffix}",
        DepartmentId = departmentId,
        JobTitle = "Coordinator",
        EmploymentType = EmploymentType.FullTime,
        Salary = 1500,
        HiredAt = new DateOnly(2021, 1, 1)
    };

    private static CreateAcademicTermDto NewTerm(
        string academicYear,
        DateTime? registrationStartsAt = null,
        DateTime? registrationEndsAt = null) => new()
    {
        Name = $"Fall {academicYear}",
        AcademicYear = academicYear,
        Semester = Semester.First,
        StartsAt = new DateOnly(2026, 9, 1),
        EndsAt = new DateOnly(2027, 1, 15),
        RegistrationStartsAt = registrationStartsAt ?? DateTime.UtcNow.AddDays(-7),
        RegistrationEndsAt = registrationEndsAt ?? DateTime.UtcNow.AddDays(30)
    };

    private static CreateCourseDto NewCourse(string code) => new()
    {
        Code = code,
        Name = $"Course {code}",
        CreditHours = 3,
        LectureHours = 2,
        LabHours = 1,
        Level = 2,
        IsElective = false
    };

    private async Task<StudentDto> CreateStudentAsync(HttpClient client, string suffix, long facultyId = 1, long? departmentId = 1)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/students", NewStudent(suffix, facultyId, departmentId));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<StudentDto>(JsonOptions))!;
    }

    private async Task<ProfessorDto> CreateProfessorAsync(HttpClient client, string suffix, long departmentId = 1)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/professors", NewProfessor(suffix, departmentId));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<ProfessorDto>(JsonOptions))!;
    }

    private async Task<EmployeeDto> CreateEmployeeAsync(HttpClient client, string suffix, long departmentId = 1)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/employees", NewEmployee(suffix, departmentId));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<EmployeeDto>(JsonOptions))!;
    }

    private async Task<AcademicTermDto> CreateTermAsync(
        HttpClient client,
        string academicYear,
        DateTime? registrationStartsAt = null,
        DateTime? registrationEndsAt = null)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/catalog/terms",
            NewTerm(academicYear, registrationStartsAt, registrationEndsAt));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<AcademicTermDto>(JsonOptions))!;
    }

    private async Task<CourseDto> CreateCourseAsync(HttpClient client, string code)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/catalog/courses", NewCourse(code));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<CourseDto>(JsonOptions))!;
    }

    private async Task<SectionDto> CreateSectionAsync(
        HttpClient client,
        long courseId,
        long professorId,
        long termId,
        ushort capacity)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/sections", new CreateSectionDto
        {
            CourseId = courseId,
            ProfessorId = professorId,
            AcademicTermId = termId,
            Capacity = capacity,
            Room = "C-301",
            Schedule = """{"days":["Tuesday"],"startsAt":"11:00","endsAt":"12:30"}"""
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<SectionDto>(JsonOptions))!;
    }
}
