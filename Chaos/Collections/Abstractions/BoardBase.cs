#region
using Chaos.Common.Abstractions;
using Chaos.Common.Identity;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.NLog.Logging.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Represents an in game message board that can be posted to
/// </summary>
public abstract class BoardBase : IEnumerable<Post>, ITransformableCollection
{
    /// <summary>
    ///     Defines the minimum time interval to wait before showing the same content to a client again
    /// </summary>
    protected static readonly TimeSpan MINIMUM_SHOW_INTERVAL = TimeSpan.FromMilliseconds(50);

    /// <summary>
    ///     A unique numeric identifier for the board. This id is auto generated and used by the networking layer to identify
    ///     the board
    /// </summary>
    public ushort BoardId { get; }

    /// <summary>
    ///     A unique string identifier for the board. This id is the template key of the board, and is used to identify the
    ///     board in the storage mechanism
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Tracks the most recent time a specific client was shown content on the board. The client spams the server with
    ///     requests, but it would be a waste to spam the client back
    /// </summary>
    protected ConcurrentDictionary<uint, DateTime> LastShown { get; }

    /// <summary>
    ///     The name of the board
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     A thread-safe id generator used to generate post ids
    /// </summary>
    protected IIdGenerator<short> PostIdGenerator { get; }

    /// <summary>
    ///     A thread-safe collection of posts on the board
    /// </summary>
    internal ConcurrentDictionary<short, Post> Posts { get; }

    /// <summary>
    ///     The synchronization mechanism used to ensure thread safety
    /// </summary>
    protected Lock Sync { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BoardBase" /> class
    /// </summary>
    /// <param name="boardId">
    ///     A unique numeric identifier for the board
    /// </param>
    /// <param name="name">
    ///     The name of the board
    /// </param>
    /// <param name="key">
    ///     A unique string identifier for the board
    /// </param>
    /// <param name="posts">
    ///     The posts on the board
    /// </param>
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

    /// <summary>
    ///     Determines whether the board contains a post with the specified id
    /// </summary>
    /// <param name="postId">
    ///     The id of the post to check for existence.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the post exists in the board, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public virtual bool Contains(short postId)
    {
        using var @lock = Sync.EnterScope();

        return Posts.ContainsKey(postId);
    }

    /// <summary>
    ///     Determines whether a specified post exists within the collection
    /// </summary>
    /// <param name="post">
    ///     The post to locate in the collection
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the post exists in the collection, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public virtual bool Contains(Post post)
    {
        using var @lock = Sync.EnterScope();

        return Contains(post.PostId);
    }

    /// <summary>
    ///     Deletes a post from the board
    /// </summary>
    /// <param name="deletedBy">
    ///     The <see cref="Aisling" /> instance attempting to delete the post
    /// </param>
    /// <param name="postId">
    ///     The id of the post to delete
    /// </param>
    /// <return>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the post is successfully deleted, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </return>
    public abstract bool Delete(Aisling deletedBy, short postId);

    /// <summary>
    ///     Highlights a specific post on the board
    /// </summary>
    /// <param name="highlightedBy">
    ///     The <see cref="Aisling" /> performing the highlight action
    /// </param>
    /// <param name="postId">
    ///     The identifier of the post to highlight
    /// </param>
    public abstract void Highlight(Aisling highlightedBy, short postId);

    /// <summary>
    ///     Posts a new message to the board
    /// </summary>
    /// <param name="addedBy">
    ///     The Aisling who is adding the post
    /// </param>
    /// <param name="author">
    ///     The author of the post
    /// </param>
    /// <param name="subject">
    ///     The subject of the post
    /// </param>
    /// <param name="message">
    ///     The message content of the post
    /// </param>
    /// <param name="highlighted">
    ///     Indicates whether the post should be highlighted
    /// </param>
    public abstract void Post(
        Aisling addedBy,
        string author,
        string subject,
        string message,
        bool highlighted = false);

    /// <summary>
    ///     Determines if the board should be shown to the specified client. This uses <see cref="LastShown" /> and
    ///     <see cref="MINIMUM_SHOW_INTERVAL" />
    /// </summary>
    /// <param name="clientId">
    ///     The unique identifier of the client
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the board should be shown to the client, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    protected virtual bool ShouldShowTo(uint clientId)
        => !LastShown.TryGetValue(clientId, out var lastShown) || (DateTime.UtcNow.Subtract(lastShown) > MINIMUM_SHOW_INTERVAL);

    /// <summary>
    ///     Displays the board content to the specified Aisling
    /// </summary>
    /// <param name="aisling">
    ///     The Aisling to whom the board content is displayed
    /// </param>
    /// <param name="startPostId">
    ///     The ID of the post from which to start displaying, defaults to the maximum possible value if not provided. Ids
    ///     start high and count down (newest to oldest)
    /// </param>
    public abstract void Show(Aisling aisling, short startPostId = short.MaxValue);

    /// <summary>
    ///     Displays a post on the board to the specified Aisling based on the post id and the control
    /// </summary>
    /// <param name="aisling">
    ///     The Aisling for whom the post is being displayed
    /// </param>
    /// <param name="postId">
    ///     The identifier of the post to be displayed
    /// </param>
    /// <param name="control">
    ///     The type of request made on the board
    /// </param>
    public abstract void ShowPost(Aisling aisling, short postId, BoardControls control);

    /// <summary>
    ///     Attempts to retrieve a post from the board
    /// </summary>
    /// <param name="postId">
    ///     The ID of the post to retrieve
    /// </param>
    /// <param name="post">
    ///     The retrieved post if found; otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the post is found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public virtual bool TryGet(short postId, [MaybeNullWhen(false)] out Post post)
    {
        using var @lock = Sync.EnterScope();

        return Posts.TryGetValue(postId, out post);
    }

    /// <summary>
    ///     Removes the highlight from a specified post on the board
    /// </summary>
    /// <param name="unhighlightedBy">
    ///     The <see cref="Aisling" /> who unhighlights the post
    /// </param>
    /// <param name="post">
    ///     A reference to the <see cref="Post" /> to be unhighlighted
    /// </param>
    public abstract void UnHighlight(Aisling unhighlightedBy, ref Post post);
}