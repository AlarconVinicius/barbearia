using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class InboxMessage : Entity
{
    private InboxMessage()
    {
    }

    public InboxMessage(
        Guid messageId,
        string consumer)
        : base()
    {
        MessageId = DomainGuard.Required(messageId, nameof(messageId));
        Consumer = DomainGuard.Required(consumer, nameof(consumer));
        ProcessedAtUtc = CreatedAtUtc;
    }

    public Guid MessageId { get; private set; }

    public string Consumer { get; private set; } = null!;

    public DateTimeOffset ProcessedAtUtc { get; private set; }
}
