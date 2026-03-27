#region
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Board;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class MailBoxTests
{
    private readonly Mock<ILogger<MailBox>> LoggerMock = MockLogger.Create<MailBox>();
    private readonly MapInstance Map = MockMapInstance.Create();

    private MailBox CreateMailBox(string ownerName = "TestOwner", IEnumerable<Post>? posts = null)
        => new(ownerName, LoggerMock.Object, posts);

    private static Post CreatePost(
        short postId = 1,
        string author = "Author",
        string subject = "Subject",
        string message = "Message",
        bool isHighlighted = false)
        => new(
            postId,
            author,
            subject,
            message,
            DateTime.UtcNow,
            isHighlighted);

    #region GetEnumerator
    [Test]
    public void GetEnumerator_ShouldEnumerateAllPosts()
    {
        var posts = new[]
        {
            CreatePost(subject: "A"),
            CreatePost(2, subject: "B")
        };

        var mailBox = CreateMailBox(posts: posts);

        mailBox.Should()
               .HaveCount(2);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldSetBoardIdToZero()
    {
        var mailBox = CreateMailBox("Alice");

        mailBox.BoardId
               .Should()
               .Be(0);
    }

    [Test]
    public void Constructor_ShouldSetNameToMail()
    {
        var mailBox = CreateMailBox("Alice");

        mailBox.Name
               .Should()
               .Be("Mail");
    }

    [Test]
    public void Constructor_ShouldSetKeyToOwnerName()
    {
        var mailBox = CreateMailBox("Alice");

        mailBox.Key
               .Should()
               .Be("Alice");
    }

    [Test]
    public void Constructor_ShouldRenumberExistingPostsSequentially()
    {
        var posts = new[]
        {
            new Post(
                99,
                "A",
                "First",
                "msg1",
                DateTime.UtcNow.AddMinutes(-2),
                false),
            new Post(
                50,
                "B",
                "Second",
                "msg2",
                DateTime.UtcNow.AddMinutes(-1),
                false),
            new Post(
                75,
                "C",
                "Third",
                "msg3",
                DateTime.UtcNow,
                false)
        };

        var mailBox = CreateMailBox(posts: posts);

        // Posts should be renumbered 1, 2, 3 ordered by CreationDate
        mailBox.TryGet(1, out var post1)
               .Should()
               .BeTrue();

        post1!.Subject
              .Should()
              .Be("First");

        mailBox.TryGet(2, out var post2)
               .Should()
               .BeTrue();

        post2!.Subject
              .Should()
              .Be("Second");

        mailBox.TryGet(3, out var post3)
               .Should()
               .BeTrue();

        post3!.Subject
              .Should()
              .Be("Third");
    }

    [Test]
    public void Constructor_ShouldHandleNullPosts()
    {
        var mailBox = CreateMailBox();

        mailBox.Should()
               .BeEmpty();
    }
    #endregion

    #region Contains
    [Test]
    public void Contains_ById_ShouldReturnTrue_WhenPostExists()
    {
        var mailBox = CreateMailBox(posts: [CreatePost()]);

        mailBox.Contains(1)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Contains_ById_ShouldReturnFalse_WhenPostDoesNotExist()
    {
        var mailBox = CreateMailBox();

        mailBox.Contains(99)
               .Should()
               .BeFalse();
    }

    [Test]
    public void Contains_ByPost_ShouldReturnTrue_WhenPostExists()
    {
        var post = CreatePost();
        var mailBox = CreateMailBox(posts: [post]);

        // The post is renumbered to 1, so we need to check with the renumbered ID
        mailBox.Contains(
                   post with
                   {
                       PostId = 1
                   })
               .Should()
               .BeTrue();
    }

    [Test]
    public void Contains_ByPost_ShouldReturnFalse_WhenPostDoesNotExist()
    {
        var mailBox = CreateMailBox();

        mailBox.Contains(CreatePost(99))
               .Should()
               .BeFalse();
    }
    #endregion

    #region TryGet
    [Test]
    public void TryGet_ShouldReturnTrue_WhenPostExists()
    {
        var mailBox = CreateMailBox(posts: [CreatePost()]);

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        post.Should()
            .NotBeNull();
    }

    [Test]
    public void TryGet_ShouldReturnFalse_WhenPostDoesNotExist()
    {
        var mailBox = CreateMailBox();

        mailBox.TryGet(99, out _)
               .Should()
               .BeFalse();
    }
    #endregion

    #region Delete
    [Test]
    public void Delete_ShouldReturnFalse_WhenPostDoesNotExist()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "Owner");

        var result = mailBox.Delete(aisling, 99);

        result.Should()
              .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such message", false));
    }

    [Test]
    public void Delete_ShouldReturnFalse_WhenNonOwnerNonAdmin()
    {
        var mailBox = CreateMailBox("Alice", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "Bob");

        var result = mailBox.Delete(aisling, 1);

        result.Should()
              .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "You lack the permission", false));
    }

    [Test]
    public void Delete_ShouldSucceed_WhenOwner()
    {
        var mailBox = CreateMailBox("TestOwner", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        var result = mailBox.Delete(aisling, 1);

        result.Should()
              .BeTrue();

        mailBox.Contains(1)
               .Should()
               .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "Message deleted", true));
    }

    [Test]
    public void Delete_ShouldSucceed_WhenAdmin()
    {
        var mailBox = CreateMailBox("Alice", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "Admin", setup: a => a.IsAdmin = true);

        var result = mailBox.Delete(aisling, 1);

        result.Should()
              .BeTrue();

        mailBox.Contains(1)
               .Should()
               .BeFalse();
    }
    #endregion

    #region Highlight
    [Test]
    public void Highlight_ShouldSendError_WhenPostDoesNotExist()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "Owner");

        mailBox.Highlight(aisling, 99);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such message", false));
    }

    [Test]
    public void Highlight_ShouldDeny_WhenNonOwnerNonAdmin()
    {
        var mailBox = CreateMailBox("Alice", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "Bob");

        mailBox.Highlight(aisling, 1);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "You lack the permission", false));
    }

    [Test]
    public void Highlight_ShouldSucceed_WhenOwner()
    {
        var mailBox = CreateMailBox("TestOwner", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.Highlight(aisling, 1);

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        post!.IsHighlighted
             .Should()
             .BeTrue();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "Message highlighted", true));
    }

    [Test]
    public void Highlight_ShouldSucceed_WhenAdmin()
    {
        var mailBox = CreateMailBox("Alice", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "Admin", setup: a => a.IsAdmin = true);

        mailBox.Highlight(aisling, 1);

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        post!.IsHighlighted
             .Should()
             .BeTrue();
    }
    #endregion

    #region Post
    [Test]
    public void Post_ShouldAlwaysSucceed_NoPermissionChecks()
    {
        var mailBox = CreateMailBox("Alice");
        var aisling = MockAisling.Create(Map, "Bob");

        mailBox.Post(
            aisling,
            "Bob",
            "Hello",
            "Hi Alice!");

        mailBox.Should()
               .HaveCount(1);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "Message sent", true));
    }

    [Test]
    public void Post_ShouldAutoGeneratePostId()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "Owner");

        mailBox.Post(
            aisling,
            "Author1",
            "Sub1",
            "Msg1");

        mailBox.Post(
            aisling,
            "Author2",
            "Sub2",
            "Msg2");

        mailBox.TryGet(1, out var post1)
               .Should()
               .BeTrue();

        post1!.Subject
              .Should()
              .Be("Sub1");

        mailBox.TryGet(2, out var post2)
               .Should()
               .BeTrue();

        post2!.Subject
              .Should()
              .Be("Sub2");
    }

    [Test]
    public void Post_ShouldPreserveHighlightFlag()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "Owner");

        mailBox.Post(
            aisling,
            "Author",
            "Subject",
            "Message",
            true);

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        post!.IsHighlighted
             .Should()
             .BeTrue();
    }
    #endregion

    #region Show
    [Test]
    public void Show_ShouldSendDisplayBoard_WhenOwner()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(mailBox, short.MaxValue));
    }

    [Test]
    public void Show_ShouldDeny_WhenNonOwnerNonAdmin()
    {
        var mailBox = CreateMailBox("Alice");
        var aisling = MockAisling.Create(Map, "Bob");

        mailBox.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(It.IsAny<BoardBase>(), It.IsAny<short>()), Times.Never);
    }

    [Test]
    public void Show_ShouldSucceed_WhenAdmin()
    {
        var mailBox = CreateMailBox("Alice");
        var aisling = MockAisling.Create(Map, "Admin", setup: a => a.IsAdmin = true);

        mailBox.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(mailBox, short.MaxValue));
    }

    [Test]
    public void Show_ShouldRateLimit()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "TestOwner");

        // First show should work
        mailBox.Show(aisling);

        // Immediate second show should be rate-limited
        mailBox.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(mailBox, short.MaxValue), Times.Once);
    }
    #endregion

    #region ShowPost
    [Test]
    public void ShowPost_RequestPost_ShouldSendPost()
    {
        var mailBox = CreateMailBox("TestOwner", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_ShouldReturnSilently_WhenPostNotFound()
    {
        var mailBox = CreateMailBox();
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.ShowPost(aisling, 99, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void ShowPost_ShouldDeny_WhenNonOwnerNonAdmin()
    {
        var mailBox = CreateMailBox("Alice", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "Bob");

        mailBox.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void ShowPost_ShouldSucceed_WhenAdmin()
    {
        var mailBox = CreateMailBox("Alice", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "Admin", setup: a => a.IsAdmin = true);

        mailBox.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_ShouldUnhighlightPost_WhenShowing()
    {
        var mailBox = CreateMailBox("TestOwner", [CreatePost(isHighlighted: true)]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.ShowPost(aisling, 1, BoardControls.RequestPost);

        // Post should be marked as read (unhighlighted)
        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        post!.IsHighlighted
             .Should()
             .BeFalse();
    }

    [Test]
    public void ShowPost_PreviousPage_ShouldNavigateToNextHighestPost()
    {
        var posts = new[]
        {
            CreatePost(subject: "Post1"),
            CreatePost(2, subject: "Post2"),
            CreatePost(3, subject: "Post3")
        };
        var mailBox = CreateMailBox("TestOwner", posts);
        var aisling = MockAisling.Create(Map, "TestOwner");

        // PreviousPage with postId 3 means client sent 3, we do postId-- = 2, then NextHighest(2)
        mailBox.ShowPost(aisling, 3, BoardControls.PreviousPage);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_NextPage_ShouldNavigateToNextLowestPost()
    {
        var posts = new[]
        {
            CreatePost(subject: "Post1"),
            CreatePost(2, subject: "Post2"),
            CreatePost(3, subject: "Post3")
        };
        var mailBox = CreateMailBox("TestOwner", posts);
        var aisling = MockAisling.Create(Map, "TestOwner");

        // NextPage with postId 1 means client sent 1, we do postId++ = 2, then NextLowest(2)
        mailBox.ShowPost(aisling, 1, BoardControls.NextPage);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_ShouldSetEnablePrevBtn_WhenMaxPostId()
    {
        // MailBox uses: Posts.Keys.Max() == postIdActual for enablePrevBtn
        // When viewing the max (newest) post, enablePrevBtn should be true
        var mailBox = CreateMailBox("TestOwner", [CreatePost()]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.ShowPost(aisling, 1, BoardControls.RequestPost);

        // Single post, max == postIdActual, so enablePrevBtn = true
        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, true));
    }
    #endregion

    #region UnHighlight
    [Test]
    public void UnHighlight_ShouldReturnEarly_WhenNotHighlighted()
    {
        var mailBox = CreateMailBox("TestOwner", [CreatePost(isHighlighted: false)]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        mailBox.UnHighlight(aisling, ref post!);

        post.IsHighlighted
            .Should()
            .BeFalse();

        // Should not send Secondary attribute when returning early
        Mock.Get(aisling.Client)
            .Verify(c => c.SendAttributes(It.IsAny<StatUpdateType>()), Times.Never);
    }

    [Test]
    public void UnHighlight_ShouldUnhighlightAndSendSecondary_WhenNoMoreUnread()
    {
        var mailBox = CreateMailBox("TestOwner", [CreatePost(isHighlighted: true)]);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        mailBox.UnHighlight(aisling, ref post!);

        post.IsHighlighted
            .Should()
            .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendAttributes(StatUpdateType.Secondary));
    }

    [Test]
    public void UnHighlight_ShouldUnhighlightButNotSendSecondary_WhenOtherUnreadExist()
    {
        var posts = new[]
        {
            CreatePost(subject: "Read", isHighlighted: true),
            CreatePost(2, subject: "Unread", isHighlighted: true)
        };
        var mailBox = CreateMailBox("TestOwner", posts);
        var aisling = MockAisling.Create(Map, "TestOwner");

        mailBox.TryGet(1, out var post)
               .Should()
               .BeTrue();

        mailBox.UnHighlight(aisling, ref post!);

        post.IsHighlighted
            .Should()
            .BeFalse();

        // Other post still highlighted, so don't send Secondary
        Mock.Get(aisling.Client)
            .Verify(c => c.SendAttributes(StatUpdateType.Secondary), Times.Never);
    }
    #endregion
}