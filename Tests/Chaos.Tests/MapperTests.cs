using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Chaos.Core.Definitions;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Serializable;
using Chaos.Objects.World;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Chaos.Tests;

public class MapperTests
{
    public MapperTests(ITestOutputHelper helper)
    {
    }

    [Fact]
    public async Task UserMapperTest()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            #if DEBUG
            .AddJsonFile("appsettings.local.json")
            #endif
            .Build();

        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);
        //services.AddLogging(c => c.AddConsole());

        var provider = services.BuildServiceProvider();
        await Program.InitializeAsync(provider);

        var mapper = provider.GetRequiredService<IMapper>();

        var user = new User(
            "testing",
            Gender.Male,
            10,
            DisplayColor.Lilac);

        var seria = mapper.Map<SerializableUser>(user);

        seria.MapInstanceId = "Testing";

        var populatedUser = mapper.Map(seria, user);
        populatedUser.Should().NotBeNull();
        populatedUser.MapInstance.InstanceId.Should().Be(seria.MapInstanceId);
    }
}