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

    private HttpClient CreateAdminClient()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "admin@example.com", ["admin"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static CreateStudentDto NewStudent(string suffix) => new()
    {
        Email = $"student-{suffix}@example.com",
        FirstName = suffix,
        LastName = "Student",
        NationalId = $"NAT-STUDENT-{suffix}",
        StudentNumber = $"STU-{suffix}",
        FacultyId = 1,
        DepartmentId = 1,
        AcademicYear = 1,
        Semester = Semester.First,
        EnrolledAt = new DateOnly(2026, 9, 1)
    };

    private static CreateProfessorDto NewProfessor(string suffix) => new()
    {
        Email = $"professor-{suffix}@example.com",
        FirstName = suffix,
        LastName = "Professor",
        NationalId = $"NAT-PROFESSOR-{suffix}",
        StaffNumber = $"PROF-{suffix}",
        DepartmentId = 1,
        Specialization = "Computer Science",
        AcademicRank = AcademicRank.Lecturer,
        OfficeLocation = "A-100",
        HiredAt = new DateOnly(2020, 9, 1)
    };

    private static CreateEmployeeDto NewEmployee(string suffix) => new()
    {
        Email = $"employee-{suffix}@example.com",
        FirstName = suffix,
        LastName = "Employee",
        NationalId = $"NAT-EMPLOYEE-{suffix}",
        StaffNumber = $"EMP-{suffix}",
        DepartmentId = 1,
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

    private async Task<StudentDto> CreateStudentAsync(HttpClient client, string suffix)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/students", NewStudent(suffix));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<StudentDto>(JsonOptions))!;
    }

    private async Task<ProfessorDto> CreateProfessorAsync(HttpClient client, string suffix)
    {
        var response = await client.PostAsJsonAsync("/api/v1/admin/professors", NewProfessor(suffix));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<ProfessorDto>(JsonOptions))!;
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
