using Chaos.Clients.Interfaces;
using Chaos.Core.Definitions;
using Chaos.WorldObjects;

namespace Chaos.Factories.Interfaces;

public interface IUserFactory
{
    User CreateUser(string name, Gender gender, int hairStyle, DisplayColor hairColor);
    User CreateUser(IWorldClient worldClient, string name);
}