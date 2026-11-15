- Remove IMessageBus, not needed and complicates things.
- Merge Contexts or inherit from Infinity.Toolkit.Handlers.HandlerContext.
- Refactor so that you can register a consumer that listens for messages on multiple buses. Not just one bus. This will allow for more flexibility in the system.
  For example that a message that arrives on the inmemory bus can resolve the same message handler as if it arrived on Azure Service Bus.

- 