using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage.Abstractions.Tests.Mocks;

public sealed class MockDirectoryBackupService(IOptions<IDirectoryBackupOptions> options, ILogger<MockDirectoryBackupService> logger)
    : DirectoryBackupService<IDirectoryBackupOptions>(options, logger);