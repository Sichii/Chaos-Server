#region
using System.ComponentModel;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a collection of user options
/// </summary>
public sealed class UserOptions
{
    private string GetDescription(UserOption userOption)
        => userOption switch
        {
            UserOption.Option1 => typeof(UserOptions).GetDescription(nameof(ShowBodyAnimations)),
            UserOption.Option2 => typeof(UserOptions).GetDescription(nameof(ListenToHitSounds)),
            UserOption.Option3 => typeof(UserOptions).GetDescription(nameof(PriorityAnimations)),
            UserOption.Option4 => typeof(UserOptions).GetDescription(nameof(Option4)),
            UserOption.Option5 => typeof(UserOptions).GetDescription(nameof(Option5)),
            UserOption.Option6 => typeof(UserOptions).GetDescription(nameof(AllowExchange)),
            UserOption.Option7 => string.Empty,
            UserOption.Option8 => typeof(UserOptions).GetDescription(nameof(Option8)),
            _                  => throw new ArgumentOutOfRangeException(nameof(userOption), userOption, null)
        };

    private bool IsEnabled(UserOption userOption)
        => userOption switch
        {
            UserOption.Option1 => ShowBodyAnimations,
            UserOption.Option2 => ListenToHitSounds,
            UserOption.Option3 => PriorityAnimations,
            UserOption.Option4 => Option4,
            UserOption.Option5 => Option5,
            UserOption.Option6 => AllowExchange,
            UserOption.Option7 => false,
            UserOption.Option8 => Option8,
            _                  => throw new ArgumentOutOfRangeException(nameof(userOption), userOption, null)
        };

    /// <summary>
    ///     Toggles the given UserOption
    /// </summary>
    /// <param name="opt">
    ///     Option to toggle
    /// </param>
    public void Toggle(UserOption opt)
    {
        switch (opt)
        {
            case UserOption.Option1:
                ShowBodyAnimations = !ShowBodyAnimations;

                break;
            case UserOption.Option2:
                ListenToHitSounds = !ListenToHitSounds;

                break;
            case UserOption.Option3:
                PriorityAnimations = !PriorityAnimations;

                break;
            case UserOption.Option4:
                Option4 = !Option4;

                break;
            case UserOption.Option5:
                Option5 = !Option5;

                break;
            case UserOption.Option6:
                AllowExchange = !AllowExchange;

                break;
            case UserOption.Option7:
                //not used, don't use

                break;
            case UserOption.Option8:
                Option8 = !Option8;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(opt), opt, "Unknown enum value");
        }
    }

    /// <summary>
    ///     Toggles the group option
    /// </summary>
    public void ToggleGroup() => AllowGroup = !AllowGroup;

    /// <summary>
    ///     Prints the option as a string in the format required by the networking layer
    /// </summary>
    /// <param name="opt">
    ///     The option to print as a string
    /// </param>
    public string ToString(UserOption opt)
    {
        if (opt == UserOption.Request)
            return ToString();

        const string OPTIONS_FORMAT = "{0,-25}:{1,-3}";

        var enabled = IsEnabled(opt);
        var description = GetDescription(opt);

        return string.Format(OPTIONS_FORMAT, $"{(byte)opt}{description}", enabled ? "ON" : "OFF");
    }

    /// <summary>
    ///     Prints all of the options as a string in the format required by the networking layer
    /// </summary>
    public override string ToString()
    {
        var options = new string[8];

        for (var i = 0; i < 8; i++)
            options[i] = ToString((UserOption)i + 1)
                .Remove(0, 1);

        return $"0{string.Join("\t", options)}";
    }

    #region OtherOptions
    /// <summary>
    ///     Whether or not to allow grouping
    /// </summary>
    public bool AllowGroup { get; set; } = true;

    /// <summary>
    ///     The social status of the aisling
    /// </summary>
    public SocialStatus SocialStatus { get; set; }
    #endregion

    #region F4 Options
    /// <summary>
    ///     Whether to show body animations
    /// </summary>
    [Description("Show body animations")]
    public bool ShowBodyAnimations { get; set; } = true;

    /// <summary>
    ///     Whether to listen to hit sounds
    /// </summary>
    [Description("Listen to hit sounds")]
    public bool ListenToHitSounds { get; set; } = true;

    /// <summary>
    ///     Whether to use priority animations
    /// </summary>
    [Description("Use Priority Animations")]
    public bool PriorityAnimations { get; set; } = true;

    /// <summary>
    ///     Unused option 4
    /// </summary>
    [Description("Option 4")]
    public bool Option4 { get; set; } = true;

    /// <summary>
    ///     Unused option 5
    /// </summary>
    [Description("Option 5")]
    public bool Option5 { get; set; } = true;

    /// <summary>
    ///     Whether to allow exchanges
    /// </summary>
    [Description("Allow Exchanges")]
    public bool AllowExchange { get; set; } = true;

    /// <summary>
    ///     Unused option 8
    /// </summary>
    [Description("Option 8")]
    public bool Option8 { get; set; } = true;
    #endregion
}