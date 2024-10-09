using System.Diagnostics;
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Board;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;

namespace Chaos.Collections;

public sealed class MailBox : BoardBase
{
    public const ushort BOARD_ID = 0;
    private readonly ILogger<MailBox> Logger;

    /// <inheritdoc />
    public MailBox(string ownerName, ILogger<MailBox> logger, IEnumerable<Post>? posts = null)
        : base(
            BOARD_ID,
            "Mail",
            ownerName,
            posts)
        => Logger = logger;

    /// <inheritdoc />
    public override bool Delete(Aisling deletedBy, short postId)
    {
        using var @lock = Sync.Enter();

        //check that post exists
        if (!Posts.TryGetValue(postId, out var post))
        {
            deletedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such message", false);

            return false;
        }

        //only an admin or the owner can delete posts
        if (!deletedBy.IsAdmin && !Key.EqualsI(deletedBy.Name))
        {
            deletedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "You lack the permission", false);

            Logger.WithTopics(
                      Topics.Entities.MailBox,
                      Topics.Actions.Update,
                      Topics.Entities.Mail,
                      Topics.Actions.Delete,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(deletedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to delete {@PostSubject} from {@MailboxOwnerName}'s mailbox without permission",
                      deletedBy.Name,
                      post.Subject,
                      Key);

            return false;
        }

        //remove post from mailbox
        if (!Posts.TryRemove(postId, out _))
            throw new UnreachableException("We already checked that the post exists");

        deletedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "Message deleted", true);

        Logger.WithTopics(
                  Topics.Entities.MailBox,
                  Topics.Actions.Update,
                  Topics.Entities.Mail,
                  Topics.Actions.Delete)
              .WithProperty(deletedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} deleted {@PostSubject} from {@MailboxOwnerName}'s mailbox",
                  deletedBy.Name,
                  post.Subject,
                  Key);

        return true;
    }

    /// <inheritdoc />
    public override void Highlight(Aisling highlightedBy, short postId)
    {
        using var @lock = Sync.Enter();

        //if post doesnt exist
        if (!Posts.TryGetValue(postId, out var post))
        {
            highlightedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such message", false);

            return;
        }

        //only admins and the owner can highlight posts
        if (!highlightedBy.IsAdmin && !Key.EqualsI(highlightedBy.Name))
        {
            highlightedBy.Client.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "You lack the permission", false);

            Logger.WithTopics(
                      Topics.Entities.MailBox,
                      Topics.Actions.Update,
                      Topics.Entities.Mail,
                      Topics.Actions.Highlight,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(highlightedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to highlight {@PostSubject} in {@MailboxOwnerName}'s mailbox without permission",
                      highlightedBy.Name,
                      post.Subject,
                      Key);

            return;
        }

        //highlight post
        post = post with
        {
            IsHighlighted = true
        };
        Posts[postId] = post;
        highlightedBy.Client.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "Message highlighted", true);

        Logger.WithTopics(
                  Topics.Entities.MailBox,
                  Topics.Actions.Update,
                  Topics.Entities.Mail,
                  Topics.Actions.Highlight)
              .WithProperty(highlightedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} highlighted {@PostSubject} in {@MailboxOwnerName}'s mailbox",
                  highlightedBy.Name,
                  post.Subject,
                  Key);
    }

    /// <inheritdoc />
    public override void Post(
        Aisling addedBy,
        string author,
        string subject,
        string message,
        bool highlighted = false)
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

        addedBy.Client.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "Message sent", true);

        Logger.WithTopics(
                  Topics.Entities.MailBox,
                  Topics.Actions.Update,
                  Topics.Entities.Mail,
                  Topics.Actions.Add)
              .WithProperty(addedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} posted {@PostSubject} in {@MailboxOwnerName}'s mailbox",
                  addedBy.Name,
                  post.Subject,
                  Key);
    }

    /// <inheritdoc />
    public override void Show(Aisling aisling, short startPostId = short.MaxValue)
    {
        using var @lock = Sync.Enter();

        if (!ShouldShowTo(aisling.Id))
            return;

        var now = DateTime.UtcNow;

        LastShown.AddOrUpdate(aisling.Id, now, (_, _) => now);

        if (!aisling.IsAdmin && !Key.EqualsI(aisling.Name))
        {
            Logger.WithTopics(
                      Topics.Entities.MailBox,
                      Topics.Entities.Mail,
                      Topics.Actions.Read,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(aisling)
                  .WithProperty(this)
                  .LogWarning("{@AislingName} attempted to view {@MailboxOwnerName}'s mailbox without permission", aisling.Name, Key);

            return;
        }

        aisling.Client.SendDisplayBoard(this, startPostId);
    }

    /// <inheritdoc />
    public override void ShowPost(Aisling aisling, short postId, BoardControls control)
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

        if (!aisling.IsAdmin && !Key.EqualsI(aisling.Name))
        {
            Logger.WithTopics(
                      Topics.Entities.MailBox,
                      Topics.Entities.Mail,
                      Topics.Actions.Read,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(aisling)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to view {@PostSubject} in {@MailboxOwnerName}'s mailbox without permission",
                      aisling.Name,
                      post.Subject,
                      Key);

            return;
        }

        //mark as read
        UnHighlight(aisling, ref post);
        aisling.Client.SendPost(post, true, Posts.Keys.Max() == postIdActual);
    }

    /// <inheritdoc />
    public override void UnHighlight(Aisling unhighlightedBy, ref Post post)
    {
        using var @lock = Sync.Enter();

        if (!post.IsHighlighted)
            return;

        post = post with
        {
            IsHighlighted = false
        };

        Posts[post.PostId] = post;

        //if no unread mail, remove blinking mail icon
        if (!Posts.Values.Any(i => i.IsHighlighted))
            unhighlightedBy.Client.SendAttributes(StatUpdateType.Secondary);

        Logger.WithTopics(
                  Topics.Entities.MailBox,
                  Topics.Actions.Update,
                  Topics.Entities.Mail,
                  Topics.Actions.Highlight)
              .WithProperty(unhighlightedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} unhighlighted {@PostSubject} in {@MailboxOwnerName}'s mailbox",
                  unhighlightedBy.Name,
                  post.Subject,
                  Key);
    }
}