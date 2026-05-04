using FluentValidation;
using UniOne.Application.DTOs;

namespace UniOne.Application.Validators;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NationalId).NotEmpty().MinimumLength(5).MaximumLength(30);
        RuleFor(x => x.StudentNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FacultyId).GreaterThan(0);
        RuleFor(x => x.AcademicYear).InclusiveBetween((byte)1, (byte)7);
        RuleFor(x => x.Semester).IsInEnum();
        RuleFor(x => x.EnrolledAt).NotEmpty();
    }
}

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.AcademicYear).InclusiveBetween((byte)1, (byte)7);
        RuleFor(x => x.Semester).IsInEnum();
        RuleFor(x => x.EnrollmentStatus).IsInEnum();
    }
}

public class CreateProfessorDtoValidator : AbstractValidator<CreateProfessorDto>
{
    public CreateProfessorDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NationalId).NotEmpty().MinimumLength(5).MaximumLength(30);
        RuleFor(x => x.StaffNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.Specialization).NotEmpty().MaximumLength(255);
        RuleFor(x => x.AcademicRank).IsInEnum();
        RuleFor(x => x.HiredAt).NotEmpty();
    }
}

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NationalId).NotEmpty().MinimumLength(5).MaximumLength(30);
        RuleFor(x => x.StaffNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(255);
        RuleFor(x => x.EmploymentType).IsInEnum();
        RuleFor(x => x.HiredAt).NotEmpty();
    }
}
