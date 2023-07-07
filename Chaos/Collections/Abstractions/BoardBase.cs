using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Models.Board;
using Chaos.Networking.Abstractions;

namespace Chaos.Collections.Abstractions;

public abstract class BoardBase : IEnumerable<Post>
{
    protected static readonly TimeSpan MINIMUM_SHOW_INTERVAL = TimeSpan.FromMilliseconds(50);
    public ushort BoardId { get; }
    public string Key { get; }
    protected ConcurrentDictionary<uint, DateTime> LastShown { get; }
    public ICollection<string> Moderators { get; }
    public string Name { get; }
    protected IIdGenerator<short> PostIdGenerator { get; }
    protected ConcurrentDictionary<short, Post> Posts { get; }
    protected AutoReleasingMonitor Sync { get; }

    protected BoardBase(
        ushort boardId,
        string name,
        string key,
        IEnumerable<Post>? posts = null,
        IEnumerable<string>? moderators = null
    )
    {
        BoardId = boardId;
        Name = name;
        Key = key;
        Posts = new ConcurrentDictionary<short, Post>();
        PostIdGenerator = new SequentialIdGenerator<short>();
        Moderators = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Sync = new AutoReleasingMonitor();
        LastShown = new ConcurrentDictionary<uint, DateTime>();

        if (moderators is not null)
            Moderators.AddRange(moderators);

        //for existing posts, number them sequentially
        //starting at 1 for the oldest, counting up to the newest
        if (posts is not null)
            foreach (var post in posts.OrderBy(p => p.CreationDate))
            {
                var postActual = post with { PostId = PostIdGenerator.NextId };

                Posts.TryAdd(postActual.PostId, postActual);
            }
    }

    /// <inheritdoc />
    public IEnumerator<Post> GetEnumerator()
    {
        using var @lock = Sync.Enter();

        var posts = Posts.Values.ToList();

        return posts.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual bool Contains(short postId)
    {
        using var @lock = Sync.Enter();

        return Posts.ContainsKey(postId);
    }

    public virtual bool Contains(Post post)
    {
        using var @lock = Sync.Enter();

        return Contains(post.PostId);
    }

    public abstract bool Delete(IWorldClient deletedBy, short postId);

    public abstract void Highlight(IWorldClient highlightedBy, short postId);

    public abstract void Post(
        IWorldClient addedBy,
        string author,
        string subject,
        string message,
        bool highlighted = false
    );

    protected virtual bool ShouldShowTo(uint clientId) => !LastShown.TryGetValue(clientId, out var lastShown)
                                                          || (DateTime.UtcNow.Subtract(lastShown) > MINIMUM_SHOW_INTERVAL);

    public abstract void Show(IWorldClient client, short startPostId);

    public abstract void ShowPost(IWorldClient client, short postId, BoardControls control);

    public virtual bool TryGet(short postId, [MaybeNullWhen(false)] out Post post)
    {
        using var @lock = Sync.Enter();

        return Posts.TryGetValue(postId, out post);
    }

    public abstract void UnHighlight(IWorldClient unhighlightedBy, ref Post post);
}