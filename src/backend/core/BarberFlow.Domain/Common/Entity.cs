namespace BarberFlow.Domain.Common;

public abstract class Entity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    protected DateTimeOffset MarkAsUpdated(DateTimeOffset? updatedAtUtc = null)
    {
        UpdatedAtUtc = updatedAtUtc ?? DateTimeOffset.UtcNow;
        return UpdatedAtUtc.Value;
    }
}
