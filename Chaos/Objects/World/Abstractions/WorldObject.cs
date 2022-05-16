using Chaos.Core.Identity;
using Chaos.Core.Utilities;

namespace Chaos.Objects.World.Abstractions;

/// <summary>
///     Represents an object that exists within the world.
/// </summary>
public abstract class WorldObject : IEquatable<WorldObject>
{
    public DateTime Creation { get; } = DateTime.UtcNow;
    public uint Id { get; } = ClientId.NextId;

    public bool Equals(WorldObject? other)
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

        return Equals((WorldObject)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Id, Creation);
}