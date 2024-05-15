using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Common.Utilities;

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockKeyMapper<T> : KeyMapper<T> where T: INumber<T>
{
    /// <inheritdoc />
    public MockKeyMapper(IIdGenerator<T> idGenerator)
        : base(idGenerator) { }
}