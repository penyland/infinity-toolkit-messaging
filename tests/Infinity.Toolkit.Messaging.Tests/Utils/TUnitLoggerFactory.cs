using Microsoft.Extensions.Logging;

namespace Infinity.Toolkit.Messaging.Tests.Utils;

public sealed class TUnitLoggerFactory : ILoggerFactory
{
    private readonly LoggerExternalScopeProvider scopeProvider = new();

    public void AddProvider(ILoggerProvider provider)
    {
        throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TUnitLogger(scopeProvider, categoryName);
    }

    public void Dispose()
    {
    }

    public static ILogger<T> CreateLogger<T>()
    {
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(new TUnitLoggerProvider());
        return loggerFactory.CreateLogger<T>();
    }
}
