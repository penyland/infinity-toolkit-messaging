using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Infinity.Toolkit.Messaging.Tests.Utils;

public static class TUnitLoggerProviderExtensions
{
    public static ILoggingBuilder AddTUnit(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TUnitLoggerProvider>());
        return builder;
    }
}
