using Dapr.Workflow;
using PizzaWorkflow.Activities;
using PizzaWorkflow.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().Dapr();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();