#region
using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Common.Utilities;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockKeyMapper<T> : KeyMapper<T> where T: INumber<T>
{
    /// <inheritdoc />
    public MockKeyMapper(IIdGenerator<T> idGenerator)
        : base(idGenerator) { }

    public static Mock<IIdGenerator<T>> CreateIdGenerator(T startValue)
    {
        var current = startValue;
        var mock = new Mock<IIdGenerator<T>>();

        mock.SetupGet(g => g.NextId)
            .Returns(() => current++);

        return mock;
    }
}