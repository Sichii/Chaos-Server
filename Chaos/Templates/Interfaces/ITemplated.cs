namespace Chaos.Templates.Interfaces;

public interface ITemplated { }

public interface ITemplated<TKey, out TTemplate> : ITemplated where TTemplate: ITemplate<TKey>
{
    TTemplate? Template { get; }
}