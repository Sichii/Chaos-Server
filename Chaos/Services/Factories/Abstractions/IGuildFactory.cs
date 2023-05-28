using Chaos.Collections;

namespace Chaos.Services.Factories.Abstractions;

public interface IGuildFactory
{
    Guild Create(string name);
}