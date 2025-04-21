namespace PizzaShared.Messages.StoreFront;

public class OrderMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string PizzaType { get; set; }
    public string Size { get; set; }
    public CustomerDto Customer { get; set; }
}

public class OrderResultMessage : OrderMessage
{
    public string Status { get; set; }
    public string? Error { get; set; }
}
