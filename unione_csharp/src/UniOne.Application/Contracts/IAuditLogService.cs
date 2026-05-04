namespace UniOne.Application.Contracts;

public interface IAuditLogService
{
    Task RecordAsync(
        string action,
        string auditableType,
        long? auditableId,
        string description,
        object? oldValues = null,
        object? newValues = null);
}
