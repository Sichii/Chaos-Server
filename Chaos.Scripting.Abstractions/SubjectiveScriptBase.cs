namespace Chaos.Scripting.Abstractions;

public abstract class SubjectiveScriptBase<T> : ScriptBase where T: IScripted
{
    public T Subject { get; }

    protected SubjectiveScriptBase(T subject) => Subject = subject;
}