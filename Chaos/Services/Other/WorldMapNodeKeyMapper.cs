using Chaos.Common.Identity;
using Chaos.Common.Utilities;

namespace Chaos.Services.Other;

public class WorldMapNodeKeyMapper : KeyMapper<ushort>
{
    /// <inheritdoc />
    public WorldMapNodeKeyMapper()
        : base(new SequentialIdGenerator<ushort>()) { }
}