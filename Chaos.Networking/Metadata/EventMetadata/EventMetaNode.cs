using Chaos.IO.Memory;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.EventMetadata;

/// <summary>
///     A node that stores metadata about an event
/// </summary>
/// <remarks>
///     events follow a structure like this <br />
///     <br />
///     "01_start" <br />
///     "01_title" <br />
///     - {QuestName} <br />
///     "01_id" <br />
///     - {??} -- icon? or maybe legend mark id? <br />
///     "01_qual" <br />
///     - {circle} <br />
///     - {classflag} <br />
///     "01_sum" <br />
///     - {Quest Summary} <br />
///     "01_result" <br />
///     - {??} -- icon after completing? <br />
///     "01_sub" <br />
///     - {maybe something to do with required pre-req quest} <br />
///     "01_reward" <br />
///     - {QuestRewards} <br />
///     "01_end" <br />
/// </remarks>
public sealed record EventMetaNode : MetaNodeBase
{
    public string? Id { get; init; }
    public string? PrerequisiteEventId { get; init; }
    public string? QualifyingCircles { get; init; }
    public string? QualifyingClasses { get; init; }
    public string? Result { get; init; }
    public string? Rewards { get; init; }
    public string? Summary { get; init; }
    public int Page { get; }

    /// <inheritdoc />
    public EventMetaNode(string name, int page)
        : base(name) => Page = page;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer)
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