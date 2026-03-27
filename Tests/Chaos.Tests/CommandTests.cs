#region
using System.Net;
using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging;
using Chaos.Messaging.Admin;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Security.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class CommandTests
{
    #region HelpCommand
    [Test]
    public async Task HelpCommand_NoArgs_ShouldDoNothing()
    {
        var command = new HelpCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task HelpCommand_WithHelpText_ShouldSendScrollWindow()
    {
        var command = new HelpCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("someHelpText");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.ScrollWindow, "someHelpText"), Times.Once);
    }
    #endregion

    #region ChannelListCommand
    [Test]
    public async Task ChannelListCommand_NoChannels_ShouldSendNothing()
    {
        var command = new ChannelListCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ChannelListCommand_WithChannels_ShouldSendEachChannelName()
    {
        var command = new ChannelListCommand();
        var aisling = MockAisling.Create();

        aisling.ChannelSettings.Add(new ChannelSettings("!General", true));
        aisling.ChannelSettings.Add(new ChannelSettings("!Trade", true));

        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "!General"), Times.Once);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "!Trade"), Times.Once);
    }
    #endregion

    #region SetLevelCommand
    [Test]
    public async Task SetLevelCommand_NoArgs_ShouldDoNothing()
    {
        var command = new SetLevelCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAttributes(It.IsAny<StatUpdateType>()), Times.Never);
    }

    [Test]
    public async Task SetLevelCommand_InvalidArgs_ShouldDoNothing()
    {
        var command = new SetLevelCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("notanumber");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAttributes(It.IsAny<StatUpdateType>()), Times.Never);
    }

    [Test]
    public async Task SetLevelCommand_ValidLevel_ShouldSetLevelAndSendAttributes()
    {
        var command = new SetLevelCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("50");

        await command.ExecuteAsync(aisling, args);

        aisling.UserStatSheet
               .Level
               .Should()
               .Be(50);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Once);
    }
    #endregion

    #region WhoChannelCommand
    [Test]
    public async Task WhoChannelCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var command = new WhoChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task WhoChannelCommand_NotInChannel_ShouldSendErrorMessage()
    {
        var channelService = MockChannelService.Create();
        var command = new WhoChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("General");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(s => s.Contains("not in channel"))),
            Times.Once);
    }

    [Test]
    public async Task WhoChannelCommand_InChannel_ShouldListSubscribers()
    {
        var channelService = MockChannelService.Create();
        var command = new WhoChannelCommand(channelService);
        var aisling = MockAisling.Create(name: "Player1");

        // Register and join channel
        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        channelService.JoinChannel(aisling, "!General");

        var args = new ArgumentCollection("General");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Player1"), Times.Once);
    }
    #endregion

    #region CreateChannelCommand
    [Test]
    public async Task CreateChannelCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var command = new CreateChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        channelService.ContainsChannel("!anything")
                      .Should()
                      .BeFalse();
    }

    [Test]
    public async Task CreateChannelCommand_ChannelAlreadyExists_ShouldSendErrorMessage()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!TestChannel",
            MessageColor.Default,
            (_, _) => { });

        var command = new CreateChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("TestChannel");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(s => s.Contains("already exists"))),
            Times.Once);
    }

    [Test]
    public async Task CreateChannelCommand_NewChannel_ShouldRegisterAndAddSettings()
    {
        var channelService = MockChannelService.Create();
        var command = new CreateChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NewChannel");

        await command.ExecuteAsync(aisling, args);

        channelService.ContainsChannel("!NewChannel")
                      .Should()
                      .BeTrue();

        aisling.ChannelSettings
               .Should()
               .Contain(s => s.ChannelName == "!NewChannel");
    }
    #endregion

    #region LeaveChannelCommand
    [Test]
    public async Task LeaveChannelCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var command = new LeaveChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        // No exception and no side effects
        aisling.ChannelSettings
               .Should()
               .BeEmpty();
    }

    [Test]
    public async Task LeaveChannelCommand_ValidChannel_ShouldRemoveFromSettingsAndService()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        var aisling = MockAisling.Create();
        channelService.JoinChannel(aisling, "!General");
        aisling.ChannelSettings.Add(new ChannelSettings("!General", true));

        var command = new LeaveChannelCommand(channelService);
        var args = new ArgumentCollection("General");

        await command.ExecuteAsync(aisling, args);

        channelService.IsInChannel(aisling, "!General")
                      .Should()
                      .BeFalse();

        aisling.ChannelSettings
               .Should()
               .NotContain(s => s.ChannelName == "!General");
    }

    [Test]
    public async Task LeaveChannelCommand_GuildChannel_WithGuild_ShouldCallGuildLeaveChannel()
    {
        var channelService = MockChannelService.Create();
        var command = new LeaveChannelCommand(channelService);
        var aisling = MockAisling.Create();

        var guild = MockGuild.Create(channelService: channelService);
        aisling.Guild = guild;

        // Register and join the guild channel so we can verify leave
        channelService.JoinChannel(aisling, guild.ChannelName, true);

        var args = new ArgumentCollection("guild");

        await command.ExecuteAsync(aisling, args);

        // Verify aisling left the guild channel
        channelService.IsInChannel(aisling, guild.ChannelName)
                      .Should()
                      .BeFalse();
    }

    [Test]
    public async Task LeaveChannelCommand_GuildChannel_NoGuild_ShouldLeaveNormally()
    {
        var channelService = MockChannelService.Create();
        var command = new LeaveChannelCommand(channelService);
        var aisling = MockAisling.Create();

        // No guild assigned
        var args = new ArgumentCollection("guild");

        // Should not throw, just fall through to normal leave
        await command.ExecuteAsync(aisling, args);
    }
    #endregion

    #region JoinChannelCommand
    [Test]
    public async Task JoinChannelCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var command = new JoinChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        aisling.ChannelSettings
               .Should()
               .BeEmpty();
    }

    [Test]
    public async Task JoinChannelCommand_ValidChannel_ShouldJoinAndAddSettings()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        var command = new JoinChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("General");

        await command.ExecuteAsync(aisling, args);

        channelService.IsInChannel(aisling, "!General")
                      .Should()
                      .BeTrue();

        aisling.ChannelSettings
               .Should()
               .Contain(s => s.ChannelName == "!General");
    }

    [Test]
    public async Task JoinChannelCommand_GuildChannel_WithGuild_ShouldCallGuildJoinChannel()
    {
        var channelService = MockChannelService.Create();
        var command = new JoinChannelCommand(channelService);
        var aisling = MockAisling.Create();

        var guild = MockGuild.Create(channelService: channelService);
        aisling.Guild = guild;

        var args = new ArgumentCollection("guild");

        await command.ExecuteAsync(aisling, args);

        // Verify aisling joined the guild channel
        channelService.IsInChannel(aisling, guild.ChannelName)
                      .Should()
                      .BeTrue();
    }

    [Test]
    public async Task JoinChannelCommand_GuildChannel_NoGuild_ShouldJoinNormally()
    {
        var channelService = MockChannelService.Create();
        var command = new JoinChannelCommand(channelService);
        var aisling = MockAisling.Create();

        var args = new ArgumentCollection("guild");

        // Should not throw, just fall through to normal join (which may fail since !guild not registered)
        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task JoinChannelCommand_ChannelDoesNotExist_ShouldNotAddSettings()
    {
        var channelService = MockChannelService.Create();
        var command = new JoinChannelCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NonExistent");

        await command.ExecuteAsync(aisling, args);

        aisling.ChannelSettings
               .Should()
               .BeEmpty();
    }
    #endregion

    #region SetChannelColorCommand
    [Test]
    public async Task SetChannelColorCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var command = new SetChannelColorCommand(channelService);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task SetChannelColorCommand_InvalidColor_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var command = new SetChannelColorCommand(channelService);
        var aisling = MockAisling.Create();

        // Only channel name, no color arg
        var args = new ArgumentCollection("General");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        // No error message about channel — bails out at color parse
        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(s => s.Contains("not in channel"))),
            Times.Never);
    }

    [Test]
    public async Task SetChannelColorCommand_NotInChannel_ShouldSendErrorMessage()
    {
        var channelService = MockChannelService.Create();
        var command = new SetChannelColorCommand(channelService);
        var aisling = MockAisling.Create();

        // Channel name + valid color enum name
        var args = new ArgumentCollection("General Default");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(s => s.Contains("not in channel"))),
            Times.Once);
    }

    [Test]
    public async Task SetChannelColorCommand_InChannelButNoSettings_ShouldSendErrorMessage()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        var aisling = MockAisling.Create();
        channelService.JoinChannel(aisling, "!General");

        // Joined but no ChannelSettings in aisling's list
        var command = new SetChannelColorCommand(channelService);
        var args = new ArgumentCollection("General Default");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, It.Is<string>(s => s.Contains("not in channel"))),
            Times.Once);
    }

    [Test]
    public async Task SetChannelColorCommand_InChannelWithSettings_ShouldSetColor()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        var aisling = MockAisling.Create();
        channelService.JoinChannel(aisling, "!General");

        var settings = new ChannelSettings("!General", true);
        aisling.ChannelSettings.Add(settings);

        var command = new SetChannelColorCommand(channelService);
        var args = new ArgumentCollection("General Orange");

        await command.ExecuteAsync(aisling, args);

        settings.MessageColor
                .Should()
                .Be(MessageColor.Orange);
    }
    #endregion

    #region KickCommand
    [Test]
    public async Task KickCommand_NoArgs_ShouldDoNothing()
    {
        var groupService = MockGroupService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new KickCommand(groupService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task KickCommand_TargetNotFound_ShouldSendErrorMessage()
    {
        var groupService = MockGroupService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new KickCommand(groupService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NonExistentPlayer");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task KickCommand_TargetFound_AdminMismatch_ShouldSendErrorMessage()
    {
        var groupService = MockGroupService.Create();
        var source = MockAisling.Create(name: "Leader");
        var target = MockAisling.Create(name: "Target");

        // Source is not admin, target is admin — mismatch
        target.IsAdmin = true;

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new KickCommand(groupService, clientRegistryMock.Object);
        var args = new ArgumentCollection("Target");

        await command.ExecuteAsync(source, args);

        var clientMock = Mock.Get(source.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task KickCommand_TargetFound_SameAdminStatus_ShouldCallKick()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var source = MockAisling.Create(name: "Leader");
        var target = MockAisling.Create(name: "Target");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new KickCommand(groupServiceMock.Object, clientRegistryMock.Object);
        var args = new ArgumentCollection("Target");

        await command.ExecuteAsync(source, args);

        groupServiceMock.Verify(g => g.Kick(source, target), Times.Once);
    }

    [Test]
    public async Task KickCommand_TargetNameWithUnderscores_ShouldReplaceWithSpaces()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var source = MockAisling.Create(name: "Leader");
        var target = MockAisling.Create(name: "Some Player");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new KickCommand(groupServiceMock.Object, clientRegistryMock.Object);
        var args = new ArgumentCollection("Some_Player");

        await command.ExecuteAsync(source, args);

        groupServiceMock.Verify(g => g.Kick(source, target), Times.Once);
    }
    #endregion

    #region GroupInviteCommand
    [Test]
    public async Task GroupInviteCommand_NoArgs_ShouldDoNothing()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new GroupInviteCommand(clientRegistryMock.Object, groupServiceMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        groupServiceMock.Verify(g => g.Invite(It.IsAny<Aisling>(), It.IsAny<Aisling>()), Times.Never);
    }

    [Test]
    public async Task GroupInviteCommand_TargetNotFound_ShouldSendErrorMessage()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new GroupInviteCommand(clientRegistryMock.Object, groupServiceMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NonExistent");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task GroupInviteCommand_TargetFound_ShouldCallInvite()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var source = MockAisling.Create(name: "Sender");
        var target = MockAisling.Create(name: "Receiver");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new GroupInviteCommand(clientRegistryMock.Object, groupServiceMock.Object);
        var args = new ArgumentCollection("Receiver");

        await command.ExecuteAsync(source, args);

        groupServiceMock.Verify(g => g.Invite(source, target), Times.Once);
    }
    #endregion

    #region PromoteLeaderCommand
    [Test]
    public async Task PromoteLeaderCommand_NoArgs_ShouldDoNothing()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new PromoteLeaderCommand(groupServiceMock.Object, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        groupServiceMock.Verify(g => g.Promote(It.IsAny<Aisling>(), It.IsAny<Aisling>()), Times.Never);
    }

    [Test]
    public async Task PromoteLeaderCommand_TargetNotFound_ShouldSendErrorMessage()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new PromoteLeaderCommand(groupServiceMock.Object, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NonExistent");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task PromoteLeaderCommand_TargetFound_ShouldCallPromote()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var source = MockAisling.Create(name: "Leader");
        var target = MockAisling.Create(name: "Member");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new PromoteLeaderCommand(groupServiceMock.Object, clientRegistryMock.Object);
        var args = new ArgumentCollection("Member");

        await command.ExecuteAsync(source, args);

        groupServiceMock.Verify(g => g.Promote(source, target), Times.Once);
    }
    #endregion

    #region RequestGroupInviteCommand
    [Test]
    public async Task RequestGroupInviteCommand_NoArgs_ShouldDoNothing()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new RequestGroupInviteCommand(clientRegistryMock.Object, groupServiceMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        groupServiceMock.Verify(g => g.RequestToJoin(It.IsAny<Aisling>(), It.IsAny<Aisling>()), Times.Never);
    }

    [Test]
    public async Task RequestGroupInviteCommand_TargetNotFound_ShouldSendErrorMessage()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new RequestGroupInviteCommand(clientRegistryMock.Object, groupServiceMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NonExistent");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task RequestGroupInviteCommand_TargetFound_ShouldCallRequestToJoin()
    {
        var groupServiceMock = new Mock<IGroupService>();
        var source = MockAisling.Create(name: "Requester");
        var target = MockAisling.Create(name: "Leader");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new RequestGroupInviteCommand(clientRegistryMock.Object, groupServiceMock.Object);
        var args = new ArgumentCollection("Leader");

        await command.ExecuteAsync(source, args);

        groupServiceMock.Verify(g => g.RequestToJoin(source, target), Times.Once);
    }
    #endregion

    #region GiveGoldCommand
    [Test]
    public async Task GiveGoldCommand_NoArgs_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new GiveGoldCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        aisling.Gold
               .Should()
               .Be(0);
    }

    [Test]
    public async Task GiveGoldCommand_AmountOnly_ShouldGiveGoldToSelf()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new GiveGoldCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("500");

        await command.ExecuteAsync(aisling, args);

        aisling.Gold
               .Should()
               .Be(500);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("gave yourself") && s.Contains("500"))),
            Times.Once);
    }

    [Test]
    public async Task GiveGoldCommand_TargetNameAndAmount_TargetNotFound_ShouldSendError()
    {
        var clients = new List<IChaosWorldClient>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new GiveGoldCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("TargetPlayer 500");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("not online"))), Times.Once);
    }

    [Test]
    public async Task GiveGoldCommand_TargetNameAndAmount_TargetFound_ShouldGiveGold()
    {
        var target = MockAisling.Create(name: "Target");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new GiveGoldCommand(clientRegistryMock.Object);
        var source = MockAisling.Create(name: "Admin");
        var args = new ArgumentCollection("Target 500");

        await command.ExecuteAsync(source, args);

        target.Gold
              .Should()
              .Be(500);
    }
    #endregion

    #region AdminKickCommand
    [Test]
    public async Task AdminKickCommand_NoArgs_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new AdminKickCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task AdminKickCommand_TargetNotFound_ShouldSendError()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new AdminKickCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("Nobody");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task AdminKickCommand_TargetIsAdmin_ShouldSendError()
    {
        var target = MockAisling.Create(name: "AdminTarget");
        target.IsAdmin = true;

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new AdminKickCommand(clientRegistryMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("AdminTarget");

        await command.ExecuteAsync(source, args);

        var clientMock = Mock.Get(source.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task AdminKickCommand_TargetFound_ShouldDisconnect()
    {
        var target = MockAisling.Create(name: "Target");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new AdminKickCommand(clientRegistryMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("Target");

        await command.ExecuteAsync(source, args);

        Mock.Get(target.Client)
            .Verify(c => c.Disconnect(), Times.Once);
    }
    #endregion

    #region MutePlayerCommand
    [Test]
    public async Task MutePlayerCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task MutePlayerCommand_Player_NoPlayerName_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("player");

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task MutePlayerCommand_Player_PlayerNotFound_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("player Nobody");

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task MutePlayerCommand_Player_Found_ShouldSetMuted()
    {
        var channelService = MockChannelService.Create();
        var target = MockAisling.Create(name: "Target");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("player Target");

        await command.ExecuteAsync(aisling, args);

        target.Muted
              .Should()
              .BeTrue();
    }

    [Test]
    public async Task MutePlayerCommand_Channel_NoChannelName_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("channel");

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task MutePlayerCommand_Channel_ShouldMuteChannel()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("channel !General");

        await command.ExecuteAsync(aisling, args);

        // Muted channel should reject sends (no easy way to verify; test just covers the branch)
    }

    [Test]
    public async Task MutePlayerCommand_UnknownType_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new MutePlayerCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("unknown SomeArg");

        await command.ExecuteAsync(aisling, args);
    }
    #endregion

    #region UnmuteCommand
    [Test]
    public async Task UnmuteCommand_NoArgs_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new UnmuteCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task UnmuteCommand_Player_Found_ShouldSetMutedFalse()
    {
        var channelService = MockChannelService.Create();
        var target = MockAisling.Create(name: "Target");
        target.Muted = true;

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new UnmuteCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("player Target");

        await command.ExecuteAsync(aisling, args);

        target.Muted
              .Should()
              .BeFalse();
    }

    [Test]
    public async Task UnmuteCommand_Player_NotFound_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new UnmuteCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("player Nobody");

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task UnmuteCommand_Channel_ShouldUnmuteChannel()
    {
        var channelService = MockChannelService.Create();

        channelService.RegisterChannel(
            null,
            "!General",
            MessageColor.Default,
            (_, _) => { });

        channelService.MuteChannel("!General");

        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new UnmuteCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("channel !General");

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task UnmuteCommand_UnknownType_ShouldDoNothing()
    {
        var channelService = MockChannelService.Create();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new UnmuteCommand(channelService, clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("unknown SomeArg");

        await command.ExecuteAsync(aisling, args);
    }
    #endregion

    #region SetClassCommand
    [Test]
    public async Task SetClassCommand_NoArgs_ShouldDoNothing()
    {
        var command = new SetClassCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAttributes(It.IsAny<StatUpdateType>()), Times.Never);
    }

    [Test]
    public async Task SetClassCommand_ValidClass_ShouldSetClassAndSendAttributes()
    {
        var command = new SetClassCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("Warrior");

        await command.ExecuteAsync(aisling, args);

        aisling.UserStatSheet
               .BaseClass
               .Should()
               .Be(BaseClass.Warrior);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Once);
        clientMock.Verify(c => c.SendUserId(), Times.Once);
    }
    #endregion

    #region FormCommand
    [Test]
    public async Task FormCommand_NoArgs_CurrentSpriteNotZero_ShouldResetToZero()
    {
        var command = new FormCommand();
        var aisling = MockAisling.Create();
        aisling.SetSprite(100);
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        aisling.Sprite
               .Should()
               .Be(0);
    }

    [Test]
    public async Task FormCommand_NoArgs_CurrentSpriteZero_ShouldRemainZero()
    {
        var command = new FormCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        aisling.Sprite
               .Should()
               .Be(0);
    }

    [Test]
    public async Task FormCommand_ValidForm_ShouldSetSprite()
    {
        var command = new FormCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("42");

        await command.ExecuteAsync(aisling, args);

        aisling.Sprite
               .Should()
               .Be(42);
    }
    #endregion

    #region SendMessageCommand
    [Test]
    public async Task SendMessageCommand_NoArgs_ShouldDoNothing()
    {
        var command = new SendMessageCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task SendMessageCommand_TypeOnly_ShouldDoNothing()
    {
        var command = new SendMessageCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("3");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task SendMessageCommand_TypeAndMessage_ShouldSendMessage()
    {
        var command = new SendMessageCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("3 Hello");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage((ServerMessageType)3, "Hello"), Times.Once);
    }
    #endregion

    #region AdminMessageCommand
    [Test]
    public async Task AdminMessageCommand_NoArgs_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var command = new AdminMessageCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task AdminMessageCommand_WithMessage_ShouldBroadcastToAll()
    {
        var target1 = MockAisling.Create(name: "Player1");
        var target2 = MockAisling.Create(name: "Player2");

        var clients = new List<IChaosWorldClient>
        {
            target1.Client,
            target2.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new AdminMessageCommand(clientRegistryMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("TestMessage");

        await command.ExecuteAsync(source, args);

        Mock.Get(target1.Client)
            .Verify(
                c => c.SendServerMessage(
                    ServerMessageType.ActiveMessage,
                    It.Is<string>(s => s.Contains("[Admin]") && s.Contains("TestMessage"))),
                Times.Once);

        Mock.Get(target2.Client)
            .Verify(
                c => c.SendServerMessage(
                    ServerMessageType.ActiveMessage,
                    It.Is<string>(s => s.Contains("[Admin]") && s.Contains("TestMessage"))),
                Times.Once);
    }
    #endregion

    #region SetLanternSizeCommand
    [Test]
    public async Task SetLanternSizeCommand_NoArgs_ShouldSetNone()
    {
        var command = new SetLanternSizeCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        aisling.LanternSize
               .Should()
               .Be(LanternSize.None);
    }

    [Test]
    public async Task SetLanternSizeCommand_ValidSize_ShouldSetLanternSize()
    {
        var command = new SetLanternSizeCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("Large");

        await command.ExecuteAsync(aisling, args);

        aisling.LanternSize
               .Should()
               .Be(LanternSize.Large);
    }
    #endregion

    #region MapFlagCommand
    [Test]
    public async Task MapFlagCommand_NoArgs_ShouldSendError()
    {
        var command = new MapFlagCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Invalid arguments."), Times.Once);
    }

    [Test]
    public async Task MapFlagCommand_AddFlag_ShouldAddFlagToMap()
    {
        var command = new MapFlagCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var args = new ArgumentCollection("add Snow");

        await command.ExecuteAsync(aisling, args);

        map.Flags
           .Should()
           .HaveFlag(MapFlags.Snow);
    }

    [Test]
    public async Task MapFlagCommand_RemoveFlag_ShouldRemoveFlagFromMap()
    {
        var command = new MapFlagCommand();
        var map = MockMapInstance.Create();
        map.Flags = MapFlags.Snow | MapFlags.Rain;
        var aisling = MockAisling.Create(map);
        var args = new ArgumentCollection("remove Snow");

        await command.ExecuteAsync(aisling, args);

        map.Flags
           .Should()
           .NotHaveFlag(MapFlags.Snow);

        map.Flags
           .Should()
           .HaveFlag(MapFlags.Rain);
    }
    #endregion

    #region SetLightLevelCommand
    [Test]
    public async Task SetLightLevelCommand_NoArgs_ShouldDoNothing()
    {
        var command = new SetLightLevelCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task SetLightLevelCommand_ValidLevel_ShouldSetLevelAndDisableAuto()
    {
        var command = new SetLightLevelCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var args = new ArgumentCollection("Dark_B");

        await command.ExecuteAsync(aisling, args);

        map.AutoDayNightCycle
           .Should()
           .BeFalse();

        map.CurrentLightLevel
           .Should()
           .Be(LightLevel.Dark_B);
    }

    [Test]
    public async Task SetLightLevelCommand_Auto_ShouldEnableAutoMode()
    {
        var command = new SetLightLevelCommand();
        var map = MockMapInstance.Create();
        map.AutoDayNightCycle = false;
        var aisling = MockAisling.Create(map);
        var args = new ArgumentCollection("auto");

        await command.ExecuteAsync(aisling, args);

        map.AutoDayNightCycle
           .Should()
           .BeTrue();
    }
    #endregion

    #region AdminIpBanCommand
    [Test]
    public async Task AdminIpBanCommand_NoArgs_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIpBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        accessManagerMock.Verify(a => a.IpBanishAsync(It.IsAny<IPAddress>()), Times.Never);
    }

    [Test]
    public async Task AdminIpBanCommand_TargetNotFound_ShouldSendError()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIpBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("Nobody");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task AdminIpBanCommand_TargetIsAdmin_ShouldSendError()
    {
        var target = MockAisling.Create(name: "Admin");
        target.IsAdmin = true;

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIpBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("Admin");

        await command.ExecuteAsync(source, args);

        var clientMock = Mock.Get(source.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task AdminIpBanCommand_TargetFound_ShouldBanAndDisconnect()
    {
        var target = MockAisling.Create(name: "BadPlayer");
        var targetClientMock = Mock.Get(target.Client);

        targetClientMock.SetupGet(c => c.RemoteIp)
                        .Returns(IPAddress.Parse("1.2.3.4"));

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIpBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("BadPlayer");

        await command.ExecuteAsync(source, args);

        accessManagerMock.Verify(a => a.IpBanishAsync(It.Is<IPAddress>(ip => ip.ToString() == "1.2.3.4")), Times.Once);
        targetClientMock.Verify(c => c.Disconnect(), Times.Once);
    }
    #endregion

    #region AdminIdBanCommand
    [Test]
    public async Task AdminIdBanCommand_NoArgs_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIdBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        accessManagerMock.Verify(a => a.IdBanishAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Never);
    }

    [Test]
    public async Task AdminIdBanCommand_TargetNotFound_ShouldSendError()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(new List<IChaosWorldClient>().GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIdBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("Nobody");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.OrangeBar1, It.Is<string>(s => s.Contains("can not be found"))),
            Times.Once);
    }

    [Test]
    public async Task AdminIdBanCommand_TargetWithDefaultIds_ShouldIpBanInstead()
    {
        var target = MockAisling.Create(name: "BadPlayer");
        var targetClientMock = Mock.Get(target.Client);

        targetClientMock.SetupGet(c => c.LoginId1)
                        .Returns(4278255360U);

        targetClientMock.SetupGet(c => c.LoginId2)
                        .Returns(7695);

        targetClientMock.SetupGet(c => c.RemoteIp)
                        .Returns(IPAddress.Parse("5.6.7.8"));

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIdBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("BadPlayer");

        await command.ExecuteAsync(source, args);

        // Should NOT id ban
        accessManagerMock.Verify(a => a.IdBanishAsync(It.IsAny<uint>(), It.IsAny<uint>()), Times.Never);

        // Should IP ban instead
        accessManagerMock.Verify(a => a.IpBanishAsync(It.Is<IPAddress>(ip => ip.ToString() == "5.6.7.8")), Times.Once);
        targetClientMock.Verify(c => c.Disconnect(), Times.Once);
    }

    [Test]
    public async Task AdminIdBanCommand_TargetWithValidIds_ShouldIdBanAndDisconnect()
    {
        var target = MockAisling.Create(name: "BadPlayer");
        var targetClientMock = Mock.Get(target.Client);

        targetClientMock.SetupGet(c => c.LoginId1)
                        .Returns(12345U);

        targetClientMock.SetupGet(c => c.LoginId2)
                        .Returns(6789);

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var accessManagerMock = new Mock<IAccessManager>();
        var command = new AdminIdBanCommand(clientRegistryMock.Object, accessManagerMock.Object);
        var source = MockAisling.Create();
        var args = new ArgumentCollection("BadPlayer");

        await command.ExecuteAsync(source, args);

        accessManagerMock.Verify(a => a.IdBanishAsync(12345U, 6789U), Times.Once);
        targetClientMock.Verify(c => c.Disconnect(), Times.Once);
    }
    #endregion

    #region DestroyCommand
    [Test]
    public async Task DestroyCommand_NoArgs_ShouldDoNothing()
    {
        var command = new DestroyCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task DestroyCommand_ById_EntityNotFound_ShouldSendError()
    {
        var command = new DestroyCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var args = new ArgumentCollection("99999");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Invalid entity."), Times.Once);
    }

    [Test]
    public async Task DestroyCommand_ById_EntityFound_ShouldRemoveFromMap()
    {
        var command = new DestroyCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var monster = MockMonster.Create(map);
        map.AddEntity(monster, new Point(3, 3));
        var monsterId = monster.Id;

        var args = new ArgumentCollection(monsterId.ToString());

        await command.ExecuteAsync(aisling, args);

        map.TryGetEntity<Monster>(monsterId, out _)
           .Should()
           .BeFalse();
    }

    [Test]
    public async Task DestroyCommand_ByName_NotFound_ShouldSendError()
    {
        var command = new DestroyCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var args = new ArgumentCollection("NonExistentMonster");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Invalid entity."), Times.Once);
    }

    [Test]
    public async Task DestroyCommand_ByName_FoundMonster_ShouldRemoveFromMap()
    {
        var command = new DestroyCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var monster = MockMonster.Create(map, "TestMob");
        map.AddEntity(aisling, new Point(5, 5));
        map.AddEntity(monster, new Point(3, 3));

        var args = new ArgumentCollection("TestMob");

        await command.ExecuteAsync(aisling, args);

        map.GetEntities<Monster>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public async Task DestroyCommand_ByName_FoundAisling_ShouldSendError()
    {
        var command = new DestroyCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map, "TestPlayer");
        var target = MockAisling.Create(map, "Target");
        map.AddEntity(aisling, new Point(5, 5));
        map.AddEntity(target, new Point(3, 3));

        var args = new ArgumentCollection("Target");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Invalid entity."), Times.Once);
    }
    #endregion

    #region DestroyAllCommand
    [Test]
    public async Task DestroyAllCommand_NoArgs_ShouldDoNothing()
    {
        var command = new DestroyAllCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task DestroyAllCommand_Money_ShouldRemoveAllMoney()
    {
        var command = new DestroyAllCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var money = new Money(100, map, new Point(3, 3));
        map.AddEntity(money, new Point(3, 3));

        var args = new ArgumentCollection("money");

        await command.ExecuteAsync(aisling, args);

        map.GetEntities<Money>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public async Task DestroyAllCommand_Gold_ShouldRemoveAllMoney()
    {
        var command = new DestroyAllCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var money = new Money(50, map, new Point(3, 3));
        map.AddEntity(money, new Point(3, 3));

        var args = new ArgumentCollection("gold");

        await command.ExecuteAsync(aisling, args);

        map.GetEntities<Money>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public async Task DestroyAllCommand_GroundItems_ShouldRemoveAllGroundItems()
    {
        var command = new DestroyAllCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var groundItem = MockGroundItem.Create(map);
        map.AddEntity(groundItem, new Point(3, 3));

        var args = new ArgumentCollection("items");

        await command.ExecuteAsync(aisling, args);

        map.GetEntities<GroundItem>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public async Task DestroyAllCommand_Monsters_ShouldRemoveAllMonsters()
    {
        var command = new DestroyAllCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var monster = MockMonster.Create(map);
        map.AddEntity(monster, new Point(3, 3));

        var args = new ArgumentCollection("monsters");

        await command.ExecuteAsync(aisling, args);

        map.GetEntities<Monster>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public async Task DestroyAllCommand_Merchants_ShouldRemoveAllMerchants()
    {
        var command = new DestroyAllCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var merchant = MockMerchant.Create(map);
        map.AddEntity(merchant, new Point(3, 3));

        var args = new ArgumentCollection("merchants");

        await command.ExecuteAsync(aisling, args);

        map.GetEntities<Merchant>()
           .Should()
           .BeEmpty();
    }

    [Test]
    public async Task DestroyAllCommand_UnknownType_ShouldDoNothing()
    {
        var command = new DestroyAllCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var monster = MockMonster.Create(map);
        map.AddEntity(monster, new Point(3, 3));

        var args = new ArgumentCollection("unknown");

        await command.ExecuteAsync(aisling, args);

        // Monster should still be there
        map.GetEntities<Monster>()
           .Should()
           .NotBeEmpty();
    }
    #endregion

    #region CreateCommand
    [Test]
    public async Task CreateCommand_NoArgs_ShouldDoNothing()
    {
        var itemFactoryMock = new Mock<IItemFactory>();
        var command = new CreateCommand(itemFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        itemFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);
    }

    [Test]
    public async Task CreateCommand_WithKeyOnly_ShouldCreateItemWithCountOne()
    {
        var item = MockItem.Create();
        var itemFactoryMock = new Mock<IItemFactory>();

        itemFactoryMock.Setup(f => f.Create("stick", It.IsAny<ICollection<string>>()))
                       .Returns(item);

        var command = new CreateCommand(itemFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("stick");

        await command.ExecuteAsync(aisling, args);

        item.Count
            .Should()
            .Be(1);
    }

    [Test]
    public async Task CreateCommand_WithKeyAndAmount_ShouldCreateItemWithSpecifiedCount()
    {
        var item = MockItem.Create();
        var itemFactoryMock = new Mock<IItemFactory>();

        itemFactoryMock.Setup(f => f.Create("stick", It.IsAny<ICollection<string>>()))
                       .Returns(item);

        var command = new CreateCommand(itemFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("stick 5");

        await command.ExecuteAsync(aisling, args);

        item.Count
            .Should()
            .Be(5);
    }
    #endregion

    #region LearnCommand
    [Test]
    public async Task LearnCommand_NoArgs_ShouldDoNothing()
    {
        var spellFactoryMock = new Mock<ISpellFactory>();
        var skillFactoryMock = new Mock<ISkillFactory>();
        var command = new LearnCommand(spellFactoryMock.Object, skillFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        spellFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);
    }

    [Test]
    public async Task LearnCommand_NoTemplateKey_ShouldDoNothing()
    {
        var spellFactoryMock = new Mock<ISpellFactory>();
        var skillFactoryMock = new Mock<ISkillFactory>();
        var command = new LearnCommand(spellFactoryMock.Object, skillFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("spell");

        await command.ExecuteAsync(aisling, args);

        spellFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);
    }

    [Test]
    public async Task LearnCommand_SpellType_ShouldCreateSpell()
    {
        var spell = MockSpell.Create();
        var spellFactoryMock = new Mock<ISpellFactory>();

        spellFactoryMock.Setup(f => f.Create("testSpell", It.IsAny<ICollection<string>>()))
                        .Returns(spell);

        var skillFactoryMock = new Mock<ISkillFactory>();
        var command = new LearnCommand(spellFactoryMock.Object, skillFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("spell testSpell");

        await command.ExecuteAsync(aisling, args);

        spellFactoryMock.Verify(f => f.Create("testSpell", It.IsAny<ICollection<string>>()), Times.Once);
    }

    [Test]
    public async Task LearnCommand_SkillType_ShouldCreateSkill()
    {
        var skill = MockSkill.Create();
        var skillFactoryMock = new Mock<ISkillFactory>();

        skillFactoryMock.Setup(f => f.Create("testSkill", It.IsAny<ICollection<string>>()))
                        .Returns(skill);

        var spellFactoryMock = new Mock<ISpellFactory>();
        var command = new LearnCommand(spellFactoryMock.Object, skillFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("skill testSkill");

        await command.ExecuteAsync(aisling, args);

        skillFactoryMock.Verify(f => f.Create("testSkill", It.IsAny<ICollection<string>>()), Times.Once);
    }

    [Test]
    public async Task LearnCommand_UnknownType_ShouldDoNothing()
    {
        var spellFactoryMock = new Mock<ISpellFactory>();
        var skillFactoryMock = new Mock<ISkillFactory>();
        var command = new LearnCommand(spellFactoryMock.Object, skillFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("unknown testKey");

        await command.ExecuteAsync(aisling, args);

        spellFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);
        skillFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);
    }
    #endregion

    #region SpawnMerchantCommand
    [Test]
    public async Task SpawnMerchantCommand_NoArgs_ShouldDoNothing()
    {
        var merchantFactoryMock = new Mock<IMerchantFactory>();
        var command = new SpawnMerchantCommand(merchantFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        merchantFactoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>>()),
            Times.Never);
    }

    [Test]
    public async Task SpawnMerchantCommand_WithKeyOnly_ShouldCreateMerchant()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var merchant = MockMerchant.Create(map);

        var merchantFactoryMock = new Mock<IMerchantFactory>();

        merchantFactoryMock.Setup(f => f.Create(
                               It.IsAny<string>(),
                               It.IsAny<MapInstance>(),
                               It.IsAny<IPoint>(),
                               It.IsAny<ICollection<string>>()))
                           .Returns(merchant);

        var command = new SpawnMerchantCommand(merchantFactoryMock.Object);
        var args = new ArgumentCollection("testMerchant");

        await command.ExecuteAsync(aisling, args);

        merchantFactoryMock.Verify(
            f => f.Create(
                "testMerchant",
                map,
                aisling,
                It.IsAny<ICollection<string>>()),
            Times.Once);
    }

    [Test]
    public async Task SpawnMerchantCommand_WithKeyAndDirection_ShouldSetDirection()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var merchant = MockMerchant.Create(map);

        var merchantFactoryMock = new Mock<IMerchantFactory>();

        merchantFactoryMock.Setup(f => f.Create(
                               It.IsAny<string>(),
                               It.IsAny<MapInstance>(),
                               It.IsAny<IPoint>(),
                               It.IsAny<ICollection<string>>()))
                           .Returns(merchant);

        var command = new SpawnMerchantCommand(merchantFactoryMock.Object);
        var args = new ArgumentCollection("testMerchant Right");

        await command.ExecuteAsync(aisling, args);

        merchant.Direction
                .Should()
                .Be(Direction.Right);
    }
    #endregion

    #region SetAislingEnumCommand
    [Test]
    public async Task SetAislingEnumCommand_NoArgs_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();
        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task SetAislingEnumCommand_NoEnumTypeArg_ShouldDoNothing()
    {
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();
        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("TestAisling");

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task SetAislingEnumCommand_ClientNotFound_ShouldSendError()
    {
        var clients = new List<IChaosWorldClient>();
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("NonExistent AoeShape.Front");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Aisling not found"), Times.Once);
    }

    [Test]
    public async Task SetAislingEnumCommand_InvalidFormat_NoDot_ShouldSendError()
    {
        var target = MockAisling.Create(name: "EnumTarget1");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("EnumTarget1 InvalidFormat");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Invalid enum value. (TypeName.EnumValue)"), Times.Once);
    }

    [Test]
    public async Task SetAislingEnumCommand_InvalidFormat_TooManyDots_ShouldSendError()
    {
        var target = MockAisling.Create(name: "EnumTarget2");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("EnumTarget2 Type.Value.Extra");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Invalid enum value. (TypeName.EnumValue)"), Times.Once);
    }

    [Test]
    public async Task SetAislingEnumCommand_TypeNotFound_ShouldSendError()
    {
        var target = MockAisling.Create(name: "EnumTarget3");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("EnumTarget3 NonExistentType.Value");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "No such type exists."), Times.Once);
    }

    [Test]
    public async Task SetAislingEnumCommand_NonFlagEnum_ShouldSetEnum()
    {
        var target = MockAisling.Create(name: "EnumTarget4");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("EnumTarget4 AoeShape.Front");

        await command.ExecuteAsync(aisling, args);

        aisling.Trackers
               .Enums
               .TryGetValue<AoeShape>(out var value)
               .Should()
               .BeTrue();

        value.Should()
             .Be(AoeShape.Front);
    }

    [Test]
    public async Task SetAislingEnumCommand_FlagEnum_ShouldAddFlag()
    {
        var target = MockAisling.Create(name: "EnumTarget5");

        var clients = new List<IChaosWorldClient>
        {
            target.Client
        };
        var clientRegistryMock = new Mock<IClientRegistry<IChaosWorldClient>>();

        clientRegistryMock.Setup(r => r.GetEnumerator())
                          .Returns(() => clients.GetEnumerator());

        var command = new SetAislingEnumCommand(clientRegistryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("EnumTarget5 TargetFilter.FriendlyOnly");

        await command.ExecuteAsync(aisling, args);

        aisling.Trackers
               .Flags
               .HasFlag(TargetFilter.FriendlyOnly)
               .Should()
               .BeTrue();
    }
    #endregion

    #region SpawnMonsterCommand
    [Test]
    public async Task SpawnMonsterCommand_NoArgs_ShouldDoNothing()
    {
        var monsterFactoryMock = new Mock<IMonsterFactory>();
        var simpleCacheMock = new Mock<ISimpleCache>();
        var command = new SpawnMonsterCommand(monsterFactoryMock.Object, simpleCacheMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        monsterFactoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>>()),
            Times.Never);
    }

    [Test]
    public async Task SpawnMonsterCommand_WithKeyOnly_ShouldCreateMonster()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var monster = MockMonster.Create(map);

        var monsterFactoryMock = new Mock<IMonsterFactory>();

        monsterFactoryMock.Setup(f => f.Create(
                              It.IsAny<string>(),
                              It.IsAny<MapInstance>(),
                              It.IsAny<IPoint>(),
                              It.IsAny<ICollection<string>>()))
                          .Returns(monster);

        var simpleCacheMock = new Mock<ISimpleCache>();
        var command = new SpawnMonsterCommand(monsterFactoryMock.Object, simpleCacheMock.Object);
        var args = new ArgumentCollection("common_rat");

        await command.ExecuteAsync(aisling, args);

        monsterFactoryMock.Verify(
            f => f.Create(
                "common_rat",
                map,
                aisling,
                It.IsAny<ICollection<string>>()),
            Times.Once);
    }

    [Test]
    public async Task SpawnMonsterCommand_WithAllArgs_ShouldSetAllProperties()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        var monster = MockMonster.Create(map);

        var monsterFactoryMock = new Mock<IMonsterFactory>();

        monsterFactoryMock.Setup(f => f.Create(
                              It.IsAny<string>(),
                              It.IsAny<MapInstance>(),
                              It.IsAny<IPoint>(),
                              It.IsAny<ICollection<string>>()))
                          .Returns(monster);

        var itemFactoryMock = new Mock<IItemFactory>();
        var simpleCacheMock = new Mock<ISimpleCache>();

        var lootTable = new LootTable(itemFactoryMock.Object, simpleCacheMock.Object, MockScriptProvider.ItemCloner.Object)
        {
            Key = "testLoot",
            LootDrops = new List<LootDrop>(),
            Mode = LootTableMode.ChancePerItem
        };

        simpleCacheMock.Setup(c => c.Get<LootTable>("testLoot"))
                       .Returns(lootTable);

        var command = new SpawnMonsterCommand(monsterFactoryMock.Object, simpleCacheMock.Object);
        var args = new ArgumentCollection("common_rat testLoot 500 100 12");

        await command.ExecuteAsync(aisling, args);

        monster.Experience
               .Should()
               .Be(500);

        monster.Gold
               .Should()
               .Be(100);

        monster.AggroRange
               .Should()
               .Be(12);
    }
    #endregion

    #region StressCommand
    [Test]
    public async Task StressCommand_NoArgs_ShouldDoNothing()
    {
        var itemFactoryMock = new Mock<IItemFactory>();
        var monsterFactoryMock = new Mock<IMonsterFactory>();
        var command = new StressCommand(itemFactoryMock.Object, monsterFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task StressCommand_GroundItems_NoAmount_ShouldDoNothing()
    {
        var itemFactoryMock = new Mock<IItemFactory>();
        var monsterFactoryMock = new Mock<IMonsterFactory>();
        var command = new StressCommand(itemFactoryMock.Object, monsterFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("grounditems");

        await command.ExecuteAsync(aisling, args);

        itemFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);
    }

    [Test]
    public async Task StressCommand_GroundItems_WithAmount_ShouldCreateItems()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var itemFactoryMock = new Mock<IItemFactory>();

        itemFactoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()))
                       .Returns(() => MockItem.Create("stick"));

        var monsterFactoryMock = new Mock<IMonsterFactory>();
        var command = new StressCommand(itemFactoryMock.Object, monsterFactoryMock.Object);
        var args = new ArgumentCollection("grounditems 3");

        await command.ExecuteAsync(aisling, args);

        itemFactoryMock.Verify(f => f.Create("stick", It.IsAny<ICollection<string>>()), Times.Exactly(3));

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "3 stick(s) spawned on the ground"), Times.Once);
    }

    [Test]
    public async Task StressCommand_Monsters_NoAmount_ShouldDoNothing()
    {
        var itemFactoryMock = new Mock<IItemFactory>();
        var monsterFactoryMock = new Mock<IMonsterFactory>();
        var command = new StressCommand(itemFactoryMock.Object, monsterFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("monsters");

        await command.ExecuteAsync(aisling, args);

        monsterFactoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>>()),
            Times.Never);
    }

    [Test]
    public async Task StressCommand_Monsters_WithAmount_ShouldCreateMonsters()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var itemFactoryMock = new Mock<IItemFactory>();
        var monsterFactoryMock = new Mock<IMonsterFactory>();

        monsterFactoryMock.Setup(f => f.Create(
                              It.IsAny<string>(),
                              It.IsAny<MapInstance>(),
                              It.IsAny<IPoint>(),
                              It.IsAny<ICollection<string>>()))
                          .Returns(() => MockMonster.Create(map));

        var command = new StressCommand(itemFactoryMock.Object, monsterFactoryMock.Object);
        var args = new ArgumentCollection("monsters 3");

        await command.ExecuteAsync(aisling, args);

        monsterFactoryMock.Verify(
            f => f.Create(
                "common_rat",
                map,
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>>()),
            Times.Exactly(3));

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "3 rat(s) spawned"), Times.Once);
    }

    [Test]
    public async Task StressCommand_UnknownType_ShouldDoNothing()
    {
        var itemFactoryMock = new Mock<IItemFactory>();
        var monsterFactoryMock = new Mock<IMonsterFactory>();
        var command = new StressCommand(itemFactoryMock.Object, monsterFactoryMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("unknown 5");

        await command.ExecuteAsync(aisling, args);

        itemFactoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<string>>()), Times.Never);

        monsterFactoryMock.Verify(
            f => f.Create(
                It.IsAny<string>(),
                It.IsAny<MapInstance>(),
                It.IsAny<IPoint>(),
                It.IsAny<ICollection<string>>()),
            Times.Never);
    }
    #endregion

    #region RestockCommand
    [Test]
    public async Task RestockCommand_NoArgs_ShouldDoNothing()
    {
        var stockServiceMock = new Mock<IStockService>();
        var command = new RestockCommand(stockServiceMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        stockServiceMock.Verify(s => s.Restock(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task RestockCommand_WithShopName_ShouldRestock()
    {
        var stockServiceMock = new Mock<IStockService>();
        var command = new RestockCommand(stockServiceMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("testShop");

        await command.ExecuteAsync(aisling, args);

        stockServiceMock.Verify(s => s.Restock("testShop", 100), Times.Once);
    }
    #endregion

    #region ForceLoadMapCommand
    [Test]
    public async Task ForceLoadMapCommand_WithMapId_ShouldGetMap()
    {
        var mapCacheMock = new Mock<ISimpleCache<MapInstance>>();
        var command = new ForceLoadMapCommand(mapCacheMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("testMap123");

        await command.ExecuteAsync(aisling, args);

        mapCacheMock.Verify(c => c.Get("testMap123"), Times.Once);
    }

    [Test]
    public async Task ForceLoadMapCommand_NoArgs_ShouldForceLoad()
    {
        var mapCacheMock = new Mock<ISimpleCache<MapInstance>>();
        var command = new ForceLoadMapCommand(mapCacheMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        mapCacheMock.Verify(c => c.ForceLoad(), Times.Once);
    }
    #endregion

    #region DestroyMapCommand
    [Test]
    public async Task DestroyMapCommand_NoArgs_ShouldDoNothing()
    {
        var cacheMock = new Mock<ISimpleCache>();
        var command = new DestroyMapCommand(cacheMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);

        cacheMock.Verify(c => c.Get<MapInstance>(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task DestroyMapCommand_WithMapId_ShouldDestroyMap()
    {
        var map = MockMapInstance.Create();
        var cacheMock = new Mock<ISimpleCache>();

        cacheMock.Setup(c => c.Get<MapInstance>("testMap"))
                 .Returns(map);

        var command = new DestroyMapCommand(cacheMock.Object);
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("testMap");

        await command.ExecuteAsync(aisling, args);

        cacheMock.Verify(c => c.Get<MapInstance>("testMap"), Times.Once);
    }
    #endregion

    #region ShowCommand
    [Test]
    public async Task ShowCommand_NoArgs_ShouldDoNothing()
    {
        var command = new ShowCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection();

        await command.ExecuteAsync(aisling, args);
    }

    [Test]
    public async Task ShowCommand_Ids_ShouldReportEntityIds()
    {
        var command = new ShowCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        map.AddEntity(aisling, new Point(5, 5));

        var monster = MockMonster.Create(map);
        map.AddEntity(monster, new Point(6, 5));

        var groundItem = MockGroundItem.Create(map);
        map.AddEntity(groundItem, new Point(6, 5));

        var args = new ArgumentCollection("ids");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(monster.Id, PublicMessageType.Normal, It.Is<string>(s => s.Contains(monster.Id.ToString()))),
            Times.Once);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(
                groundItem.Id,
                PublicMessageType.Normal,
                It.Is<string>(s => s.Contains(groundItem.Id.ToString()))),
            Times.Once);
    }

    [Test]
    public async Task ShowCommand_Keys_ShouldReportTemplateKeys()
    {
        var command = new ShowCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        map.AddEntity(aisling, new Point(5, 5));

        var monster = MockMonster.Create(map);
        map.AddEntity(monster, new Point(6, 5));

        var merchant = MockMerchant.Create(map);
        map.AddEntity(merchant, new Point(7, 5));

        var groundItem = MockGroundItem.Create(map);
        map.AddEntity(groundItem, new Point(6, 5));

        var args = new ArgumentCollection("keys");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(
                monster.Id,
                PublicMessageType.Normal,
                It.Is<string>(s => s.Contains(monster.Template.TemplateKey))),
            Times.Once);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(
                merchant.Id,
                PublicMessageType.Normal,
                It.Is<string>(s => s.Contains(merchant.Template.TemplateKey))),
            Times.Once);
    }

    [Test]
    public async Task ShowCommand_Ips_ShouldReportAislingIps()
    {
        var command = new ShowCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);
        map.AddEntity(aisling, new Point(5, 5));

        var otherAisling = MockAisling.Create(map, "OtherPlayer");
        map.AddEntity(otherAisling, new Point(6, 5));

        var otherClientMock = Mock.Get(otherAisling.Client);

        otherClientMock.SetupGet(c => c.RemoteIp)
                       .Returns(IPAddress.Parse("192.168.1.1"));

        var args = new ArgumentCollection("ips");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(otherAisling.Id, PublicMessageType.Normal, It.Is<string>(s => s.Contains("192.168.1.1"))),
            Times.Once);
    }

    [Test]
    public async Task ShowCommand_MapInfo_NoShardingNoFlags_ShouldReportBasicInfo()
    {
        var command = new ShowCommand();
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map);

        var args = new ArgumentCollection("mapinfo");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(
                ServerMessageType.ScrollWindow,
                It.Is<string>(s => s.Contains("ShardingOptions: N/A") && s.Contains("InstanceId:"))),
            Times.Once);
    }

    [Test]
    public async Task ShowCommand_MapInfo_WithFlags_ShouldReportFlags()
    {
        var command = new ShowCommand();
        var map = MockMapInstance.Create();
        map.Flags = MapFlags.Snow;
        var aisling = MockAisling.Create(map);

        var args = new ArgumentCollection("mapinfo");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.ScrollWindow, It.Is<string>(s => s.Contains("Snow"))), Times.Once);
    }

    [Test]
    public async Task ShowCommand_MapInfo_WithShardingOptions_ShouldReportDetails()
    {
        var command = new ShowCommand();
        var map = MockMapInstance.Create();

        typeof(MapInstance).GetProperty("ShardingOptions")!.SetValue(
            map,
            new ShardingOptions
            {
                ShardingType = ShardingType.AbsolutePlayerLimit,
                Limit = 5,
                ExitLocation = new Location("exit", 5, 5)
            });

        var aisling = MockAisling.Create(map);

        var args = new ArgumentCollection("mapinfo");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendServerMessage(
                ServerMessageType.ScrollWindow,
                It.Is<string>(s => s.Contains("Limit: 5") && s.Contains("AbsolutePlayerLimit"))),
            Times.Once);
    }

    [Test]
    public async Task ShowCommand_UnknownType_ShouldDoNothing()
    {
        var command = new ShowCommand();
        var aisling = MockAisling.Create();
        var args = new ArgumentCollection("unknown");

        await command.ExecuteAsync(aisling, args);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.IsAny<string>()), Times.Never);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(It.IsAny<uint>(), It.IsAny<PublicMessageType>(), It.IsAny<string>()),
            Times.Never);
    }
    #endregion
}