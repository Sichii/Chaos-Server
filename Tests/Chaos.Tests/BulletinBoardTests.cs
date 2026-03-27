#region
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Board;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.BulletinBoardScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class BulletinBoardTests
{
    private readonly Mock<ILogger<BulletinBoard>> LoggerMock;
    private readonly MapInstance Map;
    private readonly Mock<IBulletinBoardScript> ScriptMock;
    private readonly Mock<IScriptProvider> ScriptProviderMock;

    public BulletinBoardTests()
    {
        Map = MockMapInstance.Create();
        LoggerMock = MockLogger.Create<BulletinBoard>();
        ScriptMock = new Mock<IBulletinBoardScript>();

        // Default: allow everything
        ScriptMock.Setup(s => s.AllowedToDelete(It.IsAny<Aisling>(), It.IsAny<Post>()))
                  .Returns(true);

        ScriptMock.Setup(s => s.AllowedToHighlight(It.IsAny<Aisling>()))
                  .Returns(true);

        ScriptMock.Setup(s => s.AllowedToPost(It.IsAny<Aisling>()))
                  .Returns(true);

        ScriptMock.Setup(s => s.AllowedToView(It.IsAny<Aisling>()))
                  .Returns(true);

        ScriptProviderMock = new Mock<IScriptProvider>();

        ScriptProviderMock.Setup(sp => sp.CreateScript<IBulletinBoardScript, BulletinBoard>(
                              It.IsAny<ICollection<string>>(),
                              It.IsAny<BulletinBoard>()))
                          .Returns(ScriptMock.Object);
    }

    private BulletinBoard CreateBoard(BulletinBoardTemplate? template = null, IEnumerable<Post>? posts = null)
        => new(
            template ?? CreateTemplate(),
            LoggerMock.Object,
            ScriptProviderMock.Object,
            posts ?? []);

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

    private static BulletinBoardTemplate CreateTemplate(ushort id = 1, string name = "TestBoard", string templateKey = "testBoard")
        => new()
        {
            Id = id,
            Name = name,
            TemplateKey = templateKey,
            ScriptKeys = new List<string>
            {
                "TestScript"
            },
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

    #region Constructor
    [Test]
    public void Constructor_ShouldSetPropertiesFromTemplate()
    {
        var template = CreateTemplate(42, "Announcements", "announcements");

        var board = CreateBoard(template);

        board.BoardId
             .Should()
             .Be(42);

        board.Name
             .Should()
             .Be("Announcements");

        board.Key
             .Should()
             .Be("announcements");

        board.Template
             .Should()
             .BeSameAs(template);
    }

    [Test]
    public void Constructor_ShouldSetScriptKeysFromTemplate()
    {
        var board = CreateBoard();

        board.ScriptKeys
             .Should()
             .Contain("TestScript");
    }

    [Test]
    public void Constructor_ShouldRenumberExistingPosts()
    {
        var posts = new[]
        {
            new Post(
                99,
                "A",
                "Oldest",
                "msg1",
                DateTime.UtcNow.AddMinutes(-2),
                false),
            new Post(
                50,
                "B",
                "Middle",
                "msg2",
                DateTime.UtcNow.AddMinutes(-1),
                false),
            new Post(
                75,
                "C",
                "Newest",
                "msg3",
                DateTime.UtcNow,
                false)
        };

        var board = CreateBoard(posts: posts);

        board.TryGet(1, out var post1)
             .Should()
             .BeTrue();

        post1!.Subject
              .Should()
              .Be("Oldest");

        board.TryGet(2, out var post2)
             .Should()
             .BeTrue();

        post2!.Subject
              .Should()
              .Be("Middle");

        board.TryGet(3, out var post3)
             .Should()
             .BeTrue();

        post3!.Subject
              .Should()
              .Be("Newest");
    }
    #endregion

    #region Delete
    [Test]
    public void Delete_ShouldReturnFalse_WhenPostDoesNotExist()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        var result = board.Delete(aisling, 99);

        result.Should()
              .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such post", false));
    }

    [Test]
    public void Delete_ShouldReturnFalse_WhenNonAdminAndScriptDenies()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToDelete(aisling, It.IsAny<Post>()))
                  .Returns(false);

        var result = board.Delete(aisling, 1);

        result.Should()
              .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "You lack the permission", false));
    }

    [Test]
    public void Delete_ShouldSucceed_WhenScriptAllows()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map);

        var result = board.Delete(aisling, 1);

        result.Should()
              .BeTrue();

        board.Contains(1)
             .Should()
             .BeFalse();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "Post deleted", true));
    }

    [Test]
    public void Delete_ShouldSucceed_WhenAdminEvenIfScriptDenies()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);

        ScriptMock.Setup(s => s.AllowedToDelete(aisling, It.IsAny<Post>()))
                  .Returns(false);

        var result = board.Delete(aisling, 1);

        result.Should()
              .BeTrue();

        board.Contains(1)
             .Should()
             .BeFalse();
    }
    #endregion

    #region Highlight
    [Test]
    public void Highlight_ShouldSendError_WhenPostDoesNotExist()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        board.Highlight(aisling, 99);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.DeletePostResponse, "No such post", false));
    }

    [Test]
    public void Highlight_ShouldDeny_WhenNonAdminAndScriptDenies()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToHighlight(aisling))
                  .Returns(false);

        board.Highlight(aisling, 1);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "You lack the permission", false));
    }

    [Test]
    public void Highlight_ShouldSucceed_WhenScriptAllows()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map);

        board.Highlight(aisling, 1);

        board.TryGet(1, out var post)
             .Should()
             .BeTrue();

        post!.IsHighlighted
             .Should()
             .BeTrue();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.HighlightPostResponse, "Post highlighted", true));
    }

    [Test]
    public void Highlight_ShouldSucceed_WhenAdminEvenIfScriptDenies()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);

        ScriptMock.Setup(s => s.AllowedToHighlight(aisling))
                  .Returns(false);

        board.Highlight(aisling, 1);

        board.TryGet(1, out var post)
             .Should()
             .BeTrue();

        post!.IsHighlighted
             .Should()
             .BeTrue();
    }
    #endregion

    #region Post
    [Test]
    public void Post_ShouldDeny_WhenNonAdminAndScriptDenies()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToPost(aisling))
                  .Returns(false);

        board.Post(
            aisling,
            "Author",
            "Subject",
            "Message");

        board.Should()
             .BeEmpty();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "You lack the permission", false));
    }

    [Test]
    public void Post_ShouldSucceed_WhenScriptAllows()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        board.Post(
            aisling,
            "Author",
            "Subject",
            "Message");

        board.Should()
             .HaveCount(1);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "Message posted", true));
    }

    [Test]
    public void Post_ShouldReject_WhenScriptRejectsPost()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);
        var reason = "Post contains banned words";

        ScriptMock.Setup(s => s.ShouldRejectPost(aisling, It.IsAny<Post>(), out reason))
                  .Returns(true);

        board.Post(
            aisling,
            "Author",
            "Subject",
            "Message");

        board.Should()
             .BeEmpty();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, reason, false));
    }

    [Test]
    public void Post_ShouldSucceed_WhenAdminEvenIfScriptDeniesAndRejects()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);
        var reason = "Rejected";

        ScriptMock.Setup(s => s.AllowedToPost(aisling))
                  .Returns(false);

        ScriptMock.Setup(s => s.ShouldRejectPost(aisling, It.IsAny<Post>(), out reason))
                  .Returns(true);

        board.Post(
            aisling,
            "Author",
            "Subject",
            "Message");

        board.Should()
             .HaveCount(1);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendBoardResponse(BoardOrResponseType.SubmitPostResponse, "Message posted", true));
    }

    [Test]
    public void Post_ShouldAutoGenerateSequentialIds()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        board.Post(
            aisling,
            "A",
            "Sub1",
            "Msg1");

        board.Post(
            aisling,
            "B",
            "Sub2",
            "Msg2");

        board.TryGet(1, out var p1)
             .Should()
             .BeTrue();

        p1!.Subject
           .Should()
           .Be("Sub1");

        board.TryGet(2, out var p2)
             .Should()
             .BeTrue();

        p2!.Subject
           .Should()
           .Be("Sub2");
    }
    #endregion

    #region Show
    [Test]
    public void Show_ShouldSendDisplayBoard_WhenAllowed()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        board.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(board, short.MaxValue));
    }

    [Test]
    public void Show_ShouldDeny_WhenNonAdminAndScriptDeniesView()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToView(aisling))
                  .Returns(false);

        board.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(It.IsAny<BoardBase>(), It.IsAny<short>()), Times.Never);
    }

    [Test]
    public void Show_ShouldSendGameMasterBAttribute_WhenAllowedToHighlight()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToHighlight(aisling))
                  .Returns(true);

        board.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendAttributes(StatUpdateType.GameMasterB));
    }

    [Test]
    public void Show_ShouldNotSendGameMasterBAttribute_WhenNotAllowedToHighlight()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToHighlight(aisling))
                  .Returns(false);

        board.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendAttributes(StatUpdateType.GameMasterB), Times.Never);
    }

    [Test]
    public void Show_ShouldRateLimit()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        board.Show(aisling);
        board.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(board, short.MaxValue), Times.Once);
    }

    [Test]
    public void Show_ShouldSucceed_WhenAdmin()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);

        ScriptMock.Setup(s => s.AllowedToView(aisling))
                  .Returns(false);

        board.Show(aisling);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendDisplayBoard(board, short.MaxValue));
    }
    #endregion

    #region ShowPost
    [Test]
    public void ShowPost_RequestPost_ShouldSendPost()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map);

        board.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_ShouldReturnSilently_WhenPostNotFound()
    {
        var board = CreateBoard();
        var aisling = MockAisling.Create(Map);

        board.ShowPost(aisling, 99, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void ShowPost_ShouldDeny_WhenNonAdminAndScriptDeniesView()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map);

        ScriptMock.Setup(s => s.AllowedToView(aisling))
                  .Returns(false);

        board.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Test]
    public void ShowPost_ShouldSucceed_WhenAdmin()
    {
        var board = CreateBoard(posts: [CreatePost()]);
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);

        ScriptMock.Setup(s => s.AllowedToView(aisling))
                  .Returns(false);

        board.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_PreviousPage_ShouldNavigateCorrectly()
    {
        var posts = new[]
        {
            CreatePost(subject: "Post1"),
            CreatePost(2, subject: "Post2"),
            CreatePost(3, subject: "Post3")
        };
        var board = CreateBoard(posts: posts);
        var aisling = MockAisling.Create(Map);

        board.ShowPost(aisling, 3, BoardControls.PreviousPage);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_NextPage_ShouldNavigateCorrectly()
    {
        var posts = new[]
        {
            CreatePost(subject: "Post1"),
            CreatePost(2, subject: "Post2"),
            CreatePost(3, subject: "Post3")
        };
        var board = CreateBoard(posts: posts);
        var aisling = MockAisling.Create(Map);

        board.ShowPost(aisling, 1, BoardControls.NextPage);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, It.IsAny<bool>()));
    }

    [Test]
    public void ShowPost_ShouldSetEnablePrevBtn_WhenMorePostsExist()
    {
        var posts = new[]
        {
            CreatePost(subject: "Post1"),
            CreatePost(2, subject: "Post2")
        };
        var board = CreateBoard(posts: posts);
        var aisling = MockAisling.Create(Map);

        // Viewing post 1, max is 2, so enablePrevBtn = true (2 > 1)
        board.ShowPost(aisling, 1, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, true));
    }

    [Test]
    public void ShowPost_ShouldNotSetEnablePrevBtn_WhenViewingMaxPost()
    {
        var posts = new[]
        {
            CreatePost(subject: "Post1"),
            CreatePost(2, subject: "Post2")
        };
        var board = CreateBoard(posts: posts);
        var aisling = MockAisling.Create(Map);

        // Viewing post 2 (max), enablePrevBtn = false (2 > 2 is false)
        board.ShowPost(aisling, 2, BoardControls.RequestPost);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendPost(It.IsAny<Post>(), true, false));
    }
    #endregion

    #region UnHighlight
    [Test]
    public void UnHighlight_ShouldReturnEarly_WhenNotHighlighted()
    {
        var board = CreateBoard(posts: [CreatePost(isHighlighted: false)]);
        var aisling = MockAisling.Create(Map);

        board.TryGet(1, out var post)
             .Should()
             .BeTrue();

        board.UnHighlight(aisling, ref post!);

        post.IsHighlighted
            .Should()
            .BeFalse();
    }

    [Test]
    public void UnHighlight_ShouldUnhighlightPost()
    {
        var board = CreateBoard(posts: [CreatePost(isHighlighted: true)]);
        var aisling = MockAisling.Create(Map);

        board.TryGet(1, out var post)
             .Should()
             .BeTrue();

        board.UnHighlight(aisling, ref post!);

        post.IsHighlighted
            .Should()
            .BeFalse();

        board.TryGet(1, out var stored)
             .Should()
             .BeTrue();

        stored!.IsHighlighted
               .Should()
               .BeFalse();
    }

    [Test]
    public void UnHighlight_ShouldNotSendSecondaryAttribute()
    {
        // BulletinBoard.UnHighlight does NOT send Secondary (unlike MailBox)
        var board = CreateBoard(posts: [CreatePost(isHighlighted: true)]);
        var aisling = MockAisling.Create(Map);

        board.TryGet(1, out var post)
             .Should()
             .BeTrue();

        board.UnHighlight(aisling, ref post!);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendAttributes(StatUpdateType.Secondary), Times.Never);
    }
    #endregion
}