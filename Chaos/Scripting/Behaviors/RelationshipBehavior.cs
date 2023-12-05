using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Behaviors;

public class RelationshipBehavior
{
    public virtual bool IsFriendlyTo(Creature source, Creature target)
        => source switch
        {
            Aisling aisling => target switch
            {
                Aisling other  => IsFriendlyTo(aisling, other),
                Merchant other => IsFriendlyTo(aisling, other),
                Monster other  => IsFriendlyTo(aisling, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Merchant merchant => target switch
            {
                Aisling other  => IsFriendlyTo(merchant, other),
                Merchant other => IsFriendlyTo(merchant, other),
                Monster other  => IsFriendlyTo(merchant, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Monster monster => target switch
            {
                Aisling other  => IsFriendlyTo(monster, other),
                Merchant other => IsFriendlyTo(monster, other),
                Monster other  => IsFriendlyTo(monster, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };

    protected virtual bool IsFriendlyTo(Aisling source, Merchant target) => true;

    protected virtual bool IsFriendlyTo(Merchant source, Merchant target) => source.Equals(target);

    protected virtual bool IsFriendlyTo(Monster source, Merchant target) => false;

    protected virtual bool IsFriendlyTo(Aisling source, Aisling target)
    {
        if (source.Equals(target))
            return true;

        //var inPvpMap = false;
        var inGroup = source.Group?.Contains(target) ?? false;

        /* uncomment this if you want friendly fire in pvp maps
        if (inPvpMap)
            return false;
        */

        if (inGroup)
            return true;

        return false;
    }

    protected virtual bool IsFriendlyTo(Aisling source, Monster target) => false;

    protected virtual bool IsFriendlyTo(Merchant source, Aisling target) => true;

    protected virtual bool IsFriendlyTo(Merchant source, Monster target) => false;

    protected virtual bool IsFriendlyTo(Monster source, Aisling target) => false;

    protected virtual bool IsFriendlyTo(Monster source, Monster target) => source.Equals(target);

    public virtual bool IsHostileTo(Creature source, Creature target)
        => source switch
        {
            Aisling aisling => target switch
            {
                Aisling other  => IsHostileTo(aisling, other),
                Merchant other => IsHostileTo(aisling, other),
                Monster other  => IsHostileTo(aisling, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Merchant merchant => target switch
            {
                Aisling other  => IsHostileTo(merchant, other),
                Merchant other => IsHostileTo(merchant, other),
                Monster other  => IsHostileTo(merchant, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Monster monster => target switch
            {
                Aisling other  => IsHostileTo(monster, other),
                Merchant other => IsHostileTo(monster, other),
                Monster other  => IsHostileTo(monster, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };

    protected virtual bool IsHostileTo(Aisling source, Aisling target)
    {
        if (source.Equals(target))
            return false;

        // ReSharper disable once ConvertToConstant.Local
        var onPvpMap = false;
        var inGroup = source.Group?.Contains(target) ?? false;

        //comment this if you want friendly fire in pvp maps
        if (inGroup)
            return false;

        return onPvpMap;
    }

    protected virtual bool IsHostileTo(Aisling source, Merchant target) => false;

    protected virtual bool IsHostileTo(Aisling source, Monster target) => true;

    protected virtual bool IsHostileTo(Merchant source, Aisling target) => false;

    protected virtual bool IsHostileTo(Merchant source, Merchant target) => false;

    protected virtual bool IsHostileTo(Merchant source, Monster target) => true;

    protected virtual bool IsHostileTo(Monster source, Aisling target) => true;

    protected virtual bool IsHostileTo(Monster source, Merchant target) => true;

    protected virtual bool IsHostileTo(Monster source, Monster target) => false;
}