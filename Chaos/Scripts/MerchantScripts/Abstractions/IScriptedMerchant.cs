using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MerchantScripts.Abstractions;

public interface IScriptedMerchant : IScripted
{
    IMerchantScript Script { get; }
}