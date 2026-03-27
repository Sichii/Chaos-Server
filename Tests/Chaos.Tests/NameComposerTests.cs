#region
using Chaos.DarkAges.Definitions;
using Chaos.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class NameComposerTests
{
    #region Constructor
    [Test]
    public void Constructor_ShouldSetComposedNameToBaseName()
    {
        var composer = new NameComposer("Sword");

        composer.ComposedName
                .Should()
                .Be("Sword");
    }
    #endregion

    #region Full Composition
    [Test]
    public void FullComposition_WithPrefixSuffixAndColor_OnDyeableItem()
    {
        var composer = new NameComposer("Sword", true);

        composer.SetPrefix("Great");
        composer.SetSuffix("of Doom");
        composer.SetColor(DisplayColor.Apple);

        composer.ComposedName
                .Should()
                .Be("Apple Great Sword of Doom");
    }
    #endregion

    #region SetPrefix
    [Test]
    public void SetPrefix_ShouldPrependToBaseName()
    {
        var composer = new NameComposer("Sword");

        composer.SetPrefix("Great");

        composer.ComposedName
                .Should()
                .Be("Great Sword");
    }
    #endregion

    #region SetSuffix
    [Test]
    public void SetSuffix_ShouldAppendToBaseName()
    {
        var composer = new NameComposer("Sword");

        composer.SetSuffix("of Doom");

        composer.ComposedName
                .Should()
                .Be("Sword of Doom");
    }
    #endregion

    #region SetCustomName
    [Test]
    public void SetCustomName_ShouldOverrideComposedName()
    {
        var composer = new NameComposer("Sword");
        composer.SetPrefix("Great");
        composer.SetSuffix("of Doom");

        composer.SetCustomName("Excalibur");

        composer.ComposedName
                .Should()
                .Be("Excalibur");
    }

    [Test]
    public void SetCustomName_ToNull_ShouldRevertToBaseComposition()
    {
        var composer = new NameComposer("Sword");
        composer.SetPrefix("Great");
        composer.SetCustomName("Excalibur");

        composer.SetCustomName(null);

        composer.ComposedName
                .Should()
                .Be("Great Sword");
    }

    [Test]
    public void SetCustomName_ToEmpty_ShouldRevertToBaseComposition()
    {
        var composer = new NameComposer("Sword");
        composer.SetSuffix("of Doom");
        composer.SetCustomName("Excalibur");

        composer.SetCustomName(string.Empty);

        composer.ComposedName
                .Should()
                .Be("Sword of Doom");
    }
    #endregion

    #region SetColor
    [Test]
    public void SetColor_OnDyeableItem_ShouldPrependColor()
    {
        var composer = new NameComposer("Sword", true);

        composer.SetColor(DisplayColor.Apple);

        composer.ComposedName
                .Should()
                .Be("Apple Sword");
    }

    [Test]
    public void SetColor_OnNonDyeableItem_ShouldNotPrependColor()
    {
        var composer = new NameComposer("Sword");

        composer.SetColor(DisplayColor.Apple);

        composer.ComposedName
                .Should()
                .Be("Sword");
    }

    [Test]
    public void SetColor_WithDefault_ShouldNotPrependEvenOnDyeable()
    {
        var composer = new NameComposer("Sword", true);

        composer.SetColor(DisplayColor.Default);

        composer.ComposedName
                .Should()
                .Be("Sword");
    }
    #endregion

    #region Implicit Operators
    [Test]
    public void ImplicitOperator_StringFromComposer_ShouldReturnComposedName()
    {
        var composer = new NameComposer("Sword");
        composer.SetPrefix("Great");

        string result = composer;

        result.Should()
              .Be("Great Sword");
    }

    [Test]
    public void ImplicitOperator_ComposerFromString_ShouldCreateComposerWithBaseName()
    {
        NameComposer composer = "Sword";

        composer.ComposedName
                .Should()
                .Be("Sword");
    }
    #endregion
}