using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions.Tests.Mocks;

public sealed class MockDirectoryBackupService : DirectoryBackupService<IDirectoryBackupOptions>
{
    public MockDirectoryBackupService(IOptions<IDirectoryBackupOptions> options, ILogger<MockDirectoryBackupService> logger)
        : base(options, logger) { }
}