var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PizzaStorefront>("pizzastorefront");

builder.AddProject<Projects.PizzaKitchen>("pizzakitchen");

builder.AddProject<Projects.PizzaDelivery>("pizzadelivery");

builder.AddProject<Projects.PizzaOrder>("pizzaorder");

builder.Build().Run();
