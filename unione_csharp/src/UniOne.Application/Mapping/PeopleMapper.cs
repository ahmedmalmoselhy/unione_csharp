using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Mapping;

public class PeopleMapper
{
    public StudentDto ToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            UserId = student.UserId,
            UserFullName = MapFullName(student.User),
            Email = student.User.Email ?? string.Empty,
            StudentNumber = student.StudentNumber,
            FacultyId = student.FacultyId,
            FacultyName = student.Faculty.Name,
            DepartmentId = student.DepartmentId,
            DepartmentName = student.Department?.Name,
            AcademicYear = student.AcademicYear,
            Semester = student.Semester,
            EnrollmentStatus = student.EnrollmentStatus,
            AcademicStanding = student.AcademicStanding,
            Gpa = student.Gpa,
            EnrolledAt = student.EnrolledAt,
            GraduatedAt = student.GraduatedAt
        };
    }

    public ProfessorDto ToDto(Professor professor)
    {
        return new ProfessorDto
        {
            Id = professor.Id,
            UserId = professor.UserId,
            UserFullName = MapFullName(professor.User),
            Email = professor.User.Email ?? string.Empty,
            StaffNumber = professor.StaffNumber,
            DepartmentId = professor.DepartmentId,
            DepartmentName = professor.Department.Name,
            Specialization = professor.Specialization,
            AcademicRank = professor.AcademicRank,
            OfficeLocation = professor.OfficeLocation,
            HiredAt = professor.HiredAt
        };
    }

    public EmployeeDto ToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            UserId = employee.UserId,
            UserFullName = MapFullName(employee.User),
            Email = employee.User.Email ?? string.Empty,
            StaffNumber = employee.StaffNumber,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,
            JobTitle = employee.JobTitle,
            EmploymentType = employee.EmploymentType,
            Salary = employee.Salary,
            HiredAt = employee.HiredAt,
            TerminatedAt = employee.TerminatedAt
        };
    }

    public Student ToEntity(CreateStudentDto dto)
    {
        return new Student
        {
            StudentNumber = dto.StudentNumber,
            FacultyId = dto.FacultyId,
            DepartmentId = dto.DepartmentId,
            AcademicYear = dto.AcademicYear,
            Semester = dto.Semester,
            EnrolledAt = dto.EnrolledAt
        };
    }

    public void UpdateStudent(UpdateStudentDto dto, Student student)
    {
        student.DepartmentId = dto.DepartmentId;
        student.AcademicYear = dto.AcademicYear;
        student.Semester = dto.Semester;
        student.EnrollmentStatus = dto.EnrollmentStatus;
        student.AcademicStanding = dto.AcademicStanding;
        student.Gpa = dto.Gpa;
        student.GraduatedAt = dto.GraduatedAt;
    }

    public Professor ToEntity(CreateProfessorDto dto)
    {
        return new Professor
        {
            StaffNumber = dto.StaffNumber,
            DepartmentId = dto.DepartmentId,
            Specialization = dto.Specialization,
            AcademicRank = dto.AcademicRank,
            OfficeLocation = dto.OfficeLocation,
            HiredAt = dto.HiredAt
        };
    }

    public void UpdateProfessor(UpdateProfessorDto dto, Professor professor)
    {
        professor.DepartmentId = dto.DepartmentId;
        professor.Specialization = dto.Specialization;
        professor.AcademicRank = dto.AcademicRank;
        professor.OfficeLocation = dto.OfficeLocation;
        professor.HiredAt = dto.HiredAt;
    }

    public Employee ToEntity(CreateEmployeeDto dto)
    {
        return new Employee
        {
            StaffNumber = dto.StaffNumber,
            DepartmentId = dto.DepartmentId,
            JobTitle = dto.JobTitle,
            EmploymentType = dto.EmploymentType,
            Salary = dto.Salary,
            HiredAt = dto.HiredAt
        };
    }

    public void UpdateEmployee(UpdateEmployeeDto dto, Employee employee)
    {
        employee.DepartmentId = dto.DepartmentId;
        employee.JobTitle = dto.JobTitle;
        employee.EmploymentType = dto.EmploymentType;
        employee.Salary = dto.Salary;
        employee.HiredAt = dto.HiredAt;
        employee.TerminatedAt = dto.TerminatedAt;
    }

    private static string MapFullName(User user) => $"{user.FirstName} {user.LastName}";
}
