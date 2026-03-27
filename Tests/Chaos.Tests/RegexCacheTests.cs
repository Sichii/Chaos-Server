#region
using Chaos.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

/// <summary>
///     Tests for RegexCache compiled regex patterns used in dialog parsing
/// </summary>
public sealed class RegexCacheTests
{
    #region DEPOSIT_PATTERNS
    [Test]
    public void DepositPatterns_ShouldMatchDepositAllMy()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("I will deposit all my Swords"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("all");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Swords");
    }

    [Test]
    public void DepositPatterns_ShouldMatchDepositAmountOfMy()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("I will deposit 5 of my Apples"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("5");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Apples");
    }

    [Test]
    public void DepositPatterns_ShouldMatchDepositAmount()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("I deposit 10 Gems"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("10");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Gems");
    }

    [Test]
    public void DepositPatterns_ShouldMatchTakeAllMy()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("Take all my Potions"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("all");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Potions");
    }

    [Test]
    public void DepositPatterns_ShouldMatchTakeAmountOfMy()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("Take 3 of my Shields"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("3");
    }

    [Test]
    public void DepositPatterns_ShouldMatchTakeMyAmount()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("Take my 5 Rings"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("5");
    }
    #endregion

    #region WITHDRAW_PATTERNS
    [Test]
    public void WithdrawPatterns_ShouldMatchWithdrawAllMy()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("I will withdraw all my Swords"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("all");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Swords");
    }

    [Test]
    public void WithdrawPatterns_ShouldMatchWithdrawAmountOfMy()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("I withdraw 10 of my Gems"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("10");
    }

    [Test]
    public void WithdrawPatterns_ShouldMatchWithdrawAmount()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("I withdraw 5 Potions"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("5");
    }

    [Test]
    public void WithdrawPatterns_ShouldMatchGiveMeAllMyBack()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("Give me all my Rings back"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("all");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Rings");
    }

    [Test]
    public void WithdrawPatterns_ShouldMatchGiveMeAmountOfMyBack()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("Give me 3 of my Swords back"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("3");
    }

    [Test]
    public void WithdrawPatterns_ShouldMatchGiveMyAmountBack()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("Give my 7 Shields back"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("7");
    }
    #endregion

    #region ITEM_CHECK_PATTERNS
    [Test]
    public void ItemCheckPatterns_ShouldMatchHowManyDoIHave()
    {
        var match = RegexCache.ITEM_CHECK_PATTERNS
                              .Select(r => r.Match("How many Swords do I have"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["thing"]
              .Value
              .Should()
              .Be("Swords");
    }

    [Test]
    public void ItemCheckPatterns_ShouldMatchHowMuchDoIHave()
    {
        var match = RegexCache.ITEM_CHECK_PATTERNS
                              .Select(r => r.Match("How much Gold do I have"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["thing"]
              .Value
              .Should()
              .Be("Gold");
    }

    [Test]
    public void ItemCheckPatterns_ShouldMatchHowManyIGot()
    {
        var match = RegexCache.ITEM_CHECK_PATTERNS
                              .Select(r => r.Match("How many Potions I got"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["thing"]
              .Value
              .Should()
              .Be("Potions");
    }
    #endregion

    #region SELL_ITEM_PATTERNS
    [Test]
    public void SellItemPatterns_ShouldMatchSellAllMy()
    {
        var match = RegexCache.SELL_ITEM_PATTERNS
                              .Select(r => r.Match("I will sell all my Gems"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("all");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Gems");
    }

    [Test]
    public void SellItemPatterns_ShouldMatchSellAmountOfMy()
    {
        var match = RegexCache.SELL_ITEM_PATTERNS
                              .Select(r => r.Match("I sell 5 of my Apples"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("5");
    }

    [Test]
    public void SellItemPatterns_ShouldMatchSellAmount()
    {
        var match = RegexCache.SELL_ITEM_PATTERNS
                              .Select(r => r.Match("I sell 10 Swords"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("10");
    }
    #endregion

    #region BUY_ITEM_PATTERNS
    [Test]
    public void BuyItemPatterns_ShouldMatchBuyAmount()
    {
        var match = RegexCache.BUY_ITEM_PATTERNS
                              .Select(r => r.Match("I will buy 3 Potions"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["amount"]
              .Value
              .Should()
              .Be("3");

        match.Groups["thing"]
             .Value
             .Should()
             .Be("Potions");
    }

    [Test]
    public void BuyItemPatterns_ShouldMatchBuyWithoutAmount()
    {
        var match = RegexCache.BUY_ITEM_PATTERNS
                              .Select(r => r.Match("I buy Sword"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();

        match!.Groups["thing"]
              .Value
              .Should()
              .Be("Sword");
    }
    #endregion

    #region MessageColorRegex
    [Test]
    public void MessageColorRegex_ShouldMatchColorCode()
    {
        var match = RegexCache.MessageColorRegex.Match("{=a");

        match.Success
             .Should()
             .BeTrue();
    }

    [Test]
    public void MessageColorRegex_ShouldNotMatchInvalidCode()
    {
        var match = RegexCache.MessageColorRegex.Match("{=1");

        match.Success
             .Should()
             .BeFalse();
    }

    [Test]
    public void MessageColorRegex_ShouldMatchInsideText()
    {
        var match = RegexCache.MessageColorRegex.Match("Hello {=c World");

        match.Success
             .Should()
             .BeTrue();
    }
    #endregion

    #region Case Insensitivity
    [Test]
    public void DepositPatterns_ShouldBeCaseInsensitive()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("i WILL DEPOSIT all MY swords"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();
    }

    [Test]
    public void WithdrawPatterns_ShouldBeCaseInsensitive()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("I WITHDRAW all my Gems"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .NotBeNull();
    }
    #endregion

    #region Non-matching inputs
    [Test]
    public void DepositPatterns_ShouldNotMatchUnrelatedText()
    {
        var match = RegexCache.DEPOSIT_PATTERNS
                              .Select(r => r.Match("Hello world"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .BeNull();
    }

    [Test]
    public void WithdrawPatterns_ShouldNotMatchUnrelatedText()
    {
        var match = RegexCache.WITHDRAW_PATTERNS
                              .Select(r => r.Match("Buy me a sword"))
                              .FirstOrDefault(m => m.Success);

        match.Should()
             .BeNull();
    }
    #endregion
}