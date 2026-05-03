using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.IntegrationTests;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    public TestTokenStore TokenStore { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unione_tests;Username=unione;Password=unione",
                ["Jwt:Issuer"] = TestAuthConstants.Issuer,
                ["Jwt:Audience"] = TestAuthConstants.Audience,
                ["Jwt:Key"] = TestAuthConstants.SigningKey,
                ["Jwt:ExpireDays"] = "7"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IIdentityService>();
            services.RemoveAll<IPersonalAccessTokenRepository>();

            services.AddSingleton(TokenStore);
            services.AddSingleton<IPersonalAccessTokenRepository>(TokenStore);
            services.AddScoped<IIdentityService, FakeIdentityService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthenticationHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.SchemeName,
                _ => { });
        });
    }
}

public static class TestAuthConstants
{
    public const long UserId = 100;
    public const string Issuer = "unione-tests";
    public const string Audience = "unione-tests";
    public const string SigningKey = "unione_phase_one_auth_tests_signing_key_1234567890";
}

public class TestTokenStore : IPersonalAccessTokenRepository
{
    private readonly Dictionary<long, List<(long Id, string Token, string Name, DateTime CreatedAt, bool MustChangePassword)>> _tokens = new();
    private readonly HashSet<string> _revokedTokens = [];
    private long _nextId = 1;

    public string IssueToken(
        long userId,
        string email,
        IEnumerable<string> roles,
        bool mustChangePassword = false)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("must_change_password", mustChangePassword.ToString().ToLowerInvariant())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestAuthConstants.SigningKey));
        var token = new JwtSecurityToken(
            TestAuthConstants.Issuer,
            TestAuthConstants.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        StoreToken(userId, "API token", accessToken, mustChangePassword);
        return accessToken;
    }

    public Task StoreTokenAsync(long userId, string name, string accessToken, DateTime expiresAt)
    {
        StoreToken(userId, name, accessToken, mustChangePassword: false);
        return Task.CompletedTask;
    }

    public Task<bool> IsTokenActiveAsync(long userId, string accessToken)
    {
        return Task.FromResult(!_revokedTokens.Contains(accessToken));
    }

    public Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId, string? currentAccessToken)
    {
        var tokens = _tokens.TryGetValue(userId, out var userTokens)
            ? userTokens.Select(token => new UserTokenDto(
                token.Id,
                token.Name,
                token.CreatedAt,
                null,
                token.Token == currentAccessToken))
            : Enumerable.Empty<UserTokenDto>();

        return Task.FromResult(tokens);
    }

    public Task<bool> RevokeTokenAsync(long userId, long tokenId)
    {
        if (!_tokens.TryGetValue(userId, out var tokens))
        {
            return Task.FromResult(false);
        }

        var removed = tokens.RemoveAll(token => token.Id == tokenId) > 0;
        return Task.FromResult(removed);
    }

    public Task<bool> RevokeTokenAsync(long userId, string accessToken)
    {
        if (!_tokens.TryGetValue(userId, out var tokens))
        {
            return Task.FromResult(false);
        }

        var removed = tokens.RemoveAll(token => token.Token == accessToken) > 0;
        if (removed)
        {
            _revokedTokens.Add(accessToken);
        }

        return Task.FromResult(removed);
    }

    public Task RevokeAllTokensAsync(long userId)
    {
        if (_tokens.TryGetValue(userId, out var tokens))
        {
            foreach (var token in tokens)
            {
                _revokedTokens.Add(token.Token);
            }
        }

            _tokens.Remove(userId);
        return Task.CompletedTask;
    }

    public bool MustChangePassword(string accessToken)
    {
        return _tokens.Values
            .SelectMany(tokens => tokens)
            .FirstOrDefault(token => token.Token == accessToken)
            .MustChangePassword;
    }

    private void StoreToken(long userId, string name, string accessToken, bool mustChangePassword)
    {
        if (!_tokens.TryGetValue(userId, out var tokens))
        {
            tokens = [];
            _tokens[userId] = tokens;
        }

        tokens.Add((_nextId++, accessToken, name, DateTime.UtcNow, mustChangePassword));
    }
}

public class FakeIdentityService : IIdentityService
{
    private readonly TestTokenStore _tokenStore;

    public FakeIdentityService(TestTokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        if (request is not { Email: "student@example.com", Password: "Password1" })
        {
            return Task.FromResult<AuthResponse?>(null);
        }

        var user = CreateUser();
        var token = _tokenStore.IssueToken(user.Id, user.Email, user.Roles);
        return Task.FromResult<AuthResponse?>(new AuthResponse(token, user));
    }

    public Task LogoutAsync(long userId, string accessToken)
    {
        return _tokenStore.RevokeTokenAsync(userId, accessToken);
    }

    public Task<UserDto?> GetCurrentUserAsync(long userId)
    {
        return Task.FromResult(userId == TestAuthConstants.UserId ? CreateUser() : null);
    }

    public Task<bool> ForgotPasswordAsync(string email)
    {
        return Task.FromResult(true);
    }

    public Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> ChangePasswordAsync(long userId, ChangePasswordRequest request)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<UserDto?> UpdateProfileAsync(long userId, UpdateProfileRequest request)
    {
        return Task.FromResult<UserDto?>(CreateUser());
    }

    public Task<IEnumerable<UserTokenDto>> GetActiveTokensAsync(long userId, string? currentAccessToken)
    {
        return _tokenStore.GetActiveTokensAsync(userId, currentAccessToken);
    }

    public Task<bool> RevokeTokenAsync(long userId, long tokenId)
    {
        return _tokenStore.RevokeTokenAsync(userId, tokenId);
    }

    public Task RevokeAllTokensAsync(long userId)
    {
        return _tokenStore.RevokeAllTokensAsync(userId);
    }

    private static UserDto CreateUser()
    {
        return new UserDto(
            TestAuthConstants.UserId,
            "student@example.com",
            "Test",
            "Student",
            "NAT-100",
            null,
            ["student"]);
    }
}

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    private readonly TestTokenStore _tokenStore;

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TestTokenStore tokenStore)
        : base(options, logger, encoder)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (!AuthenticationHeaderValue.TryParse(authorization, out var header) ||
            !string.Equals(header.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(header.Parameter))
        {
            return AuthenticateResult.NoResult();
        }

        if (!await _tokenStore.IsTokenActiveAsync(TestAuthConstants.UserId, header.Parameter))
        {
            return AuthenticateResult.Fail("Token has been revoked.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, TestAuthConstants.UserId.ToString()),
            new Claim(ClaimTypes.Role, "student"),
            new Claim("must_change_password", _tokenStore.MustChangePassword(header.Parameter).ToString().ToLowerInvariant())
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName));
    }
}
