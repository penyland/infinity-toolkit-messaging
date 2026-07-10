namespace Infinity.Toolkit.Messaging.Tests;

public class MessageBusBuilderTests : TestBase
{
    [Test]
    public void MapMessageHandler_Should_Succeed()
    {
        // Arrange
        var services = new ServiceCollection();
        var messageBusBuilder = new MessageBusBuilder(services);

        // Act
        var result = messageBusBuilder.MapMessageHandler<TestMessage, TestMessageHandler>();

        // Assert
        result.ShouldNotBeNull();
    }

    [Test]
    public void AddBroker_Should_Succeed()
    {
        // Arrange
        var services = new ServiceCollection();
        var messageBusBuilder = new MessageBusBuilder(services);

        // Act
        var result = messageBusBuilder.AddBroker<TestBroker, DefaultMessageBrokerOptions>("brokerType", options => { });

        // Assert
        result.ShouldNotBeNull();
    }
}

internal class DefaultMessageBrokerOptions : MessageBusBrokerOptions
{
}
