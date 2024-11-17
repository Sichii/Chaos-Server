#region
using System.Collections.Immutable;
using System.Text.RegularExpressions;
#endregion

namespace Chaos.Definitions;

public static partial class RegexCache
{
    public static readonly ICollection<Regex> DEPOSIT_PATTERNS = ImmutableList.Create(
        DepositRegex1,
        DepositRegex2,
        DepositRegex3,
        DepositRegex4,
        DepositRegex5,
        DepositRegex6);

    public static readonly ICollection<Regex> WITHDRAW_PATTERNS = ImmutableList.Create(
        WithdrawRegex1,
        WithdrawRegex2,
        WithdrawRegex3,
        WithdrawRegex4,
        WithdrawRegex5,
        WithdrawRegex6);

    public static readonly ICollection<Regex> ITEM_CHECK_PATTERNS = ImmutableList.Create(ItemCheckRegex1, ItemCheckRegex2);

    public static readonly ICollection<Regex> SELL_ITEM_PATTERNS = ImmutableList.Create(SellItemRegex1, SellItemRegex2, SellItemRegex3);

    public static readonly ICollection<Regex> BUY_ITEM_PATTERNS = ImmutableList.Create(BuyItemRegex1);

    [GeneratedRegex(@"^I (?:will )?buy (?:(?<amount>\d+) )?(?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex BuyItemRegex1 { get; }

    [GeneratedRegex("^I (?:will )?deposit (?<amount>all) my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepositRegex1 { get; }

    [GeneratedRegex(@"^I (?:will )?deposit (?<amount>\w+) of my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepositRegex2 { get; }

    [GeneratedRegex(@"^I (?:will )?deposit (?:(?<amount>\d+) )?(?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepositRegex3 { get; }

    [GeneratedRegex("^Take (?<amount>all) my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepositRegex4 { get; }

    [GeneratedRegex(@"^Take (?<amount>\d+) of my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepositRegex5 { get; }

    [GeneratedRegex(@"^Take my (?:(?<amount>\d+) )?(?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex DepositRegex6 { get; }

    [GeneratedRegex("^How (?:many|much) (?<thing>.+) do I have", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ItemCheckRegex1 { get; }

    [GeneratedRegex("^How (?:many|much) (?<thing>.+) I got", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ItemCheckRegex2 { get; }

    [GeneratedRegex("^I (?:will )?sell (?<amount>all) my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SellItemRegex1 { get; }

    [GeneratedRegex(@"^I (?:will )?sell (?<amount>\d+) of my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SellItemRegex2 { get; }

    [GeneratedRegex(@"^I (?:will )?sell (?:(?<amount>\d+) )?(?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SellItemRegex3 { get; }

    [GeneratedRegex("^I (?:will )?withdraw (?<amount>all) my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WithdrawRegex1 { get; }

    [GeneratedRegex(@"^I (?:will )?withdraw (?<amount>\d+) of my (?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WithdrawRegex2 { get; }

    [GeneratedRegex(@"^I (?:will )?withdraw (?:(?<amount>\d+) )?(?<thing>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WithdrawRegex3 { get; }

    [GeneratedRegex("^Give (?:me )?(?<amount>all) my (?<thing>.+) back", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WithdrawRegex4 { get; }

    [GeneratedRegex(@"^Give (?:me )?(?<amount>\d+) of my (?<thing>.+) back", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WithdrawRegex5 { get; }

    [GeneratedRegex(@"^Give (?:me )?my (?:(?<amount>\d+) )?(?<thing>.+) back", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex WithdrawRegex6 { get; }
}