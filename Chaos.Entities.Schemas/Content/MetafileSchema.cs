using System.Collections;

namespace Chaos.Entities.Schemas.Content;

public record MetafileSchema(string Name, Dictionary<string, ICollection> Nodes);