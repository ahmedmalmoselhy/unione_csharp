using Riok.Mapperly.Abstractions;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Mapping;

[Mapper]
public partial class OrganizationMapper
{
    public partial UniversityDto ToDto(University university);
    public partial void UpdateUniversity(UpdateUniversityDto dto, University university);

    public partial FacultyDto ToDto(Faculty faculty);
    public partial Faculty ToEntity(CreateFacultyDto dto);
    public partial void UpdateFaculty(UpdateFacultyDto dto, Faculty faculty);

    public partial DepartmentDto ToDto(Department department);
    public partial Department ToEntity(CreateDepartmentDto dto);
    public partial void UpdateDepartment(UpdateDepartmentDto dto, Department department);
}
