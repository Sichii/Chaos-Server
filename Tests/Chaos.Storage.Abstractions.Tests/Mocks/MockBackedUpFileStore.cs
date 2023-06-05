using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions.Tests.Mocks;

public sealed class MockBackedUpFileStore : BackedUpFileStoreBase<IBackedUpFileStoreOptions>
{
    public MockBackedUpFileStore(IOptions<IBackedUpFileStoreOptions> options, ILogger logger)
        : base(options, logger) { }
}