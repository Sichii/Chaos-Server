using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ExchangeArgs : ISendArgs
{
    public byte? ExchangeIndex { get; set; }
    public ExchangeResponseType ExchangeResponseType { get; set; }
    public byte? FromSlot { get; set; }
    public int? GoldAmount { get; set; }
    public DisplayColor? ItemColor { get; set; }
    public string? ItemName { get; set; }
    public ushort? ItemSprite { get; set; }
    public uint? OtherUserId { get; set; }
    public string OtherUserName { get; set; } = null!;
    public bool? PersistExchange { get; set; }
    public bool? RightSide { get; set; }
}