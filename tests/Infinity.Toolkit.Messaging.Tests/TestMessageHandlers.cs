using Infinity.Toolkit.Messaging.Abstractions;

namespace Infinity.Toolkit.Messaging.Tests;

public class TestMessageHandler(IServiceProvider serviceProvider) : IMessageHandler<TestMessage>
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public Task Handle(IMessageHandlerContext<TestMessage> message, CancellationToken cancellationToken)
    {
        var testMessageHandlerData = serviceProvider.GetService<TestMessageHandlerData>();

        if (testMessageHandlerData != null)
        {
            testMessageHandlerData.NumberOfTimesInvoked++;
            testMessageHandlerData.MessageHandlerInvoked = true;
            testMessageHandlerData.MessageProperties.TryAdd(typeof(TestMessage).FullName!, message.Message!.Content);
        }

        return Task.CompletedTask;
    }
}
