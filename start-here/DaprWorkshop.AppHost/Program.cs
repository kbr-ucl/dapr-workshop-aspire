using CommunityToolkit.Aspire.Hosting.Dapr;
using System.Collections.Immutable;

var builder = DistributedApplication.CreateBuilder(args);

//var statestore = builder.AddDaprStateStore("pizzastatestore");
//var pubsubComponent = builder.AddDaprPubSub("pizzapubsub");

builder.AddProject<Projects.PizzaOrder>("pizzaorderservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizzaorder",
        DaprHttpPort = 3501,
        ResourcesPaths = ImmutableHashSet.Create("../resources")
    });

builder.AddProject<Projects.PizzaKitchen>("pizzakitchenservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-kitchen",
        DaprHttpPort = 3503,
        ResourcesPaths = ImmutableHashSet.Create("../resources")
    });

builder.AddProject<Projects.PizzaStorefront>("pizzastorefrontservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-storefront",
        DaprHttpPort = 3502,
        ResourcesPaths = ImmutableHashSet.Create("../resources")
    });

builder.AddProject<Projects.PizzaDelivery>("pizzadeliveryservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "pizza-delivery",
        DaprHttpPort = 3504,
        ResourcesPaths = ImmutableHashSet.Create("../resources")
    });

builder.Build().Run();