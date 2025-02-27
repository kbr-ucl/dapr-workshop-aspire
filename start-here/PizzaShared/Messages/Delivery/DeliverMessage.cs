namespace PizzaShared.Messages.Delivery;

public class DeliverMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string PizzaType { get; set; }
    public string Size { get; set; }
    public CustomerDto Customer { get; set; }
}

public class DeliverResultMessage : DeliverMessage
{
    public string Status { get; set; }
    public string? Error { get; set; }
}
