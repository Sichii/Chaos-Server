namespace Chaos.NLog.Logging.Definitions;

public static class Topics
{
    public static class Actions
    {
        #region Bank Actions
        public static string Deposit => nameof(Deposit);
        public static string Withdraw => nameof(Withdraw);
        #endregion

        #region Shop Actions
        public static string Buy => nameof(Buy);
        public static string Sell => nameof(Sell);
        #endregion

        #region Exchange Actions
        public static string Canceled => nameof(Canceled);
        public static string Accepted => nameof(Accepted);
        #endregion

        #region Guild / Group Actions
        public static string Promote => nameof(Promote);
        public static string Demote => nameof(Demote);
        public static string Kick => nameof(Kick);
        public static string Admit => nameof(Admit);
        public static string Invite => nameof(Invite);
        public static string Leave => nameof(Leave);
        public static string Disband => nameof(Disband);
        #endregion

        #region Creature Actions
        public static string Walk => nameof(Walk);
        public static string Traverse => nameof(Traverse);
        public static string Death => nameof(Death);
        public static string Drop => nameof(Drop);
        public static string Login => nameof(Login);
        public static string Logout => nameof(Logout);
        public static string Pickup => nameof(Pickup);
        public static string Reward => nameof(Reward);
        public static string Message => nameof(Message);
        public static string Command => nameof(Command);
        public static string Learn => nameof(Learn);
        public static string Forget => nameof(Forget);
        #endregion

        #region Storage Actions
        public static string Load => nameof(Load);
        public static string Reload => nameof(Reload);
        public static string Save => nameof(Save);
        public static string Add => nameof(Add);
        public static string Remove => nameof(Remove);
        public static string Create => nameof(Create);
        public static string Read => nameof(Read);
        public static string Update => nameof(Update);
        public static string Delete => nameof(Delete);
        public static string Highlight => nameof(Highlight);
        #endregion

        #region Server Actions
        public static string Processing => nameof(Processing);
        public static string Receive => nameof(Receive);
        public static string Send => nameof(Send);
        public static string Redirect => nameof(Redirect);
        public static string Connect => nameof(Connect);
        public static string Disconnect => nameof(Disconnect);
        public static string Validation => nameof(Validation);
        #endregion
    }

    public static class Entities
    {
        public static string Aisling => nameof(Aisling);
        public static string BulletinBoard => nameof(BulletinBoard);
        public static string Client => nameof(Client);
        public static string Creature => nameof(Creature);
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
        public static string MetaData => nameof(MetaData);
        public static string Monster => nameof(Monster);
        public static string Options => nameof(Options);
        public static string Packet => nameof(Packet);
        public static string Post => nameof(Post);
        public static string Skill => nameof(Skill);
        public static string Spell => nameof(Spell);
        public static string WorldMap => nameof(WorldMap);
    }

    public static class Qualifiers
    {
        public static string Harassment => nameof(Harassment);
        public static string Cheating => nameof(Cheating);
        public static string Forced => nameof(Forced);
        public static string Raw => nameof(Raw);
    }

    public static class Servers
    {
        public static string LobbyServer => nameof(LobbyServer);
        public static string LoginServer => nameof(LoginServer);
        public static string WorldServer => nameof(WorldServer);
    }
}