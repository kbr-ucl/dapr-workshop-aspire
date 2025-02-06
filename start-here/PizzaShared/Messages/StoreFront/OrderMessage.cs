namespace PizzaShared.Messages.StoreFront;

public class OrderMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string PizzaType { get; set; }
    public string Size { get; set; }
    public Customer Customer { get; set; }
}

public class OrderResultMessage : WorkflowMessage
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public string? Error { get; set; }
}

public class Customer
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
}