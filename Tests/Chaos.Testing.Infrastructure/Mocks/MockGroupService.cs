#region
using Chaos.Messaging.Abstractions;
using Chaos.Services.Other;
using Chaos.Services.Other.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockGroupService
{
    public static IGroupService Create(IChannelService? channelService = null, Action<IGroupService>? setup = null)
    {
        channelService ??= MockChannelService.Create();
        var logger = MockLogger.Create<GroupService>();
        var groupFactory = MockGroupFactory.Create(channelService);

        var groupService = new GroupService(logger.Object, groupFactory.Object);

        setup?.Invoke(groupService);

        return groupService;
    }
}