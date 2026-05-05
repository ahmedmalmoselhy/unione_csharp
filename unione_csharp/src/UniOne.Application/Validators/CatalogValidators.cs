using FluentValidation;
using UniOne.Application.DTOs;

namespace UniOne.Application.Validators;

public class CreateAcademicTermDtoValidator : AbstractValidator<CreateAcademicTermDto>
{
    public CreateAcademicTermDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.AcademicYear).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Semester).IsInEnum();
        RuleFor(x => x.StartsAt).NotEmpty();
        RuleFor(x => x.EndsAt).NotEmpty().GreaterThan(x => x.StartsAt);
    }
}

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CreditHours).GreaterThan((byte)0);
        RuleFor(x => x.Level).GreaterThan((byte)0);
    }
}

public class CreateSectionDtoValidator : AbstractValidator<CreateSectionDto>
{
    public CreateSectionDtoValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.ProfessorId).GreaterThan(0);
        RuleFor(x => x.AcademicTermId).GreaterThan(0);
        RuleFor(x => x.Capacity).GreaterThan((ushort)0);
    }
}
