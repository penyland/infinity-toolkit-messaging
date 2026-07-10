using Infinity.Toolkit.Messaging.Tests.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infinity.Toolkit.Messaging.Tests;

public class TestBase
{
    public static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddTUnit());
        services.AddMetrics();
        services.AddSingleton(CreateConfiguration([]));

        configure(services);
        return services.BuildServiceProvider();
    }

    public static IConfiguration CreateConfiguration(Dictionary<string, string?> configurationValues) => new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();
}

public class TestOptionsMonitor<T>(T options) : IOptionsMonitor<T>
    where T : class
{
    private readonly T options = options;

    public T CurrentValue => options;

    public T Get(string? name) => options;

    public IDisposable OnChange(Action<T, string> listener) => throw new NotImplementedException();
}
