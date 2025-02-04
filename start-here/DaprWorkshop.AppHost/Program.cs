var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PizzaOrder>("pizzaorder");

builder.Build().Run();
