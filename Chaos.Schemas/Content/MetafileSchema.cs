using System.Collections;

namespace Chaos.Schemas.Content;

public sealed record MetafileSchema(string Name, Dictionary<string, ICollection> Nodes);