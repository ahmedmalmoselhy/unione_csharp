using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Mapping;

public class OrganizationMapper
{
    public UniversityDto ToDto(University university)
    {
        return new UniversityDto
        {
            Id = university.Id,
            Name = university.Name,
            NameAr = university.NameAr,
            Address = university.Address,
            LogoPath = university.LogoPath,
            PresidentId = university.PresidentId,
            Phone = university.Phone,
            Email = university.Email,
            Website = university.Website,
            EstablishedAt = university.EstablishedAt
        };
    }

    public void UpdateUniversity(UpdateUniversityDto dto, University university)
    {
        university.Name = dto.Name;
        university.NameAr = dto.NameAr;
        university.Address = dto.Address;
        university.PresidentId = dto.PresidentId;
        university.Phone = dto.Phone;
        university.Email = dto.Email;
        university.Website = dto.Website;
        university.EstablishedAt = dto.EstablishedAt;

        if (dto.RemoveLogo)
        {
            university.LogoPath = null;
        }
    }

    public FacultyDto ToDto(Faculty faculty)
    {
        return new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            NameAr = faculty.NameAr,
            Code = faculty.Code,
            LogoPath = faculty.LogoPath,
            EnrollmentType = faculty.EnrollmentType,
            DeanId = faculty.DeanId,
            IsActive = faculty.IsActive
        };
    }

    public Faculty ToEntity(CreateFacultyDto dto)
    {
        return new Faculty
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            Code = dto.Code,
            EnrollmentType = dto.EnrollmentType,
            DeanId = dto.DeanId,
            IsActive = dto.IsActive
        };
    }

    public void UpdateFaculty(UpdateFacultyDto dto, Faculty faculty)
    {
        faculty.Name = dto.Name;
        faculty.NameAr = dto.NameAr;
        faculty.Code = dto.Code;
        faculty.EnrollmentType = dto.EnrollmentType;
        faculty.DeanId = dto.DeanId;
        faculty.IsActive = dto.IsActive;

        if (dto.RemoveLogo)
        {
            faculty.LogoPath = null;
        }
    }

    public DepartmentDto ToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            FacultyId = department.FacultyId,
            Name = department.Name,
            NameAr = department.NameAr,
            Code = department.Code,
            LogoPath = department.LogoPath,
            Type = department.Type,
            Scope = department.Scope,
            IsPreparatory = department.IsPreparatory,
            HeadId = department.HeadId,
            IsActive = department.IsActive,
            IsMandatory = department.IsMandatory,
            RequiredCreditHours = department.RequiredCreditHours
        };
    }

    public Department ToEntity(CreateDepartmentDto dto)
    {
        return new Department
        {
            FacultyId = dto.FacultyId,
            Name = dto.Name,
            NameAr = dto.NameAr,
            Code = dto.Code,
            Type = dto.Type,
            IsPreparatory = dto.IsPreparatory,
            HeadId = dto.HeadId,
            IsActive = dto.IsActive
        };
    }

    public void UpdateDepartment(UpdateDepartmentDto dto, Department department)
    {
        department.FacultyId = dto.FacultyId;
        department.Name = dto.Name;
        department.NameAr = dto.NameAr;
        department.Code = dto.Code;
        department.IsPreparatory = dto.IsPreparatory;
        department.HeadId = dto.HeadId;
        department.IsActive = dto.IsActive;
        department.RequiredCreditHours = dto.RequiredCreditHours;

        if (dto.RemoveLogo)
        {
            department.LogoPath = null;
        }
    }
}
