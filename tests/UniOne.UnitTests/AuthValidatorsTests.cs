using FluentAssertions;
using UniOne.Application.DTOs;
using UniOne.Application.Validators;

namespace UniOne.UnitTests;

public class AuthValidatorsTests
{
    [Fact]
    public void LoginValidator_RejectsInvalidEmailAndMissingPassword()
    {
        var validator = new LoginRequestValidator();

        var result = validator.Validate(new LoginRequest("not-an-email", ""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(LoginRequest.Email));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(LoginRequest.Password));
    }

    [Fact]
    public void ResetPasswordValidator_RequiresStrongPassword()
    {
        var validator = new ResetPasswordRequestValidator();

        var result = validator.Validate(new ResetPasswordRequest("student@example.com", "token", "weak"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(ResetPasswordRequest.Password));
    }

    [Fact]
    public void UpdateProfileValidator_RejectsFutureDateOfBirth()
    {
        var validator = new UpdateProfileRequestValidator();

        var result = validator.Validate(new UpdateProfileRequest(
            Phone: null,
            DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            AvatarPath: null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(UpdateProfileRequest.DateOfBirth));
    }
}
