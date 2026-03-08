#region
using Chaos.Scripting.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class SubjectiveScriptBaseTests
{
    [Test]
    public void Constructor_SetsSubjectCorrectly()
    {
        var subject = new MockScripted();
        var script = new ConcreteSubjectiveScript(subject);

        script.Subject
              .Should()
              .BeSameAs(subject);
    }

    [Test]
    public void Subject_TwoInstances_HaveDistinctSubjects()
    {
        var subject1 = new MockScripted();
        var subject2 = new MockScripted();

        var script1 = new ConcreteSubjectiveScript(subject1);
        var script2 = new ConcreteSubjectiveScript(subject2);

        script1.Subject
               .Should()
               .NotBeSameAs(script2.Subject);
    }

    private sealed class ConcreteSubjectiveScript : SubjectiveScriptBase<MockScripted>
    {
        public ConcreteSubjectiveScript(MockScripted subject)
            : base(subject) { }
    }
}