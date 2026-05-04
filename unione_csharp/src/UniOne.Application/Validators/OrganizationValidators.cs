using FluentValidation;
using UniOne.Application.DTOs;

namespace UniOne.Application.Validators;

public class UpdateUniversityDtoValidator : AbstractValidator<UpdateUniversityDto>
{
    public UpdateUniversityDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class CreateFacultyDtoValidator : AbstractValidator<CreateFacultyDto>
{
    public CreateFacultyDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.EnrollmentType).IsInEnum();
    }
}

public class UpdateFacultyDtoValidator : AbstractValidator<UpdateFacultyDto>
{
    public UpdateFacultyDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.EnrollmentType).IsInEnum();
    }
}

public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Type).IsInEnum();
    }
}

public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
    }
}
