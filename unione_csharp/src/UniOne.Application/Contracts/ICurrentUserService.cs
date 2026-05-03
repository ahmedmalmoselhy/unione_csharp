namespace UniOne.Application.Contracts;

public interface ICurrentUserService
{
    long? UserId { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<long> FacultyScopeIds { get; }
    IEnumerable<long> DepartmentScopeIds { get; }
    bool MustChangePassword { get; }
}
