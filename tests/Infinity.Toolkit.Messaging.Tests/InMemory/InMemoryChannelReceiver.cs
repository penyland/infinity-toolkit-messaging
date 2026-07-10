using Infinity.Toolkit.Messaging.InMemory;

namespace Infinity.Toolkit.Messaging.Tests.InMemory;

public class InMemoryChannelReceiverTests : TestBase
{
    public class DeferMessageAsync
    {
        [Test]
        public async Task Should_SucceedAsync()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());
            var testMessage = new InMemoryMessage("testMessage")
            {
                SequenceNumber = 1
            };

            // Act
            var sequenceNumber = await receiver.DeferMessageAsync(testMessage, cancellationToken: CancellationToken.None);

            // Assert
            sequenceNumber.ShouldBe(1);
        }

        [Test]
        public async Task Then_ReceiveDeferred_Should_Succeed()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());
            var testMessage = new InMemoryMessage("testMessage")
            {
                SequenceNumber = 1
            };

            // Act
            var sequenceNumber = await receiver.DeferMessageAsync(testMessage, cancellationToken: CancellationToken.None);
            var message = await receiver.ReceiveDeferredMessageAsync(sequenceNumber);

            // Assert
            sequenceNumber.ShouldBe(1);
            message.ShouldNotBeNull();
            message.SequenceNumber.ShouldBe(1);
            message.Body.ToString().ShouldBe("testMessage");
        }

        [Test]
        public async Task Same_Message_MultipleTimes_Should_Fail()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());
            var testMessage = new InMemoryMessage("testMessage")
            {
                SequenceNumber = 1
            };

            // Act
            await receiver.DeferMessageAsync(testMessage, cancellationToken: CancellationToken.None);
            var action2 = () => receiver.DeferMessageAsync(testMessage, cancellationToken: CancellationToken.None);

            // Assert
            await action2.ShouldThrowAsync<InvalidOperationException>();
        }

        [Test]
        public async Task Multiple_Messages_Should_Succeed()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());

            // Act
            var sequenceNumber1 = await receiver.DeferMessageAsync(new InMemoryMessage("testMessage1") { SequenceNumber = 1 }, cancellationToken: CancellationToken.None);
            var sequenceNumber2 = await receiver.DeferMessageAsync(new InMemoryMessage("testMessage2") { SequenceNumber = 2 }, cancellationToken: CancellationToken.None);

            // Assert
            sequenceNumber1.ShouldBe(1);
            sequenceNumber2.ShouldBe(2);
        }

        [Test]
        public async Task Multiple_Messages_Multiple_Times_Should_Fail()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());

            // Act
            var sequenceNumber1 = await receiver.DeferMessageAsync(new InMemoryMessage("testMessage1") { SequenceNumber = 1 }, cancellationToken: CancellationToken.None);
            var sequenceNumber2 = await receiver.DeferMessageAsync(new InMemoryMessage("testMessage1") { SequenceNumber = 2 }, cancellationToken: CancellationToken.None);
            var action = () => receiver.DeferMessageAsync(new InMemoryMessage("testMessage2") { SequenceNumber = 2 }, cancellationToken: CancellationToken.None);

            // Assert
            sequenceNumber1.ShouldBe(1);
            sequenceNumber2.ShouldBe(2);
            await action.ShouldThrowAsync<InvalidOperationException>();
        }
    }

    public class ReceiveDeferredMessageAsync
    {
        [Test]
        public async Task Should_Succeed()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());
            var testMessage = new InMemoryMessage("testMessage")
            {
                SequenceNumber = 1
            };

            // Act
            var sequenceNumber = await receiver.DeferMessageAsync(testMessage, cancellationToken: CancellationToken.None);
            var message = await receiver.ReceiveDeferredMessageAsync(sequenceNumber);

            // Assert
            message.ShouldNotBeNull();
            message.SequenceNumber.ShouldBe(1);
            message.Body.ToString().ShouldBe("testMessage");
        }

        [Test]
        public async Task MultipleTimes_Should_Fail()
        {
            // Arrange
            var receiver = new InMemoryChannelReceiver("testChannel", new TUnitLoggerFactory());
            var testMessage = new InMemoryMessage("testMessage")
            {
                SequenceNumber = 1
            };

            // Act
            var sequenceNumber = await receiver.DeferMessageAsync(testMessage, cancellationToken: CancellationToken.None);
            await receiver.ReceiveDeferredMessageAsync(sequenceNumber);
            var action = () => receiver.ReceiveDeferredMessageAsync(sequenceNumber);

            // Assert
            await action.ShouldThrowAsync<InvalidOperationException>();
        }
    }
}
