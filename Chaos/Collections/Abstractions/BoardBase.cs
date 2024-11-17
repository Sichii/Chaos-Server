#region
using Chaos.Common.Abstractions;
using Chaos.Common.Identity;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Board;
using Chaos.Models.World;
#endregion

namespace Chaos.Collections.Abstractions;

public abstract class BoardBase : IEnumerable<Post>
{
    protected static readonly TimeSpan MINIMUM_SHOW_INTERVAL = TimeSpan.FromMilliseconds(50);
    public ushort BoardId { get; }
    public string Key { get; }
    protected ConcurrentDictionary<uint, DateTime> LastShown { get; }
    public string Name { get; }
    protected IIdGenerator<short> PostIdGenerator { get; }
    internal ConcurrentDictionary<short, Post> Posts { get; }
    protected Lock Sync { get; }

    protected BoardBase(
        ushort boardId,
        string name,
        string key,
        IEnumerable<Post>? posts = null)
    {
        BoardId = boardId;
        Name = name;
        Key = key;
        Posts = new ConcurrentDictionary<short, Post>();
        PostIdGenerator = new SequentialIdGenerator<short>();
        Sync = new Lock();
        LastShown = new ConcurrentDictionary<uint, DateTime>();

        //for existing posts, number them sequentially
        //starting at 1 for the oldest, counting up to the newest
        if (posts is not null)
            foreach (var post in posts.OrderBy(p => p.CreationDate))
            {
                var postActual = post with
                {
                    PostId = PostIdGenerator.NextId
                };

                Posts[postActual.PostId] = postActual;
            }
    }

    /// <inheritdoc />
    public IEnumerator<Post> GetEnumerator()
    {
        using var @lock = Sync.EnterScope();

        var posts = Posts.Values.ToList();

        return posts.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual bool Contains(short postId)
    {
        using var @lock = Sync.EnterScope();

        return Posts.ContainsKey(postId);
    }

    public virtual bool Contains(Post post)
    {
        using var @lock = Sync.EnterScope();

        return Contains(post.PostId);
    }

    public abstract bool Delete(Aisling deletedBy, short postId);

    public abstract void Highlight(Aisling highlightedBy, short postId);

    public abstract void Post(
        Aisling addedBy,
        string author,
        string subject,
        string message,
        bool highlighted = false);

    protected virtual bool ShouldShowTo(uint clientId)
        => !LastShown.TryGetValue(clientId, out var lastShown) || (DateTime.UtcNow.Subtract(lastShown) > MINIMUM_SHOW_INTERVAL);

    public abstract void Show(Aisling aisling, short startPostId = short.MaxValue);

    public abstract void ShowPost(Aisling aisling, short postId, BoardControls control);

    public virtual bool TryGet(short postId, [MaybeNullWhen(false)] out Post post)
    {
        using var @lock = Sync.EnterScope();

        return Posts.TryGetValue(postId, out post);
    }

    public abstract void UnHighlight(Aisling unhighlightedBy, ref Post post);
}