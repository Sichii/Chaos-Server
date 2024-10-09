using System.Diagnostics;
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Board;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.BulletinBoardScripts.Abstractions;

namespace Chaos.Collections;

public sealed class BulletinBoard : BoardBase, IScripted<IBulletinBoardScript>
{
    private readonly ILogger<BulletinBoard> Logger;

    /// <inheritdoc />
    public IBulletinBoardScript Script { get; }

    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    public BulletinBoardTemplate Template { get; }

    /// <inheritdoc />
    public BulletinBoard(
        BulletinBoardTemplate template,
        ILogger<BulletinBoard> logger,
        IScriptProvider scriptProvider,
        IEnumerable<Post> posts)
        : base(
            template.Id,
            template.Name,
            template.TemplateKey,
            posts)
    {
        Template = template;
        Logger = logger;
        ScriptKeys = Template.ScriptKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        Script = scriptProvider.CreateScript<IBulletinBoardScript, BulletinBoard>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public override bool Delete(Aisling deletedBy, short postId)
    {
        using var @lock = Sync.Enter();

        //check that post exists
        if (!Posts.TryGetValue(postId, out var post))
        {
            deletedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such post", false);

            return false;
        }

        if (!deletedBy.IsAdmin && !Script.AllowedToDelete(deletedBy, post))
        {
            deletedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "You lack the permission", false);

            Logger.WithTopics(
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Update,
                      Topics.Entities.Post,
                      Topics.Actions.Delete,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(deletedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to delete {@PostSubject} from board {@BoardName} without permission",
                      deletedBy.Name,
                      post.Subject,
                      Name);

            return false;
        }

        //remove post from mailbox
        if (!Posts.TryRemove(postId, out _))
            throw new UnreachableException("We already checked that the post exists");

        deletedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "Post deleted", true);

        Logger.WithTopics(
                  Topics.Entities.BulletinBoard,
                  Topics.Actions.Update,
                  Topics.Entities.Post,
                  Topics.Actions.Delete)
              .WithProperty(deletedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} deleted {@PostSubject} from board {@BoardName}",
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
            highlightedBy.Client.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such post", false);

            return;
        }

        //only admins and the owner can highlight posts
        if (!highlightedBy.IsAdmin && !Script.AllowedToHighlight(highlightedBy))
        {
            highlightedBy.Client.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "You lack the permission", false);

            Logger.WithTopics(
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Update,
                      Topics.Entities.Post,
                      Topics.Actions.Highlight,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(highlightedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to highlight {@PostSubject} on board {@BoardName} without permission",
                      highlightedBy.Name,
                      post.Subject,
                      Name);

            return;
        }

        //highlight post
        post = post with
        {
            IsHighlighted = true
        };
        Posts[postId] = post;
        highlightedBy.Client.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "Post highlighted", true);

        Logger.WithTopics(
                  Topics.Entities.BulletinBoard,
                  Topics.Actions.Update,
                  Topics.Entities.Post,
                  Topics.Actions.Highlight)
              .WithProperty(highlightedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} highlighted {@PostSubject} on board {@BoardName}",
                  highlightedBy.Name,
                  post.Subject,
                  Name);
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

        if (!addedBy.IsAdmin && !Script.AllowedToPost(addedBy))
        {
            addedBy.Client.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "You lack the permission", false);

            Logger.WithTopics(
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Update,
                      Topics.Entities.Post,
                      Topics.Actions.Add,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(addedBy)
                  .WithProperty(this)
                  .LogWarning("{@AislingName} attempted to post on board {@BoardName} without permission", addedBy.Name, Name);

            return;
        }

        //create post
        var post = new Post(
            PostIdGenerator.NextId,
            author,
            subject,
            message,
            DateTime.UtcNow,
            highlighted);

        if (!addedBy.IsAdmin && Script.ShouldRejectPost(addedBy, post, out var reason))
        {
            addedBy.Client.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, reason, false);

            Logger.WithTopics(
                      Topics.Entities.BulletinBoard,
                      Topics.Actions.Update,
                      Topics.Entities.Post,
                      Topics.Actions.Add,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(addedBy)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to post on board {@BoardName} but was rejected: {@Reason}",
                      addedBy.Name,
                      Name,
                      reason);

            return;
        }

        //add post to mailbox
        if (!Posts.TryAdd(post.PostId, post))
            throw new UnreachableException(
                "Failed to add post to board. This should be impossible due to the lock and threadsafe postId generator");

        addedBy.Client.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "Message posted", true);

        Logger.WithTopics(
                  Topics.Entities.BulletinBoard,
                  Topics.Actions.Update,
                  Topics.Entities.Post,
                  Topics.Actions.Add)
              .WithProperty(addedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} posted {@PostSubject} on board {@BoardName}",
                  addedBy.Name,
                  post.Subject,
                  Name);
    }

    /// <inheritdoc />
    public override void Show(Aisling aisling, short startPostId = short.MaxValue)
    {
        using var @lock = Sync.Enter();

        if (!ShouldShowTo(aisling.Id))
            return;

        var now = DateTime.UtcNow;

        LastShown.AddOrUpdate(aisling.Id, now, (_, _) => now);

        if (!aisling.IsAdmin && !Script.AllowedToView(aisling))
        {
            Logger.WithTopics(
                      Topics.Entities.BulletinBoard,
                      Topics.Entities.Post,
                      Topics.Actions.Read,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(aisling)
                  .WithProperty(this)
                  .LogWarning("{@AislingName} attempted to view board {@BoardName} without permission", aisling.Name, Name);

            return;
        }

        if (Script.AllowedToHighlight(aisling))
            aisling.Client.SendAttributes(StatUpdateType.GameMasterB);

        aisling.Client.SendDisplayBoard(this, startPostId);

        //if (Script.AllowedToHighlight(aisling))
        //  aisling.Client.SendAttributes(StatUpdateType.None);
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

        if (!aisling.IsAdmin && !Script.AllowedToView(aisling))
        {
            Logger.WithTopics(
                      Topics.Entities.BulletinBoard,
                      Topics.Entities.Post,
                      Topics.Actions.Read,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(aisling)
                  .WithProperty(this)
                  .WithProperty(post)
                  .LogWarning(
                      "{@AislingName} attempted to view post {@PostSubject} on board {@BoardName} without permission",
                      aisling.Name,
                      post.Subject,
                      Name);

            return;
        }

        aisling.Client.SendPost(post, true, Posts.Keys.Max() > postIdActual);
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

        Logger.WithTopics(
                  Topics.Entities.BulletinBoard,
                  Topics.Actions.Update,
                  Topics.Entities.Post,
                  Topics.Actions.Highlight)
              .WithProperty(unhighlightedBy)
              .WithProperty(this)
              .WithProperty(post)
              .LogInformation(
                  "{@AislingName} unhighlighted {@PostSubject} on board {@BoardName}",
                  unhighlightedBy.Name,
                  post.Subject,
                  Name);
    }
}