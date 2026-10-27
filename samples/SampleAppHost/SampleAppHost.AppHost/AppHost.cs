using System.Text.Json.Nodes;

var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("AzureServiceBus")
    .RunAsEmulator(emulator =>
    {
        emulator.WithLifetime(ContainerLifetime.Persistent);
        emulator.WithConfiguration((JsonNode config) =>
        {
            var userConfig = config["userConfig"] ??= new JsonObject();
        });
    });

var topic1 = serviceBus
    .AddServiceBusTopic("toolkit-development")
        .AddServiceBusSubscription("subscription1");

var topic2 = serviceBus
    .AddServiceBusTopic("toolkit-development2")
        .AddServiceBusSubscription("subscription2");

builder.AddProject<Projects.AzureServiceBusMessagingSample>("azureServiceBusSample")
    .WithReference(serviceBus, "AzureServiceBus").WaitFor(serviceBus);

builder.Build().Run();
