using Microsoft.Extensions.Logging;

namespace Infinity.Toolkit.Messaging.Tests.Utils;

internal class TUnitLogger(LoggerExternalScopeProvider scopeProvider, string categoryName) : ILogger
{
    private readonly LoggerExternalScopeProvider scopeProvider = scopeProvider;
    private readonly string categoryName = categoryName;

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => scopeProvider.Push(state);

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var outputWriter = TestContext.Current?.OutputWriter;
        var message = $"{categoryName}: {formatter(state, exception)}";
        if (outputWriter is not null)
        {
            outputWriter.WriteLine(message);
        }
        else
        {
            Console.WriteLine(message);
        }
    }
}

internal sealed class TUnitLogger<T>(LoggerExternalScopeProvider scopeProvider) : TUnitLogger(scopeProvider, typeof(T).Name), ILogger<T>
{
}
