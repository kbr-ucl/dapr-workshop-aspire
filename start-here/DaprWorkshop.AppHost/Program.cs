using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

var statestore = builder.AddDaprStateStore("pizzastatestore");
var pubsubComponent = builder.AddDaprPubSub("pizzapubsub");

builder.AddProject<Projects.PizzaOrder>("pizzaorderservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-order",
        DaprHttpPort = 3501
    })
    .WithReference(statestore)
    .WithReference(pubsubComponent);

builder.AddProject<Projects.PizzaKitchen>("pizzakitchenservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-kitchen",
        DaprHttpPort = 3503
    })
    .WithReference(pubsubComponent);

builder.AddProject<Projects.PizzaStorefront>("pizzastorefrontservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-storefront",
        DaprHttpPort = 3502
    })
    .WithReference(pubsubComponent);

builder.AddProject<Projects.PizzaDelivery>("pizzadeliveryservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-delivery",
        DaprHttpPort = 3504
    })
    .WithReference(pubsubComponent);

builder.Build().Run();