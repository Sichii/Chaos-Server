using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.ClassMetaData;

/// <summary>
///     A node that stores metadata about an ability.
/// </summary>
/// <remarks>
///     Each file has a name of SClass{Number} where Number is the byte representing the class the metadata file is for <br />
///     <br />
///     Peasant = 0, <br />
///     Warrior = 1, <br />
///     Rogue = 2, <br />
///     Wizard = 3, <br />
///     Priest = 4, <br />
///     Monk = 5, <br />
///     <br />
///     There is a node named "Skill" with no properties at the beginnin <br />
///     after all the skills, there is a node "Skill_End" with 1 property, but that property is blank "00 01 00 00" followed by 3 more 00s "00
///     00 00" <br />
///     <br />
///     then, there is a node "Spell" with 1 empty property "00 01 00 00" <br />
///     then when spells finished, "Spell_End" with 1 empty property "00 01 00 00" <br />
///     <br />
///     Node name = name of skill with rank <br />
///     property 1 = {RequiredLevel}/{IsMaster}/{RequiredAbility} <br />
///     property 2 = {SpriteId}/??/?? <br />
///     property 3 = {Str}/{Int}/{Wis}/{Dex}/{Con} <br />
///     property 4 = "{PreRequisite1}/{RequiredLevel}" <br />
///     property 5 = "{PreRequisite1}/{RequiredLevel}"  - NOTE.. for no prereq, use "0/0" <br />
///     property 6 = {Description}
/// </remarks>
public sealed record AbilityMetaNode(string Name, bool IsSkill, BaseClass Class) : IMetaNode
{
    /// <summary>
    ///     The ability level required to learn the ability
    /// </summary>
    public int Ability { get; init; }
    /// <summary>
    ///     The constitution required to learn the ability
    /// </summary>
    public byte Con { get; init; }
    /// <summary>
    ///     A short description of the ability
    /// </summary>
    public string? Description { get; init; }
    /// <summary>
    ///     The dexterity required to learn the ability
    /// </summary>
    public byte Dex { get; init; }
    /// <summary>
    ///     The icon id used for the ability
    /// </summary>
    public ushort IconId { get; init; }
    /// <summary>
    ///     The intelligence required to learn the ability
    /// </summary>
    public byte Int { get; init; }
    /// <summary>
    ///     The numeric level required to use the ability
    /// </summary>
    public int Level { get; init; }
    /// <summary>
    ///     The level of the first pre-requisite ability needed to learn this ability
    /// </summary>
    public byte? PreReq1Level { get; init; }
    /// <summary>
    ///     The name of the first pre-requisite ability needed to learn this ability
    /// </summary>
    public string? PreReq1Name { get; init; }
    /// <summary>
    ///     The level of the second pre-requisite ability needed to learn this ability
    /// </summary>
    public byte? PreReq2Level { get; init; }
    /// <summary>
    ///     The name of the second pre-requisite ability needed to learn this ability
    /// </summary>
    public string? PreReq2Name { get; init; }
    /// <summary>
    ///     Whether or not you must be a master to learn the ability
    /// </summary>
    public bool RequiresMaster { get; init; }
    /// <summary>
    ///     The strength required to learn the ability
    /// </summary>
    public byte Str { get; init; }
    /// <summary>
    ///     The wisdom required to learn the ability
    /// </summary>
    public byte Wis { get; init; }
    /// <summary>
    ///     The class that can learn the ability
    /// </summary>
    public BaseClass Class { get; } = Class;
    /// <summary>
    ///     Whether or not the ability is a skill
    /// </summary>
    public bool IsSkill { get; } = IsSkill;

    /// <summary>
    ///     The name of the ability
    /// </summary>
    public string Name { get; } = Name;

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteInt16(6);

        writer.WriteString16($"{Level}/{Convert.ToByte(RequiresMaster)}/{Ability}");
        writer.WriteString16($"{IconId}/0/0");
        writer.WriteString16($"{Str}/{Int}/{Wis}/{Dex}/{Con}");
        writer.WriteString16($"{PreReq1Name ?? "0"}/{PreReq1Level?.ToString() ?? string.Empty}");
        writer.WriteString16($"{PreReq2Name ?? "0"}/{PreReq2Level?.ToString() ?? string.Empty}");
        writer.WriteString16(Description ?? string.Empty);
    }
}