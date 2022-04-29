using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ExchangeArgs : ISendArgs
{
    public byte? ExchangeIndex { get; set; }
    public ExchangeResponseType ExchangeResponseType { get; set; }
    public uint? FromId { get; set; }
    public string FromName { get; set; } = null!;
    public byte? FromSlot { get; set; }
    public uint? GoldAmount { get; set; }
    public bool? IsFromSelf { get; set; }
    public DisplayColor? ItemColor { get; set; }
    public string? ItemName { get; set; }
    public ushort? ItemSprite { get; set; }
}