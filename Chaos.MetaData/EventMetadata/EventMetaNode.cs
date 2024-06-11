using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.EventMetaData;

/// <summary>
///     A node that stores metadata about an event
/// </summary>
/// <remarks>
///     events follow a structure like this
///     <br />
///     <br />
///     "01_start"
///     <br />
///     "01_title"
///     <br />
///     - {QuestName}
///     <br />
///     "01_id"
///     <br />
///     - {??} -- icon? or maybe legend mark id?
///     <br />
///     "01_qual"
///     <br />
///     - {circle}
///     <br />
///     - {classflag}
///     <br />
///     "01_sum"
///     <br />
///     - {Quest Summary}
///     <br />
///     "01_result"
///     <br />
///     - {??} -- icon after completing?
///     <br />
///     "01_sub"
///     <br />
///     - {maybe something to do with required pre-req quest}
///     <br />
///     "01_reward"
///     <br />
///     - {QuestRewards}
///     <br />
///     "01_end"
///     <br />
/// </remarks>
public sealed record EventMetaNode(string Name, int Page) : IMetaNode
{
    /// <summary>
    ///     The id of the event. To have the event marked as completed, you will need a legend mark with a key equal to this
    ///     id.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    ///     The id of the event that must be completed before this event can be started
    /// </summary>
    public string? PrerequisiteEventId { get; init; }

    /// <summary>
    ///     The circles (level ranges) that this event is available to
    /// </summary>
    public string? QualifyingCircles { get; init; }

    /// <summary>
    ///     The classes that this event is available to
    /// </summary>
    public string? QualifyingClasses { get; init; }

    /// <summary>
    ///     The text shown when the event is completed
    /// </summary>
    public string? Result { get; init; }

    /// <summary>
    ///     The rewards given when the event is completed
    /// </summary>
    public string? Rewards { get; init; }

    /// <summary>
    ///     The summary of the event
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    ///     The name of the event
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    ///     The page that this event is on
    /// </summary>
    public int Page { get; } = Page;

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        var prefix = $"{Page,2:D2}";

        var start = new MetaNode($"{prefix}_start");
        var title = new MetaNode($"{prefix}_title");
        title.Properties.Add(Name);

        var id = new MetaNode($"{prefix}_id");
        id.Properties.Add(Id ?? string.Empty);

        var qual = new MetaNode($"{prefix}_qual");
        qual.Properties.Add(QualifyingCircles ?? "1234567");
        qual.Properties.Add(QualifyingClasses ?? "012345");

        var sum = new MetaNode($"{prefix}_sum");
        sum.Properties.Add(Summary ?? string.Empty);

        var result = new MetaNode($"{prefix}_result");
        result.Properties.Add(Result ?? string.Empty);

        var sub = new MetaNode($"{prefix}_sub");
        sub.Properties.Add(PrerequisiteEventId ?? string.Empty);

        var reward = new MetaNode($"{prefix}_reward");
        reward.Properties.Add(Rewards ?? string.Empty);

        var end = new MetaNode($"{prefix}_end");

        start.Serialize(ref writer);
        title.Serialize(ref writer);
        id.Serialize(ref writer);
        qual.Serialize(ref writer);
        sum.Serialize(ref writer);
        result.Serialize(ref writer);
        sub.Serialize(ref writer);
        reward.Serialize(ref writer);
        end.Serialize(ref writer);
    }
}