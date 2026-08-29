using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class OutboxMessage : Entity
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(
        DateTimeOffset occurredAtUtc,
        string type,
        string payload,
        string? correlationId = null)
        : base()
    {
        OccurredAtUtc = occurredAtUtc;
        Type = DomainGuard.Required(type, nameof(type));
        Payload = DomainGuard.Required(payload, nameof(payload));
        CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? null : correlationId.Trim();
    }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string Type { get; private set; } = null!;

    public string Payload { get; private set; } = null!;

    public string? CorrelationId { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset? LastAttemptAtUtc { get; private set; }

    public string? LastError { get; private set; }

    public void MarkAttempt(string? error)
    {
        AttemptCount++;
        LastAttemptAtUtc = MarkAsUpdated();
        LastError = string.IsNullOrWhiteSpace(error) ? null : error.Trim();
    }

    public void MarkAsProcessed()
    {
        ProcessedAtUtc = MarkAsUpdated();
        LastError = null;
    }
}
