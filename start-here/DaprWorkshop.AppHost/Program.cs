var builder = DistributedApplication.CreateBuilder(args);

var statestore = builder.AddDaprStateStore("pizzastatestore");

builder.AddProject<Projects.PizzaStorefront>("pizzastorefront");

builder.AddProject<Projects.PizzaDelivery>("pizzadelivery");

builder.AddProject<Projects.PizzaKitchen>("pizzakitchen");

builder.AddProject<Projects.PizzaOrder>("pizzaorder");

builder.AddProject<Projects.PizzaWorkflow>("pizzaworkflow");

builder.Build().Run();
