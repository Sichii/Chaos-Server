using Chaos.Core.Identity;

namespace Chaos.Objects.World.Abstractions;

/// <summary>
///     Represents an object that exists within the world.
/// </summary>
public abstract class WorldEntity : IEquatable<WorldEntity>
{
    public DateTime Creation { get; init; } = DateTime.UtcNow;
    public uint Id { get; init; } = ClientId.NextId;

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
}