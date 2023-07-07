using System.Diagnostics;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Board;
using Chaos.Networking.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Collections;

public sealed class MailBox : BoardBase
{
    public const ushort BOARD_ID = 0;
    private readonly ILogger<MailBox> Logger;

    /// <inheritdoc />
    public MailBox(
        string ownerName,
        ILogger<MailBox> logger,
        IEnumerable<Post>? posts = null
    )
        : base(
            BOARD_ID,
            "Mail",
            ownerName,
            posts) => Logger = logger;

    /// <inheritdoc />
    public override bool Delete(IWorldClient deletedBy, short postId)
    {
        using var @lock = Sync.Enter();

        //remove post from mailbox if it exists
        if (!Posts.TryRemove(postId, out var post))
        {
            deletedBy.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such message", false);

            return false;
        }

        //only an admin or the owner can delete posts
        if (!deletedBy.Aisling.IsAdmin && !Key.EqualsI(deletedBy.Aisling.Name))
        {
            deletedBy.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "You lack the permission", false);

            Logger.WithProperty(deletedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to delete {@PostSubject} from {@MailboxOwnerName}'s mailbox without permission",
                      deletedBy.Aisling.Name,
                      post.Subject,
                      Key);

            return false;
        }

        deletedBy.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "Message deleted", true);

        Logger.WithProperty(deletedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogDebug(
                  "{@AislingName} deleted {@PostSubject} from {@MailboxOwnerName}'s mailbox",
                  deletedBy.Aisling.Name,
                  post.Subject,
                  Key);

        return true;
    }

    /// <inheritdoc />
    public override void Highlight(IWorldClient highlightedBy, short postId)
    {
        using var @lock = Sync.Enter();

        //if post doesnt exist
        if (!Posts.TryGetValue(postId, out var post))
        {
            highlightedBy.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such message", false);

            return;
        }

        //only admins and the owner can highlight posts
        if (!highlightedBy.Aisling.IsAdmin && !Key.EqualsI(highlightedBy.Aisling.Name))
        {
            highlightedBy.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "You lack the permission", false);

            Logger.WithProperty(highlightedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to highlight {@PostSubject} in {@MailboxOwnerName}'s mailbox without permission",
                      highlightedBy.Aisling.Name,
                      post.Subject,
                      Key);

            return;
        }

        //highlight post
        post = post with { IsHighlighted = true };
        Posts[postId] = post;
        highlightedBy.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "Message highlighted", true);

        Logger.WithProperty(highlightedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogDebug(
                  "{@AislingName} highlighted {@PostSubject} in {@MailboxOwnerName}'s mailbox",
                  highlightedBy.Aisling.Name,
                  post.Subject,
                  Key);
    }

    /// <inheritdoc />
    public override void Post(
        IWorldClient addedBy,
        string author,
        string subject,
        string message,
        bool highlighted = false
    )
    {
        using var @lock = Sync.Enter();

        //create post
        var post = new Post(
            PostIdGenerator.NextId,
            author,
            subject,
            message,
            DateTime.UtcNow,
            highlighted);

        //add post to mailbox
        if (!Posts.TryAdd(post.PostId, post))
            throw new UnreachableException(
                "Failed to add post to board. This should be impossible due to the lock and threadsafe postId generator");

        addedBy.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "Message sent", true);

        Logger.WithProperty(addedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogDebug(
                  "{@AislingName} posted {@PostSubject} in {@MailboxOwnerName}'s mailbox",
                  addedBy.Aisling.Name,
                  post.Subject,
                  Key);
    }

    /// <inheritdoc />
    public override void Show(IWorldClient client, short startPostId)
    {
        using var @lock = Sync.Enter();

        if (!ShouldShowTo(client.Id))
            return;

        var now = DateTime.UtcNow;

        LastShown.AddOrUpdate(client.Id, now, (_, _) => now);

        if (!client.Aisling.IsAdmin && !Key.EqualsI(client.Aisling.Name))
        {
            Logger.WithProperty(client)
                  .WithProperty(this)
                  .LogWarning(
                      "{@AislingName} attempted to view {@MailboxOwnerName}'s mailbox without permission",
                      client.Aisling.Name,
                      Key);

            return;
        }

        client.SendBoard(this, startPostId);
    }

    /// <inheritdoc />
    public override void ShowPost(IWorldClient client, short postId, BoardControls control)
    {
        using var @lock = Sync.Enter();

        var postIdActual = postId;

        switch (control)
        {
            case BoardControls.PreviousPage:
                //client sends postId + 1 as an estimate of the previous post id
                //easier to just ignore this
                postId--;

                postIdActual = Posts.Keys.NextHighest(postId);

                break;
            case BoardControls.RequestPost:
                break;
            case BoardControls.NextPage:
                //client sends postId - 1 as an estimate of the next post id
                //easier to just ignore this
                postId++;

                postIdActual = Posts.Keys.NextLowest(postId);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(control), control, null);
        }

        if (!Posts.TryGetValue(postIdActual, out var post))
            return;

        if (!client.Aisling.IsAdmin && !Key.EqualsI(client.Aisling.Name))
        {
            Logger.WithProperty(client)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to view {@PostSubject} in {@MailboxOwnerName}'s mailbox without permission",
                      client.Aisling.Name,
                      post.Subject,
                      Key);

            return;
        }

        //mark as read
        UnHighlight(client, ref post);
        client.SendPost(post, true, Posts.Keys.Max() == postIdActual);
    }

    /// <inheritdoc />
    public override void UnHighlight(IWorldClient unhighlightedBy, ref Post post)
    {
        using var @lock = Sync.Enter();

        if (!post.IsHighlighted)
            return;

        post = post with { IsHighlighted = false };

        Posts[post.PostId] = post;

        Logger.WithProperty(unhighlightedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogDebug(
                  "{@AislingName} unhighlighted {@PostSubject} in {@MailboxOwnerName}'s mailbox",
                  unhighlightedBy.Aisling.Name,
                  post.Subject,
                  Key);
    }
}