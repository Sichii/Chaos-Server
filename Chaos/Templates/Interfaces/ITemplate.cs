namespace Chaos.Templates.Interfaces;

public interface ITemplate<out TKey>
{
    TKey TemplateKey { get; }
}