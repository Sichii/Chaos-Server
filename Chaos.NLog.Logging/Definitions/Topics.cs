#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Chaos.NLog.Logging.Definitions;

public static class Topics
{
    public static class Actions
    {
        public static string Accepted => nameof(Accepted);
        public static string Add => nameof(Add);
        public static string Admit => nameof(Admit);
        public static string Buy => nameof(Buy);
        public static string Canceled => nameof(Canceled);
        public static string Connect => nameof(Connect);
        public static string Create => nameof(Create);
        public static string Death => nameof(Death);
        public static string Delete => nameof(Delete);
        public static string Demote => nameof(Demote);
        public static string Deposit => nameof(Deposit);
        public static string Disband => nameof(Disband);
        public static string Disconnect => nameof(Disconnect);
        public static string Drop => nameof(Drop);
        public static string Execute => nameof(Execute);
        public static string Forget => nameof(Forget);
        public static string Highlight => nameof(Highlight);
        public static string Invite => nameof(Invite);
        public static string Join => nameof(Join);
        public static string Kick => nameof(Kick);
        public static string Learn => nameof(Learn);
        public static string Leave => nameof(Leave);
        public static string Listening => nameof(Listening);
        public static string Load => nameof(Load);
        public static string Login => nameof(Login);
        public static string Logout => nameof(Logout);
        public static string Penalty => nameof(Penalty);
        public static string Pickup => nameof(Pickup);
        public static string Processing => nameof(Processing);
        public static string Promote => nameof(Promote);
        public static string Read => nameof(Read);
        public static string Receive => nameof(Receive);
        public static string Redirect => nameof(Redirect);
        public static string Reload => nameof(Reload);
        public static string Remove => nameof(Remove);
        public static string Reward => nameof(Reward);
        public static string Save => nameof(Save);
        public static string Sell => nameof(Sell);
        public static string Send => nameof(Send);
        public static string Traverse => nameof(Traverse);
        public static string Update => nameof(Update);
        public static string Validation => nameof(Validation);
        public static string Walk => nameof(Walk);
        public static string Withdraw => nameof(Withdraw);
    }

    public static class Entities
    {
        public static string Aisling => nameof(Aisling);
        public static string Backup => nameof(Backup);
        public static string BulletinBoard => nameof(BulletinBoard);
        public static string Channel => nameof(Channel);
        public static string Client => nameof(Client);
        public static string Command => nameof(Command);
        public static string Creature => nameof(Creature);
        public static string DeltaMonitor => nameof(DeltaMonitor);
        public static string Dialog => nameof(Dialog);
        public static string Effect => nameof(Effect);
        public static string Exchange => nameof(Exchange);
        public static string Experience => nameof(Experience);
        public static string Gold => nameof(Gold);
        public static string Group => nameof(Group);
        public static string Guild => nameof(Guild);
        public static string Item => nameof(Item);
        public static string LootTable => nameof(LootTable);
        public static string Mail => nameof(Mail);
        public static string MailBox => nameof(MailBox);
        public static string MapInstance => nameof(MapInstance);
        public static string MapTemplate => nameof(MapTemplate);
        public static string Merchant => nameof(Merchant);
        public static string Message => nameof(Message);
        public static string MetaData => nameof(MetaData);
        public static string Monster => nameof(Monster);
        public static string Options => nameof(Options);
        public static string Packet => nameof(Packet);
        public static string Post => nameof(Post);
        public static string Quest => nameof(Quest);
        public static string Script => nameof(Script);
        public static string Skill => nameof(Skill);
        public static string Spell => nameof(Spell);
        public static string WorldMap => nameof(WorldMap);
    }

    public static class Qualifiers
    {
        public static string Cheating => nameof(Cheating);
        public static string Forced => nameof(Forced);
        public static string Harassment => nameof(Harassment);
        public static string Raw => nameof(Raw);
    }

    public static class Servers
    {
        public static string LobbyServer => nameof(LobbyServer);
        public static string LoginServer => nameof(LoginServer);
        public static string WorldServer => nameof(WorldServer);
    }
}