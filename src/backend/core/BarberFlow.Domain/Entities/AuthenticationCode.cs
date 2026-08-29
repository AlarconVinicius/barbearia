using BarberFlow.Domain.Common;
using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class AuthenticationCode : Entity
{
    public const int CodeLength = 6;
    public const int MaximumAttempts = 3;
    public static readonly TimeSpan MaximumLifetime = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(3);

    private AuthenticationCode()
    {
    }

    public AuthenticationCode(
        Guid userId,
        AuthenticationCodePurpose purpose,
        string codeHash)
        : base()
    {
        UserId = DomainGuard.Required(userId, nameof(userId));
        Purpose = purpose;
        CodeHash = DomainGuard.Required(codeHash, nameof(codeHash));
        ExpiresAtUtc = CreatedAtUtc.Add(MaximumLifetime);
    }

    public Guid UserId { get; private set; }

    public AuthenticationCodePurpose Purpose { get; private set; }

    public string CodeHash { get; private set; } = null!;

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset? UsedAtUtc { get; private set; }

    public DateTimeOffset? InvalidatedAtUtc { get; private set; }

    public DateTimeOffset? LockedUntilUtc { get; private set; }

    public bool CanBeUsedAt(DateTimeOffset nowUtc) =>
        UsedAtUtc is null &&
        InvalidatedAtUtc is null &&
        ExpiresAtUtc > nowUtc &&
        AttemptCount < MaximumAttempts &&
        (LockedUntilUtc is null || LockedUntilUtc <= nowUtc);

    public void RegisterFailedAttempt(DateTimeOffset attemptedAtUtc)
    {
        if (!CanBeUsedAt(attemptedAtUtc))
        {
            throw new InvalidOperationException("The authentication code cannot be used.");
        }

        AttemptCount++;
        MarkAsUpdated(attemptedAtUtc);

        if (AttemptCount == MaximumAttempts)
        {
            InvalidatedAtUtc = attemptedAtUtc;
            LockedUntilUtc = attemptedAtUtc.Add(LockDuration);
        }
    }

    public void MarkAsUsed(DateTimeOffset usedAtUtc)
    {
        if (!CanBeUsedAt(usedAtUtc))
        {
            throw new InvalidOperationException("The authentication code cannot be used.");
        }

        UsedAtUtc = usedAtUtc;
        MarkAsUpdated(usedAtUtc);
    }

    public void Invalidate()
    {
        if (UsedAtUtc is not null)
        {
            throw new InvalidOperationException("A used authentication code cannot be invalidated.");
        }

        InvalidatedAtUtc ??= MarkAsUpdated();
    }
}
