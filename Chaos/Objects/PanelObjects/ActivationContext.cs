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

namespace Chaos
{
    internal sealed class ActivationContext
    {
        internal PanelObject Invoker { get; }
        internal Creature Target { get; }
        internal string Prompt { get; }

        internal ActivationContext(PanelObject invoker, Creature target = null, string prompt = null)
        {
            Invoker = invoker;
            Target = target;
            Prompt = prompt;
        }
    }
}
