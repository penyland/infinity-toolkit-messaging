using Microsoft.Extensions.Logging;

namespace Infinity.Toolkit.Messaging.Tests.Utils;

public sealed class TUnitLoggerProvider : ILoggerProvider
{
    private readonly LoggerExternalScopeProvider scopeProvider = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new TUnitLogger(scopeProvider, categoryName);
    }

    public void Dispose()
    {
    }
}
