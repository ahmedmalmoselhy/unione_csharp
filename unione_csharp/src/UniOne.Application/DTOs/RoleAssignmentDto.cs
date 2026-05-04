namespace UniOne.Application.DTOs;

public class RoleAssignmentDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public long? FacultyId { get; set; }
    public string? FacultyName { get; set; }
    public long? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}

public class AssignRoleDto
{
    public long UserId { get; set; }
    public string RoleName { get; set; } = null!;
    public long? FacultyId { get; set; }
    public long? DepartmentId { get; set; }
}
