namespace UniOne.Application.Contracts;

public interface ICurrentUserService
{
    long? UserId { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    bool MustChangePassword { get; }
}
