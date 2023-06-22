using Chaos.Common.Identity;

namespace Chaos.Models.World.Abstractions;

/// <summary>
///     Represents an object that exists within the world.
/// </summary>
public abstract class WorldEntity : IEquatable<WorldEntity>
{
    public DateTime Creation { get; init; } = DateTime.UtcNow;
    public uint Id { get; init; } = SequentialIdGenerator<uint>.Shared.NextId;
    public static IEqualityComparer<WorldEntity> IdComparer { get; } = new IdEqualityComparer();

    public bool Equals(WorldEntity? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (Id == other.Id)
               && Creation.Equals(other.Creation);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return Equals((WorldEntity)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    private sealed class IdEqualityComparer : IEqualityComparer<WorldEntity>
    {
        public bool Equals(WorldEntity? x, WorldEntity? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null))
                return false;

            if (ReferenceEquals(y, null))
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(WorldEntity obj) => (int)obj.Id;
    }
}