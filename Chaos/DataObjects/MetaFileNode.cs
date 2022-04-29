using System.Collections.Generic;

namespace Chaos.DataObjects;

public record MetafileNode(string Name, ICollection<string> Properties);