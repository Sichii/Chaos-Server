#region
using System.Text;
using Chaos.Extensions.DependencyInjection;
using Chaos.Packets.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Packets.Tests;

public sealed class PacketDiExtensionsTests
{
    [Test]
    public void AddPacketSerializer_Registers_Singleton()
    {
        // Ensure code pages are available if any converter uses 949
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var services = new ServiceCollection();
        services.AddPacketSerializer();
        var provider = services.BuildServiceProvider();

        var s1 = provider.GetRequiredService<IPacketSerializer>();
        var s2 = provider.GetRequiredService<IPacketSerializer>();

        s1.Should()
          .NotBeNull();

        ReferenceEquals(s1, s2)
            .Should()
            .BeTrue();
    }
}