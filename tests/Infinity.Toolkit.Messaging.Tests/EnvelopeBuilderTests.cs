using System.Text.Json;

namespace Infinity.Toolkit.Messaging.Tests;

public class EnvelopeBuilderTests
{
    public class Build
    {
        public Build()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "TEST");
        }

        [Test]
        public void Should_Return_An_Envelope()
        {
            // Arrange
            // Act
            var envelope = new EnvelopeBuilder().Build();

            // Assert
            envelope.ShouldNotBeNull();
        }

        [Test]
        public void With_Invalid_EventType_Should_Fail()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var func = () => new EnvelopeBuilder().WithEventType(default!).WithBody(message).Build();

            // Assert
            func.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WithBody_Should_Contain_Expected_Value()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var envelope = new EnvelopeBuilder().WithBody(message).Build();

            // Assert
            envelope.Body.ShouldNotBeNull();
            envelope.Body.ToObjectFromJson<TestMessage>().ShouldBeOfType<TestMessage>();

            var content = envelope.Body.ToObjectFromJson<TestMessage>()?.Content;

            content.ShouldNotBeNull();
            content.ShouldBe("testContent");
        }

        [Test]
        public void Envelope_Should_Contain_Expected_CloudEvent_Property_SpecVersion()
        {
            // Arrange
            // Act
            var envelope = new EnvelopeBuilder().Build();

            // Assert
            envelope.ApplicationProperties[CloudEvents.SpecVersion].ShouldBe(CloudEvents.CloudEventsSpecVersion);
        }

        [Test]
        public void Message_Should_Contain_Expected_CloudEvent_Property_Type()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var envelope = new EnvelopeBuilder()
                .WithBody(message)
                .WithEventType<TestMessage>()
                .Build();

            var type = envelope.ApplicationProperties[CloudEvents.Type];

            // Assert
            type.ShouldNotBeNull();
        }

        [Test]
        public void Message_Should_Contain_Expected_CloudEvent_Property_CorrelationId()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var envelope = new EnvelopeBuilder()
                .WithBody(message)
                .WithCorrelationId("correlationId")
                .Build();

            // Assert
            envelope.CorrelationId.ShouldNotBeNullOrEmpty();
            envelope.CorrelationId.ShouldBe("correlationId");
        }

        [Test]
        public void Message_Should_Contain_Expected_CloudEvent_Property_Id()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var envelope = new EnvelopeBuilder()
                .WithBody(message)
                .Build();

            // Assert
            envelope.MessageId.ShouldNotBeNullOrEmpty();
            Guid.TryParse(envelope.MessageId, out var guid).ShouldBeTrue();
        }

        [Test]
        public void Message_Should_Contain_Expected_CloudEvent_Property_Source()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var envelope = new EnvelopeBuilder()
                .WithBody(message)
                .WithSource(new("test://test"))
                .Build();

            // Assert
            envelope.ApplicationProperties[CloudEvents.Source].ShouldNotBeNull();

            var sourceString = envelope.ApplicationProperties[CloudEvents.Source] as string;
            var source = new Uri(sourceString!);
            source.ShouldNotBeNull();
            source?.Scheme.ShouldBe("test");
            source?.Host.ShouldBe("test");
        }

        [Test]
        public void With_PropertyBag_Should_Contain_Expected_Values()
        {
            // Arrange
            var message = new TestMessage("testContent");

            var propertyBag = new Dictionary<string, object?>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            var envelope = new EnvelopeBuilder().WithBody(message).WithHeaders(propertyBag).Build();

            // Assert
            envelope.ApplicationProperties.ShouldNotBeNull();
            envelope.ApplicationProperties.ShouldContainKeyAndValue("key1", "value1");
            envelope.ApplicationProperties.ShouldContainKeyAndValue("key2", "value2");
        }

        [Test]
        public void MessageBody_Should_Contain_PascalCased_Property_Name()
        {
            // Arrange
            var message = new TestMessage("testContent");

            // Act
            var envelope = new EnvelopeBuilder().WithBody(message).Build();

            // Assert
            envelope.Body.ShouldNotBeNull();
            envelope.Body.ToString().Contains("Content");
        }

        [Test]
        public void MessageBody_Should_Contain_CamelCased_Property_Name()
        {
            // Arrange
            var message = new TestMessage("testContent");
            var jsonSerializerOptions = new JsonSerializerOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Act
            var envelope = new EnvelopeBuilder().WithBody(message, jsonSerializerOptions).Build();

            // Assert
            envelope.Body.ShouldNotBeNull();
            envelope.Body.ToString().Contains("content");
        }

        [Test]
        public void MessageBody_Should_Contain_CamelCased_Property_Name_And_Expected_CloudEvent_Property_CorrelationId()
        {
            // Arrange
            var message = new TestMessage("testContent");
            var jsonSerializerOptions = new JsonSerializerOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Act
            var envelope = new EnvelopeBuilder().WithBody(message, jsonSerializerOptions).WithCorrelationId("correlationId").Build();

            // Assert
            envelope.Body.ShouldNotBeNull();
            envelope.Body.ToString().Contains("content");
            envelope.CorrelationId.ShouldNotBeNullOrEmpty();
            envelope.CorrelationId.ShouldBe("correlationId");
        }

        [Test]
        public void MessageBody_Should_Contain_CamelCased_Property_Name_And_PropertyBag_Contains_Expected_Values()
        {
            // Arrange
            var message = new TestMessage("testContent");
            var jsonSerializerOptions = new JsonSerializerOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var propertyBag = new Dictionary<string, object?>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            var envelope = new EnvelopeBuilder().WithBody(message, jsonSerializerOptions).WithHeaders(propertyBag).Build();

            // Assert
            envelope.Body.ShouldNotBeNull();
            envelope.Body.ToString().Contains("content");
            envelope.ApplicationProperties.ShouldNotBeNull();
            envelope.ApplicationProperties.ShouldContainKeyAndValue("key1", "value1");
            envelope.ApplicationProperties.ShouldContainKeyAndValue("key2", "value2");
        }
    }
}
