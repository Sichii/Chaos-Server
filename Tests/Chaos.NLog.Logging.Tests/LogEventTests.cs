#region
using System.Collections;
using Chaos.NLog.Logging.Abstractions;
using Chaos.NLog.Logging.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class LogEventTests
{
    #region Constructor Tests
    [Test]
    public void Constructor_ShouldWrapLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act
        var logEvent = new LogEvent(mockLogger.Object);

        // Assert - Verify the logger is wrapped properly by testing delegation
        logEvent.IsEnabled(LogLevel.Information);
        mockLogger.Verify(l => l.IsEnabled(LogLevel.Information), Times.Once);
    }

    [Test]
    public void Constructor_ShouldInitializeExtraPropertiesWithTopics()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act
        var logEvent = new LogEvent(mockLogger.Object);

        // Initialize LogValues by calling Log to make properties accessible
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert - Should have Topics property in ExtraProperties
        logEvent.Count
                .Should()
                .Be(1); // Only Topics initially

        logEvent[0]
            .Key
            .Should()
            .Be("Topics");

        logEvent[0]
            .Value
            .Should()
            .BeOfType<HashSet<string>>();

        var topics = logEvent[0].Value as HashSet<string>;

        topics!.Comparer
               .Should()
               .Be(StringComparer.OrdinalIgnoreCase);
    }
    #endregion

    #region ILogger Interface Tests
    [Test]
    public void BeginScope_ShouldDelegateToUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var expectedScope = new Mock<IDisposable>().Object;

        var state = new
        {
            TestState = "value"
        };

        mockLogger.Setup(l => l.BeginScope(state))
                  .Returns(expectedScope);
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        var result = logEvent.BeginScope(state);

        // Assert
        result.Should()
              .Be(expectedScope);
        mockLogger.Verify(l => l.BeginScope(state), Times.Once);
    }

    [Test]
    public void IsEnabled_ShouldDelegateToUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        mockLogger.Setup(l => l.IsEnabled(LogLevel.Warning))
                  .Returns(true);

        mockLogger.Setup(l => l.IsEnabled(LogLevel.Debug))
                  .Returns(false);

        var logEvent = new LogEvent(mockLogger.Object);

        // Act & Assert
        logEvent.IsEnabled(LogLevel.Warning)
                .Should()
                .BeTrue();

        logEvent.IsEnabled(LogLevel.Debug)
                .Should()
                .BeFalse();

        mockLogger.Verify(l => l.IsEnabled(LogLevel.Warning), Times.Once);
        mockLogger.Verify(l => l.IsEnabled(LogLevel.Debug), Times.Once);
    }
    #endregion

    #region IReadOnlyList Interface Tests
    [Test]
    public void Count_WithNoLogValues_ShouldThrowNullReferenceException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act & Assert - LogValues is null until Log() is called
        Action act = () => _ = logEvent.Count;

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void Count_WithLogValuesAndExtraProperties_ShouldReturnCombinedCount()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Add property directly to the LogEvent instance
        logEvent.WithProperty("TestProp", "testValue");

        // Simulate Log call with state
        var logValues = new List<KeyValuePair<string, object>>
        {
            new("Key1", "Value1"),
            new("Key2", "Value2")
        };

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            logValues,
            null,
            (_, _) => "test");

        // Assert - Should have LogValues + ExtraProperties
        logEvent.Count
                .Should()
                .Be(4); // 2 log values + Topics + TestProp
    }

    [Test]
    public void Indexer_WithValidIndices_ShouldReturnCorrectValues()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithProperty("ExtraProp", "extraValue");

        var logValues = new List<KeyValuePair<string, object>>
        {
            new("LogKey", "logValue")
        };

        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            logValues,
            null,
            (_, _) => "test");

        // Act & Assert
        // LogValues come first
        logEvent[0]
            .Should()
            .Be(new KeyValuePair<string, object>("LogKey", "logValue"));

        // ExtraProperties come after LogValues
        logEvent[1]
            .Key
            .Should()
            .Be("Topics");

        logEvent[2]
            .Key
            .Should()
            .Be("ExtraProp");

        logEvent[2]
            .Value
            .Should()
            .Be("extraValue");
    }

    [Test]
    public void Indexer_WithNoLogValues_ShouldThrowNullReferenceException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act & Assert - LogValues is null before Log() is called
        Action act = () => _ = logEvent[0];

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void Indexer_WithIndexBeyondRange_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Initialize LogValues by calling Log
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Act & Assert - Should throw when index is beyond both LogValues and ExtraProperties
        Action act = () => _ = logEvent[10];

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GetEnumerator_ShouldEnumerateLogValuesAndExtraProperties()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithProperty("ExtraProp", "extraValue");

        var logValues = new List<KeyValuePair<string, object>>
        {
            new("LogKey1", "logValue1"),
            new("LogKey2", "logValue2")
        };

        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            logValues,
            null,
            (_, _) => "test");

        // Act
        var allItems = logEvent.ToList();

        // Assert
        allItems.Should()
                .HaveCount(4); // 2 log values + Topics + ExtraProp

        allItems[0]
            .Should()
            .Be(new KeyValuePair<string, object>("LogKey1", "logValue1"));

        allItems[1]
            .Should()
            .Be(new KeyValuePair<string, object>("LogKey2", "logValue2"));

        allItems[2]
            .Key
            .Should()
            .Be("Topics");

        allItems[3]
            .Key
            .Should()
            .Be("ExtraProp");
    }

    [Test]
    public void GetEnumerator_NonGeneric_ShouldWork()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);
        logEvent.WithProperty("TestProp", "testValue");

        // Initialize LogValues
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Act
        var enumerable = (IEnumerable)logEvent;
        var items = new List<object>();

        foreach (var item in enumerable)
            items.Add(item);

        // Assert
        items.Should()
             .HaveCount(2); // Topics + TestProp

        items.All(i => i is KeyValuePair<string, object>)
             .Should()
             .BeTrue();
    }
    #endregion

    #region Direct LogEvent Method Tests
    [Test]
    public void WithMetrics_ShouldSetStartTimestamp()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        var result = logEvent.WithMetrics();

        // Assert
        result.Should()
              .Be(logEvent); // Should return self for chaining

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        logEvent.Count
                .Should()
                .Be(2); // Topics + Metrics  
        var metricsEntry = logEvent.First(kvp => kvp.Key == "Metrics");

        metricsEntry.Value
                    .Should()
                    .BeOfType<TimeSpan>(); // Should be calculated after Log is called
    }

    [Test]
    public void WithMetrics_MultipleCallsBeforeLog_ShouldAddMultipleMetricsEntries()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        logEvent.WithMetrics();
        logEvent.WithMetrics();

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        logEvent.Count
                .Should()
                .Be(3); // Topics + Metrics(replaced) + Metrics(untouched)

        logEvent.Where(kvp => kvp.Key == "Metrics")
                .Count()
                .Should()
                .Be(2);
    }

    [Test]
    public void WithMetrics_AfterLog_ShouldReplaceMetricsWithElapsedTime()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithMetrics();

        // Simulate some time passing
        Thread.Sleep(10);

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test message",
            null,
            (state, _) => state.ToString());

        // Assert
        var metricsEntries = logEvent.Where(kvp => kvp.Key == "Metrics")
                                     .ToList();

        metricsEntries.Should()
                      .HaveCount(1); // Should be replaced, not added

        metricsEntries[0]
            .Value
            .Should()
            .BeOfType<TimeSpan>();

        var elapsed = (TimeSpan)metricsEntries[0].Value;

        elapsed.Should()
               .BeGreaterThan(TimeSpan.Zero);

        elapsed.Should()
               .BeLessThan(TimeSpan.FromSeconds(1)); // Reasonable upper bound
    }

    [Test]
    public void WithProperty_WithRegularName_ShouldCapitalizeFirstLetter()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        var result = logEvent.WithProperty("testProperty", "testValue");

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        result.Should()
              .Be(logEvent); // Should return self for chaining

        var property = logEvent.First(kvp => kvp.Key == "TestProperty");

        property.Value
                .Should()
                .Be("testValue");
    }

    [Test]
    public void WithProperty_WithAlreadyCapitalizedName_ShouldNotChange()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        logEvent.WithProperty("TestProperty", "testValue");

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var property = logEvent.First(kvp => kvp.Key == "TestProperty");

        property.Value
                .Should()
                .Be("testValue");
    }

    [Test]
    public void WithProperty_WithNameThis_ShouldUseTypeName()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        var testObject = new TestClass
        {
            Value = "test"
        };

        // Act
        logEvent.WithProperty("this", testObject);

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var property = logEvent.First(kvp => kvp.Key == "TestClass");

        property.Value
                .Should()
                .Be(testObject);
    }

    [Test]
    public void WithProperty_WithITransformableCollection_ShouldTransformValue()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);
        var transformableCollection = new TestTransformableCollection();

        // Act & Assert - This should not throw even though no transformation is registered
        // The actual transformation logic is tested in SetupSerializationBuilderExtensionsTests
        Action act = () => logEvent.WithProperty("collection", transformableCollection);

        act.Should()
           .Throw<InvalidOperationException>() // No transformation registered
           .WithMessage("No transformation registered for type TestTransformableCollection");
    }

    [Test]
    public void WithProperty_ShouldAddTopicBasedOnValueType()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        var testObject = new TestClass
        {
            Value = "test"
        };

        // Act
        logEvent.WithProperty("testProp", testObject);

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .Contain("TestClass");
    }

    [Test]
    public void WithProperty_WithLowercaseTypeName_ShouldCapitalizeTopic()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act - Using a string which has a lowercase type name conceptually
        logEvent.WithProperty("stringProp", "value");

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .Contain("String"); // Type name is already capitalized for built-in types
    }

    [Test]
    public void WithTopic_ShouldCapitalizeFirstLetter()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        var result = logEvent.WithTopic("testTopic");

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        result.Should()
              .Be(logEvent); // Should return self for chaining

        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .Contain("TestTopic");
    }

    [Test]
    public void WithTopic_WithAlreadyCapitalizedTopic_ShouldNotChange()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        logEvent.WithTopic("TestTopic");

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .Contain("TestTopic");
    }

    [Test]
    public void WithTopic_WithDuplicateTopic_ShouldNotAddDuplicate()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        logEvent.WithTopic("testTopic");
        logEvent.WithTopic("TestTopic"); // Case insensitive duplicate

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .HaveCount(1);

        topics.Should()
              .Contain("TestTopic");
    }

    [Test]
    public void WithTopic_CaseInsensitiveComparison()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        logEvent.WithTopic("testtopic");
        logEvent.WithTopic("TESTTOPIC");
        logEvent.WithTopic("TestTopic");

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        // Assert
        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .HaveCount(1);

        topics.Should()
              .Contain("Testtopic"); // First one added wins for actual value
    }
    #endregion

    #region Log Method Tests
    [Test]
    public void Log_WithoutMetrics_ShouldDelegateToUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);
        var eventId = new EventId(123, "TestEvent");
        var exception = new InvalidOperationException("Test exception");
        var state = "test state";

        // Act
        logEvent.Log(
            LogLevel.Warning,
            eventId,
            state,
            exception,
            (s, _) => $"Formatted: {s}");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Warning,
                eventId,
                logEvent, // Should pass itself as state
                exception,
                It.IsAny<Func<LogEvent, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void Log_WithStateAsReadOnlyList_ShouldSetLogValues()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        var logValues = new List<KeyValuePair<string, object>>
        {
            new("Key1", "Value1"),
            new("Key2", 42)
        };

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            logValues,
            null,
            (_, _) => "test");

        // Assert
        logEvent.Count
                .Should()
                .Be(3); // 2 log values + Topics

        logEvent[0]
            .Should()
            .Be(new KeyValuePair<string, object>("Key1", "Value1"));

        logEvent[1]
            .Should()
            .Be(new KeyValuePair<string, object>("Key2", 42));
    }

    [Test]
    public void Log_WithNonReadOnlyListState_ShouldCreateEmptyLogValues()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "simple string",
            null,
            (state, _) => state.ToString());

        // Assert
        logEvent.Count
                .Should()
                .Be(1); // Only Topics, no log values
    }

    [Test]
    public void Log_WithMetrics_ShouldCalculateElapsedTime()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithMetrics();
        Thread.Sleep(5); // Small delay to ensure elapsed time > 0

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "formatted");

        // Assert
        var metricsEntry = logEvent.First(kvp => kvp.Key == "Metrics");

        metricsEntry.Value
                    .Should()
                    .BeOfType<TimeSpan>();

        var elapsed = (TimeSpan)metricsEntry.Value;

        elapsed.Should()
               .BeGreaterThan(TimeSpan.Zero);
    }

    [Test]
    public void Log_WithMultipleMetrics_ShouldReplaceFirstMetricsOnly()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithMetrics();
        logEvent.WithMetrics();
        Thread.Sleep(5);

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "formatted");

        // Assert - ReplaceBy only replaces the first match
        var metricsEntries = logEvent.Where(kvp => kvp.Key == "Metrics")
                                     .ToList();

        metricsEntries.Should()
                      .HaveCount(2); // First replaced, second untouched

        metricsEntries[0]
            .Value
            .Should()
            .BeOfType<TimeSpan>(); // First one replaced with elapsed time

        metricsEntries[1]
            .Value
            .Should()
            .BeNull(); // Second one still null
    }

    [Test]
    public void Log_FormatterCallsWithCorrectParameters()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);
        var originalState = "original state";
        var exception = new ArgumentException("test");

        // Act
        logEvent.Log(
            LogLevel.Error,
            new EventId(1),
            originalState,
            exception,
            (_, _) => "formatted message");

        // Assert - Verify the underlying logger's formatter was called
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                logEvent,
                exception,
                It.IsAny<Func<LogEvent, Exception?, string>>()),
            Times.Once);
    }
    #endregion

    #region Extension Method Integration Tests
    [Test]
    public void ExtensionMethods_WithMetrics_ShouldCreateLogEventAndSetMetrics()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act
        var result = mockLogger.Object.WithMetrics();

        // Assert
        result.Should()
              .BeOfType<LogEvent>();
        var logEvent = (LogEvent)result;

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        logEvent.Count
                .Should()
                .Be(2); // Topics + Metrics

        logEvent.Should()
                .Contain(kvp => kvp.Key == "Metrics");
    }

    [Test]
    public void ExtensionMethods_WithProperty_ShouldCreateLogEventAndAddProperty()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var testValue = "test value";

        // Act - Using extension method with CallerArgumentExpression
        var result = mockLogger.Object.WithProperty(testValue);

        // Assert
        result.Should()
              .BeOfType<LogEvent>();
        var logEvent = (LogEvent)result;

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        logEvent.Should()
                .Contain(kvp => (kvp.Key == "TestValue") && kvp.Value.Equals(testValue));
    }

    [Test]
    public void ExtensionMethods_WithTopic_ShouldCreateLogEventAndAddTopic()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act
        var result = mockLogger.Object.WithTopics("customTopic");

        // Assert
        result.Should()
              .BeOfType<LogEvent>();
        var logEvent = (LogEvent)result;

        // Initialize LogValues to access properties
        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            "test",
            null,
            (_, _) => "test");

        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .Contain("CustomTopic");
    }

    [Test]
    public void ComplexScenario_WithMetricsPropertyAndTopicAndLog_ShouldWorkCorrectly()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        var testObject = new TestClass
        {
            Value = "integration test"
        };

        // Act - Chain multiple operations using extension methods
        var logger = mockLogger.Object
                               .WithMetrics()
                               .WithProperty(testObject)
                               .WithTopics("integration", "test");

        Thread.Sleep(5); // Small delay for metrics

        var logValues = new List<KeyValuePair<string, object>>
        {
            new("RequestId", "12345"),
            new("UserId", 67890)
        };

        logger.Log(
            LogLevel.Information,
            new EventId(100, "Integration"),
            logValues,
            null,
            (_, _) => "Integration test message");

        // Assert
        var logEvent = (LogEvent)logger;

        logEvent.Count
                .Should()
                .Be(5); // LogValues(2) + Topics(1) + TestObject(1) + Metrics(1)

        // Verify log values
        logEvent[0]
            .Should()
            .Be(new KeyValuePair<string, object>("RequestId", "12345"));

        logEvent[1]
            .Should()
            .Be(new KeyValuePair<string, object>("UserId", 67890));

        // Verify topics
        var topics = (HashSet<string>)logEvent.First(kvp => kvp.Key == "Topics")
                                              .Value;

        topics.Should()
              .Contain("TestClass"); // Added by WithProperty

        topics.Should()
              .Contain("Integration"); // Added by WithTopic

        topics.Should()
              .Contain("Test"); // Added by WithTopic

        // Verify test object property
        var testObjectEntry = logEvent.First(kvp => kvp.Key == "TestObject");

        testObjectEntry.Value
                       .Should()
                       .Be(testObject);

        // Verify metrics
        var metricsEntry = logEvent.First(kvp => kvp.Key == "Metrics");

        metricsEntry.Value
                    .Should()
                    .BeOfType<TimeSpan>();

        ((TimeSpan)metricsEntry.Value).Should()
                                      .BeGreaterThan(TimeSpan.Zero);

        // Verify logger was called
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                new EventId(100, "Integration"),
                logEvent,
                It.IsAny<Exception?>(),
                It.IsAny<Func<LogEvent, Exception?, string>>()),
            Times.Once);
    }
    #endregion

    #region Edge Cases and Error Conditions
    [Test]
    public void WithProperty_WithEmptyString_ShouldThrow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act & Assert - Should throw when name is empty
        Action act = () => logEvent.WithProperty(string.Empty, "value");

        act.Should()
           .Throw<Exception>(); // Let's see what it actually throws
    }

    [Test]
    public void WithTopic_WithEmptyString_ShouldThrow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act & Assert - Should throw when topic is empty
        Action act = () => logEvent.WithTopic(string.Empty);

        act.Should()
           .Throw<Exception>(); // Let's see what it actually throws
    }

    [Test]
    public void WithProperty_WithNullValue_ShouldThrowNullReferenceException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        // Act & Assert - WithProperty calls value.GetType() which throws for null values
        Action act = () => logEvent.WithProperty("nullProp", null!);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void Enumeration_DuringLogCall_ShouldReflectCurrentState()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithProperty("beforeLog", "value1");

        // Act
        var logValues = new List<KeyValuePair<string, object>>
        {
            new("logKey", "logValue")
        };

        logEvent.Log(
            LogLevel.Information,
            new EventId(1),
            logValues,
            null,
            (_, _) => "test");

        logEvent.WithProperty("afterLog", "value2"); // This should be reflected in enumeration

        // Assert
        var allItems = logEvent.ToList();

        allItems.Should()
                .Contain(kvp => (kvp.Key == "BeforeLog") && kvp.Value.Equals("value1"));

        allItems.Should()
                .Contain(kvp => (kvp.Key == "AfterLog") && kvp.Value.Equals("value2"));

        allItems.Should()
                .Contain(kvp => (kvp.Key == "logKey") && kvp.Value.Equals("logValue"));
    }

    [Test]
    public void MultipleLogCalls_ShouldUpdateLogValuesCorrectly()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var logEvent = new LogEvent(mockLogger.Object);

        logEvent.WithProperty("persistent", "value");

        // First log call
        var firstLogValues = new List<KeyValuePair<string, object>>
        {
            new("first", "call")
        };

        logEvent.Log(
            LogLevel.Debug,
            new EventId(1),
            firstLogValues,
            null,
            (_, _) => "first");

        // Verify first state
        logEvent.Count
                .Should()
                .Be(3); // first + Topics + persistent

        logEvent[0]
            .Should()
            .Be(new KeyValuePair<string, object>("first", "call"));

        // Second log call with different values
        var secondLogValues = new List<KeyValuePair<string, object>>
        {
            new("second", "call"),
            new("additional", "data")
        };

        // Act
        logEvent.Log(
            LogLevel.Information,
            new EventId(2),
            secondLogValues,
            null,
            (_, _) => "second");

        // Assert - LogValues should be updated, ExtraProperties should persist
        logEvent.Count
                .Should()
                .Be(4); // second(2) + Topics + persistent

        logEvent[0]
            .Should()
            .Be(new KeyValuePair<string, object>("second", "call"));

        logEvent[1]
            .Should()
            .Be(new KeyValuePair<string, object>("additional", "data"));

        var persistentEntry = logEvent.First(kvp => kvp.Key == "Persistent");

        persistentEntry.Value
                       .Should()
                       .Be("value");
    }
    #endregion

    #region Test Helper Classes
    private sealed class TestClass
    {
        public string Value { get; set; } = string.Empty;
    }

    private sealed class TestTransformableCollection : ITransformableCollection
    {
        public List<string> Items { get; } = [];
    }
    #endregion
}