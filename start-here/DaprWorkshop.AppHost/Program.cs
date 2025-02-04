using Aspire.Hosting.Dapr;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var statestore = builder.AddDaprStateStore("pizzastatestore");

builder.AddProject<PizzaOrder>("pizzaorderservice")
    .WithDaprSidecar(new DaprSidecarOptions
        {
            AppId = "pizza-order",
            DaprHttpPort = 3501
        })
    .WithReference(statestore);



builder.Build().Run();
