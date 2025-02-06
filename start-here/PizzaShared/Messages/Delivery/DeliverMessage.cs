namespace PizzaShared.Messages.Delivery;

public class DeliverMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string PizzaType { get; set; }
    public string Size { get; set; }
    public Customer Customer { get; set; }
}

public class DeliverResultMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public string? Error { get; set; }
}