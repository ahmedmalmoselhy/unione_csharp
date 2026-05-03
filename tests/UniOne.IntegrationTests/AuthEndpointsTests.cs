using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.DTOs;

namespace UniOne.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory _factory;

    public AuthEndpointsTests(TestApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ReturnsUnauthorizedProblem_WhenCredentialsAreInvalid()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "student@example.com",
            "wrong-password"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Login_ReturnsTokenAndUser_WhenCredentialsAreValid()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "student@example.com",
            "Password1"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.User.Email.Should().Be("student@example.com");
        body.User.Roles.Should().Contain("student");
    }

    [Fact]
    public async Task Me_ReturnsUnauthorizedProblem_WhenTokenIsMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Unauthorized");
    }

    [Fact]
    public async Task Me_ReturnsCurrentUser_WhenTokenIsActive()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "student@example.com", ["student"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(TestAuthConstants.UserId);
    }

    [Fact]
    public async Task Logout_RevokesCurrentToken()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "student@example.com", ["student"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var logoutResponse = await client.PostAsync("/api/v1/auth/logout", content: null);
        var meResponse = await client.GetAsync("/api/v1/auth/me");

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        meResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_ReturnsOk_WhenTokenIsActive()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "student@example.com", ["student"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/v1/auth/change-password", new ChangePasswordRequest(
            "Password1",
            "Password2"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Tokens_ReturnsCurrentToken()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "student@example.com", ["student"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/auth/tokens");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TokenListResponse>();
        body.Should().NotBeNull();
        body!.Tokens.Should().ContainSingle(t => t.IsCurrent);
    }

    [Fact]
    public async Task ForcedPasswordChange_BlocksProtectedEndpoints()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(
            TestAuthConstants.UserId,
            "student@example.com",
            ["student"],
            mustChangePassword: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var blockedResponse = await client.GetAsync("/api/v1/auth/tokens");
        var allowedResponse = await client.GetAsync("/api/v1/auth/me");

        blockedResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        allowedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RevokeMissingToken_ReturnsNotFoundProblem()
    {
        var client = _factory.CreateClient();
        var token = _factory.TokenStore.IssueToken(TestAuthConstants.UserId, "student@example.com", ["student"]);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync("/api/v1/auth/tokens/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not Found");
    }

    [Fact]
    public async Task ForgotPassword_ReturnsOkWithoutEmailEnumeration()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/forgot-password", new ForgotPasswordRequest(
            "missing@example.com"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_ReturnsOk_WhenRequestIsValid()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", new ResetPasswordRequest(
            "student@example.com",
            "reset-token",
            "Password2"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed record TokenListResponse(IEnumerable<UserTokenDto> Tokens);
}
