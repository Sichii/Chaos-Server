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

using System.Windows.Forms;

namespace ChaosTool
{
    internal class WorldMapNodeTreeNode : TreeNode
    {
        internal Chaos.WorldMapNode WorldMapNode { get; }

        internal WorldMapNodeTreeNode(Chaos.WorldMapNode wmn, string name)
            : base(name)
        {
            WorldMapNode = wmn;
            Name = WorldMapNode.CheckSum.ToString();
        }
    }
}
