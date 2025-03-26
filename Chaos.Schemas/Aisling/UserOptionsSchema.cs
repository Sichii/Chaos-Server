#region
using System.Text.Json.Serialization;
#endregion

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of the object that contains all user options
/// </summary>
public sealed record UserOptionsSchema
{
    #region Other Options
    /// <summary>
    ///     Whether or not the player is accepting group invites.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool AllowGroup { get; set; }
    #endregion

    #region DontReorder
    /// <summary>
    ///     Option 1
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool ShowBodyAnimations { get; set; }

    /// <summary>
    ///     Option 2
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool ListenToHitSounds { get; set; }

    /// <summary>
    ///     Option 3
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool PriorityAnimations { get; set; }

    /// <summary>
    ///     Option 4
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Option4 { get; set; }

    /// <summary>
    ///     Option 5
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Option5 { get; set; }

    /// <summary>
    ///     Option 6. Whether or not the player is allowing exchanges.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool AllowExchange { get; set; }

    //Option7 doesnt show, so it is not saved or used

    /// <summary>
    ///     Option 8
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Option8 { get; set; }
    #endregion
}