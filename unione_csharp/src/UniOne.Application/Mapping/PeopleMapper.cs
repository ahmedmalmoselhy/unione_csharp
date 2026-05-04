using Riok.Mapperly.Abstractions;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Mapping;

[Mapper]
public partial class PeopleMapper
{
    [MapProperty(nameof(Student.User.FirstName), nameof(StudentDto.UserFullName))]
    [MapProperty(nameof(Student.User.Email), nameof(StudentDto.Email))]
    [MapProperty(nameof(Student.Faculty.Name), nameof(StudentDto.FacultyName))]
    [MapProperty(nameof(Student.Department.Name), nameof(StudentDto.DepartmentName))]
    public partial StudentDto ToDto(Student student);

    [MapProperty(nameof(Professor.User.FirstName), nameof(ProfessorDto.UserFullName))]
    [MapProperty(nameof(Professor.User.Email), nameof(ProfessorDto.Email))]
    [MapProperty(nameof(Professor.Department.Name), nameof(ProfessorDto.DepartmentName))]
    public partial ProfessorDto ToDto(Professor professor);

    [MapProperty(nameof(Employee.User.FirstName), nameof(EmployeeDto.UserFullName))]
    [MapProperty(nameof(Employee.User.Email), nameof(EmployeeDto.Email))]
    [MapProperty(nameof(Employee.Department.Name), nameof(EmployeeDto.DepartmentName))]
    public partial EmployeeDto ToDto(Employee employee);

    public partial Student ToEntity(CreateStudentDto dto);
    public partial void UpdateStudent(UpdateStudentDto dto, Student student);

    public partial Professor ToEntity(CreateProfessorDto dto);
    public partial void UpdateProfessor(UpdateProfessorDto dto, Professor professor);

    public partial Employee ToEntity(CreateEmployeeDto dto);
    public partial void UpdateEmployee(UpdateEmployeeDto dto, Employee employee);

    private string MapFullName(User user) => $"{user.FirstName} {user.LastName}";
}
