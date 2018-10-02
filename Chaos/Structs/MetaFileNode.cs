// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System.Collections.Generic;

namespace Chaos
{
    internal struct MetafileNode
    {
        internal string Name { get; }
        internal List<string> Properties { get; }

        /// <summary>
        /// Base constructor for a structure representing a metafile, which contains game meta-data.
        /// </summary>
        internal MetafileNode(string name)
        {
            Name = name;
            Properties = new List<string>();
        }
    }
}
