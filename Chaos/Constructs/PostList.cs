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

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class PostList : IEnumerable<Post>
    {
        private readonly object Sync = new object();
        [JsonIgnore]
        private List<Post> Messages;
        private int Counter => Messages.Count;

        public IEnumerator<Post> GetEnumerator() => Messages.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        internal Post this[byte postNum] => postNum < Counter ? Messages[postNum] : default(Post);

        [JsonConstructor]
        internal PostList(List<Post> messages = default(List<Post>))
        {
            Messages = messages ?? new List<Post>();
        }

        internal void AddPost(Post post)
        {
            lock (Sync)
                Messages[post.PostId] = post;
        }

        internal bool RemovePost(int PostNum)
        {
            lock (Sync)
            {
                if (PostNum < Counter)
                {
                    Messages.RemoveAt(PostNum);
                    return true;
                }

                return false;
            }
        }

        internal IEnumerable<Post> Reverse()
        {
            lock (Sync)
                for (int i = Counter - 1; i != 0; i--)
                    yield return Messages[i];
        }
    }
}
