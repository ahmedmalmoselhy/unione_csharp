using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IRoleAssignmentService
{
    Task<IEnumerable<RoleAssignmentDto>> GetAssignmentsAsync(long? facultyId = null, long? departmentId = null);
    Task AssignRoleAsync(AssignRoleDto dto);
    Task RevokeRoleAsync(long assignmentId);
}
