using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class Service : Entity
{
    private Service()
    {
    }

    public Service(
        string name,
        decimal price,
        int durationMinutes,
        string? description = null)
        : base()
    {
        Name = DomainGuard.Required(name, nameof(name));
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = DomainGuard.NonNegative(price, nameof(price));
        DurationMinutes = DomainGuard.Positive(durationMinutes, nameof(durationMinutes));
        IsActive = true;
    }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int DurationMinutes { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(
        string name,
        decimal price,
        int durationMinutes,
        string? description)
    {
        Name = DomainGuard.Required(name, nameof(name));
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = DomainGuard.NonNegative(price, nameof(price));
        DurationMinutes = DomainGuard.Positive(durationMinutes, nameof(durationMinutes));
        MarkAsUpdated();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        MarkAsUpdated();
    }
}
