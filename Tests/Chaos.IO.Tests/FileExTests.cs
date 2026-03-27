#region
using Chaos.IO.Exceptions;
using Chaos.IO.FileSystem;
using FluentAssertions;
#endregion

namespace Chaos.IO.Tests;

public sealed class FileExTests
{
    private string CreateTempFilePath()
        => Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid()
                .ToString("N")
            + ".txt");

    [Test]
    public void SafeOpenRead_NonRetryableException_ThrowsAggregateWithSingleInner()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "data");

        // An exception that is neither FileNotFoundException nor RetryableException breaks the loop early
        Action act = () => FileEx.SafeOpenRead<string>(path, _ => throw new InvalidOperationException("non-retryable"));

        act.Should()
           .Throw<AggregateException>()
           .Which
           .InnerExceptions
           .Should()
           .HaveCount(1)
           .And
           .AllBeAssignableTo<InvalidOperationException>();

        File.Delete(path);
    }

    [Test]
    public void SafeOpenRead_PrimaryFilePresent_ReturnsImmediately()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "primary-content");

        var result = FileEx.SafeOpenRead(
            path,
            stream =>
            {
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });

        result.Should()
              .Be("primary-content");

        File.Delete(path);
    }

    [Test]
    public void SafeOpenRead_RetryableException_ShouldTryNextPath()
    {
        var basePath = CreateTempFilePath();
        var bakPath = basePath + ".bak";

        // Create primary and backup files
        File.WriteAllText(basePath, "primary");
        File.WriteAllText(bakPath, "backup");

        var callCount = 0;

        // First call throws RetryableException (should continue to next path), second succeeds
        var result = FileEx.SafeOpenRead(
            basePath,
            stream =>
            {
                callCount++;

                if (callCount == 1)
                    throw new RetryableException("retryable");

                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });

        result.Should()
              .Be("backup");

        callCount.Should()
                 .Be(2); // primary (throw) + backup (success). Temp gets FileNotFound before reaching func.

        File.Delete(basePath);
        File.Delete(bakPath);
    }

    [Test]
    public void SafeOpenRead_WhenAllMissing_ShouldThrowAggregateWithInnerFileNotFound()
    {
        var basePath = CreateTempFilePath();
        var tempPath = basePath + ".temp";
        var bakPath = basePath + ".bak";

        if (File.Exists(basePath))
            File.Delete(basePath);

        if (File.Exists(tempPath))
            File.Delete(tempPath);

        if (File.Exists(bakPath))
            File.Delete(bakPath);

        Action act = () => FileEx.SafeOpenRead(basePath, _ => "");

        act.Should()
           .Throw<AggregateException>()
           .Which
           .InnerExceptions
           .Should()
           .NotBeEmpty();
    }

    [Test]
    public void SafeOpenRead_WhenPrimaryMissing_ShouldTryTempThenBak()
    {
        var basePath = CreateTempFilePath();
        var tempPath = basePath + ".temp";
        var bakPath = basePath + ".bak";

        // Ensure clean state
        if (File.Exists(basePath))
            File.Delete(basePath);

        if (File.Exists(tempPath))
            File.Delete(tempPath);

        if (File.Exists(bakPath))
            File.Delete(bakPath);

        // Only create .bak so code reaches the third branch
        File.WriteAllText(bakPath, "backup");

        var read = FileEx.SafeOpenRead(
            basePath,
            stream =>
            {
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });

        read.Should()
            .Be("backup");

        // Cleanup
        File.Delete(bakPath);
    }

    [Test]
    public async Task SafeOpenReadAsync_AllMissing_ShouldThrowAggregateException()
    {
        var basePath = CreateTempFilePath();

        Func<Task> act = () => FileEx.SafeOpenReadAsync<string>(basePath, _ => Task.FromResult(""));

        await act.Should()
                 .ThrowAsync<AggregateException>();
    }

    [Test]
    public async Task SafeOpenReadAsync_NonRetryableException_ShouldBreakEarly()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "data");

        Func<Task> act = () => FileEx.SafeOpenReadAsync<string>(path, _ => throw new InvalidOperationException("non-retryable"));

        await act.Should()
                 .ThrowAsync<AggregateException>()
                 .Where(e => (e.InnerExceptions.Count == 1) && e.InnerExceptions[0] is InvalidOperationException);

        File.Delete(path);
    }

    [Test]
    public async Task SafeOpenReadAsync_PrimaryFilePresent_ReturnsImmediately()
    {
        var path = CreateTempFilePath();
        await File.WriteAllTextAsync(path, "async-primary");

        var result = await FileEx.SafeOpenReadAsync(
            path,
            async stream =>
            {
                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            });

        result.Should()
              .Be("async-primary");

        File.Delete(path);
    }

    [Test]
    public async Task SafeOpenReadAsync_RetryableException_ShouldTryNextPath()
    {
        var basePath = CreateTempFilePath();
        var bakPath = basePath + ".bak";

        File.WriteAllText(basePath, "primary");
        File.WriteAllText(bakPath, "backup");

        var callCount = 0;

        var result = await FileEx.SafeOpenReadAsync(
            basePath,
            async stream =>
            {
                callCount++;

                if (callCount == 1)
                    throw new RetryableException("retryable");

                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            });

        result.Should()
              .Be("backup");

        File.Delete(basePath);
        File.Delete(bakPath);
    }

    [Test]
    public async Task SafeOpenReadAsync_WhenTempExists_ShouldReadTemp()
    {
        var basePath = CreateTempFilePath();
        var tempPath = basePath + ".temp";

        if (File.Exists(basePath))
            File.Delete(basePath);

        if (File.Exists(tempPath))
            File.Delete(tempPath);

        // Create only temp to force second-branch success
        await File.WriteAllTextAsync(tempPath, "tempcontent");

        var read = await FileEx.SafeOpenReadAsync(
            basePath,
            async stream =>
            {
                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            });

        read.Should()
            .Be("tempcontent");

        // Cleanup
        File.Delete(tempPath);
    }

    [Test]
    public void SafeWriteAllText_WhenExistingFile_ShouldUseReplaceWithBak()
    {
        var path = CreateTempFilePath();
        File.WriteAllText(path, "old");

        FileEx.SafeWriteAllText(path, "new");

        File.ReadAllText(path)
            .Should()
            .Be("new");

        File.Exists(path + ".bak")
            .Should()
            .BeTrue();

        File.Delete(path);

        if (File.Exists(path + ".bak"))
            File.Delete(path + ".bak");
    }

    [Test]
    public void SafeWriteAllText_WhenNewFile_ShouldWriteAndBeReadable()
    {
        var path = CreateTempFilePath();

        if (File.Exists(path))
            File.Delete(path);

        FileEx.SafeWriteAllText(path, "hello");

        File.ReadAllText(path)
            .Should()
            .Be("hello");

        File.Delete(path);
    }

    [Test]
    public async Task SafeWriteAllTextAsync_WhenExistingFile_ShouldUseReplaceWithBak()
    {
        var path = CreateTempFilePath();
        await File.WriteAllTextAsync(path, "old");

        await FileEx.SafeWriteAllTextAsync(path, "new");

        File.ReadAllText(path)
            .Should()
            .Be("new");

        File.Exists(path + ".bak")
            .Should()
            .BeTrue();

        // Cleanup
        File.Delete(path);

        if (File.Exists(path + ".bak"))
            File.Delete(path + ".bak");
    }

    [Test]
    public async Task SafeWriteAllTextAsync_WhenNewFile_ShouldWriteAndBeReadable()
    {
        var path = CreateTempFilePath();

        if (File.Exists(path))
            File.Delete(path);

        await FileEx.SafeWriteAllTextAsync(path, "hello-async");

        File.ReadAllText(path)
            .Should()
            .Be("hello-async");

        File.Delete(path);
    }
}