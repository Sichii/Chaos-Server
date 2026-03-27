#region
using System.Net;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Networking.Options;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class ServerTableTests
{
    private static List<ILoginServerInfo> CreateServers(int count)
    {
        var servers = new List<ILoginServerInfo>();

        for (var i = 1; i <= count; i++)
            servers.Add(
                new LoginServerInfo
                {
                    Id = (byte)i,
                    Address = IPAddress.Parse($"10.0.0.{i}"),
                    Port = 4200 + i,
                    Name = $"Server{i}",
                    Description = $"Desc{i}"
                });

        return servers;
    }

    [Test]
    public void ServerTable_ShouldKeyServersById()
    {
        var servers = new List<ILoginServerInfo>
        {
            new LoginServerInfo
            {
                Id = 5,
                Address = IPAddress.Parse("10.0.0.1"),
                Port = 4200,
                Name = "Five",
                Description = "Server five"
            },
            new LoginServerInfo
            {
                Id = 10,
                Address = IPAddress.Parse("10.0.0.2"),
                Port = 4201,
                Name = "Ten",
                Description = "Server ten"
            }
        };

        var table = new ServerTable(servers);

        table.Servers[5]
             .Name
             .Should()
             .Be("Five");

        table.Servers[10]
             .Name
             .Should()
             .Be("Ten");
    }

    [Test]
    public void ServerTable_ShouldPopulateServers_FromMultipleServers()
    {
        var servers = CreateServers(3);

        var table = new ServerTable(servers);

        table.Servers
             .Should()
             .HaveCount(3);

        table.Servers
             .Should()
             .ContainKeys(1, 2, 3);
    }

    [Test]
    public void ServerTable_ShouldPopulateServers_FromSingleServer()
    {
        var servers = CreateServers(1);

        var table = new ServerTable(servers);

        table.Servers
             .Should()
             .HaveCount(1);

        table.Servers[1]
             .Name
             .Should()
             .Be("Server1");
    }

    [Test]
    public void ServerTable_ShouldProduceDifferentCheckSum_ForDifferentInput()
    {
        var servers1 = new List<ILoginServerInfo>
        {
            new LoginServerInfo
            {
                Id = 1,
                Address = IPAddress.Parse("10.0.0.1"),
                Port = 4200,
                Name = "Alpha",
                Description = "First server"
            }
        };

        var servers2 = new List<ILoginServerInfo>
        {
            new LoginServerInfo
            {
                Id = 2,
                Address = IPAddress.Parse("10.0.0.2"),
                Port = 4201,
                Name = "Beta",
                Description = "Second server"
            }
        };

        var table1 = new ServerTable(servers1);
        var table2 = new ServerTable(servers2);

        table1.CheckSum
              .Should()
              .NotBe(table2.CheckSum);
    }

    [Test]
    public void ServerTable_ShouldProduceNonEmptyData()
    {
        var servers = CreateServers(1);

        var table = new ServerTable(servers);

        table.Data
             .Should()
             .NotBeEmpty();
    }

    [Test]
    public void ServerTable_ShouldProduceNonZeroCheckSum()
    {
        var servers = CreateServers(1);

        var table = new ServerTable(servers);

        table.CheckSum
             .Should()
             .NotBe(0u);
    }

    [Test]
    public void ServerTable_ShouldProduceSameCheckSum_ForSameInput()
    {
        var servers1 = CreateServers(2);
        var servers2 = CreateServers(2);

        var table1 = new ServerTable(servers1);
        var table2 = new ServerTable(servers2);

        table1.CheckSum
              .Should()
              .Be(table2.CheckSum);
    }

    [Test]
    public void ServerTable_ShouldProduceSameData_ForSameInput()
    {
        var servers1 = CreateServers(2);
        var servers2 = CreateServers(2);

        var table1 = new ServerTable(servers1);
        var table2 = new ServerTable(servers2);

        table1.Data
              .Should()
              .Equal(table2.Data);
    }
}