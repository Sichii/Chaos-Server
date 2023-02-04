using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.MetaData;

public sealed record EventMetaSchema
{
    /// <summary>
    ///     The level range of the event, specified by what the game calls a "Circle"
    /// </summary>
    [JsonRequired]
    public LevelCircle Circle { get; set; }

    /// <summary>
    ///     A unique id specific to this event. In order for the event to show up as completed, a legend mark with a key equal to this Id must be
    ///     given.
    /// </summary>
    [JsonRequired]
    public string Id { get; set; } = null!;

    /// <summary>
    ///     The id of the event that must be completed before this event can be completed. This event will only show up blue(available) if the
    ///     previous event was completed.
    /// </summary>
    [JsonRequired]
    public string PrerequisiteEventId { get; set; } = null!;

    /// <summary>
    ///     The classes that can complete this event. If the player is not one of these classes, the event will not show up as blue(available).
    /// </summary>
    [JsonRequired]
    public ICollection<BaseClass> QualifyingClasses { get; set; } = Array.Empty<BaseClass>();

    /// <summary>
    ///     When the event is completed, the event will be marked green(completed) and show this text in place of it's summary.
    /// </summary>
    [JsonRequired]
    public string Result { get; set; } = null!;

    /// <summary>
    ///     The rewards given when the event is completed.
    /// </summary>
    [JsonRequired]
    public string Rewards { get; set; } = null!;

    /// <summary>
    ///     A brief summary of the event
    /// </summary>
    [JsonRequired]
    public string Summary { get; set; } = null!;

    /// <summary>
    ///     The title of the event
    /// </summary>
    [JsonRequired]
    public string Title { get; set; } = null!;
}