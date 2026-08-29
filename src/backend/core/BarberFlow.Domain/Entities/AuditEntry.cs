using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class AuditEntry : Entity
{
    private AuditEntry()
    {
    }

    public AuditEntry(
        DateTimeOffset occurredAtUtc,
        string action,
        string entityType,
        string entityId,
        Guid? userId = null,
        string? correlationId = null,
        string? data = null)
        : base()
    {
        OccurredAtUtc = occurredAtUtc;
        UserId = userId is null ? null : DomainGuard.Required(userId.Value, nameof(userId));
        Action = DomainGuard.Required(action, nameof(action));
        EntityType = DomainGuard.Required(entityType, nameof(entityType));
        EntityId = DomainGuard.Required(entityId, nameof(entityId));
        CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? null : correlationId.Trim();
        Data = string.IsNullOrWhiteSpace(data) ? null : data;
    }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public Guid? UserId { get; private set; }

    public string Action { get; private set; } = null!;

    public string EntityType { get; private set; } = null!;

    public string EntityId { get; private set; } = null!;

    public string? CorrelationId { get; private set; }

    public string? Data { get; private set; }
}
