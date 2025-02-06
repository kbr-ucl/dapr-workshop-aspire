namespace PizzaShared.Messages.Kitchen;

public class CookMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string PizzaType { get; set; }
    public string Size { get; set; }
    public Customer Customer { get; set; }
}

public class CookResultMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public string? Error { get; set; }
}