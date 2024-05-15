using Chaos.Common.Identity;
using Chaos.Common.Utilities;

namespace Chaos.Services.Other;

public sealed class BulletinBoardKeyMapper : KeyMapper<ushort>
{
    /// <inheritdoc />
    public BulletinBoardKeyMapper()
        : base(new SequentialIdGenerator<ushort>()) { }
}