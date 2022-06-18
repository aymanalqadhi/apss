namespace APSS.Domain.Entities;

public record EntityKey<Entity, Tvalue>(ValueTask Id)
{
    /// <inheritence/>
    public override string ToString() => Id?.ToString();
}

public sealed record EntityKey<TEntity>(long Id) : EntityKey<TEntity, long>(Id);
