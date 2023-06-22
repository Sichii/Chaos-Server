using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.MetaData;

/// <summary>
///     Represents the serializable schema of the details of an event as part of the event meta data
/// </summary>
public sealed record EventMetaSchema
{
    /// <summary>
    ///     A unique id specific to this event. In order for the event to show up as completed, a legend mark with a key equal
    ///     to this Id must be
    ///     given
    /// </summary>
    [JsonRequired]
    public string Id { get; set; } = null!;
    /// <summary>
    ///     Default null<br />If set, this is the page that the event will show up on(starting at index of 1). If null, the
    ///     page will be determined by the event's lowest qualifying circle
    /// </summary>
    public int? PageOverride { get; set; }

    /// <summary>
    ///     Default null<br />The id of the event that must be completed before this event can be completed. This event will
    ///     only show up blue(available) if the
    ///     previous event was completed
    /// </summary>
    public string? PrerequisiteEventId { get; set; }

    /// <summary>
    ///     Default null<br />if set, these are the circles this quest is available to.<br />If null, the event will be
    ///     available to all circles
    /// </summary>
    public ICollection<LevelCircle>? QualifyingCircles { get; set; }

    /// <summary>
    ///     Default null<br />If set, these are the classes this event is available to.<br />If null, the event will be
    ///     available to all classes
    /// </summary>
    public ICollection<BaseClass>? QualifyingClasses { get; set; } = Array.Empty<BaseClass>();

    /// <summary>
    ///     When the event is completed, the event will be marked green(completed) and show this text in place of it's summary.
    /// </summary>
    [JsonRequired]
    public string Result { get; set; } = null!;

    /// <summary>
    ///     Default null.<br />The rewards given when the event is completed
    /// </summary>
    public string? Rewards { get; set; }

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