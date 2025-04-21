namespace PizzaOrder.Models;

public class CloudEvent<T>
{
    public string? Type { get; set; }
    public string? Source { get; set; }
    public T Data { get; set; } = default!;
}