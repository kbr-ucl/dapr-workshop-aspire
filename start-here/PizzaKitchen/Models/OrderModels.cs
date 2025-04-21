using System.Text.Json.Serialization;

namespace PizzaKitchen.Models;

public class Order
{
    public required string OrderId { get; set; }
    public required string PizzaType { get; set; }
    public required string Size { get; set; }
    public required Customer Customer { get; set; }
    public string Status { get; set; } = "created";
    public string? Error { get; set; }
}

public class Customer
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
}

public class ValidationRequest
{
    public required string OrderId { get; set; }
    public required bool Approved { get; set; }
}