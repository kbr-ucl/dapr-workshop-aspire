using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Microsoft.AspNetCore.Mvc;
using PizzaShared.Messages.Delivery;
using PizzaShared.Messages.Kitchen;
using PizzaShared.Messages.StoreFront;
using PizzaWorkflow.Models;
using PizzaWorkflow.Workflows;

namespace PizzaWorkflow.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly DaprWorkflowClient _daprWorkflowClient;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(DaprWorkflowClient daprWorkflowClient, ILogger<WorkflowController> logger)
    {
        _daprWorkflowClient = daprWorkflowClient;
        _logger = logger;
    }

    [HttpPost("start-order")]
    public async Task<IActionResult> StartOrder(Order order)
    {
        var instanceId = $"pizza-order-{order.OrderId}";
        
        try
        {
            _logger.LogInformation("Starting workflow for order {OrderId}", order.OrderId);

            //TODO: Start the PizzaOrderingWorkflow workflow
            await _daprWorkflowClient.ScheduleNewWorkflowAsync(
                name: nameof(PizzaOrderingWorkflow),
                instanceId: instanceId,
                input: order
  
            );

            _logger.LogInformation("Workflow started successfully for order {OrderId}", order.OrderId);

            return Ok(new
            {
                order_id = order.OrderId,
                workflow_instance_id = instanceId,
                status = "started"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start workflow for order {OrderId}", order.OrderId);
            throw;
        }
    }

    [HttpPost("validate-pizza")]
    public async Task<IActionResult> ValidatePizza(ValidationRequest validation)
    {
        var instanceId = $"pizza-order-{validation.OrderId}";
        
        try
        {
            _logger.LogInformation("Raising validation event for order {OrderId}. Approved: {Approved}", 
                validation.OrderId, validation.Approved);

            //TODO: Raise the ValidationComplete event
            await _daprWorkflowClient.RaiseEventAsync(
                instanceId: instanceId,
                eventName: "ValidationComplete",
                eventPayload: validation
            );

            _logger.LogInformation("Validation event raised successfully for order {OrderId}", 
                validation.OrderId);

            return Ok(new
            {
                order_id = validation.OrderId,
                validation_status = validation.Approved ? "approved" : "rejected"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to raise validation event for order {OrderId}", validation.OrderId);
            throw;
        }
    }

    [HttpPost("get-status")]
    public async Task<IActionResult> GetOrderStatus(ManageWorkflowRequest request)
    {
        var instanceId = $"pizza-order-{request.OrderId}";
        
        try
        {
            _logger.LogInformation("Getting workflow status for order {OrderId}", request.OrderId);

            //TODO: Get workflow status
            var status = await _daprWorkflowClient.GetWorkflowStateAsync(instanceId);

            _logger.LogInformation("Workflow status retrieved successfully for order {OrderId}", request.OrderId);

            return Ok(new
            {
                order_id = request.OrderId,
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get workflow status for order {OrderId}", request.OrderId);
            throw;
        }
    }

    [HttpPost("pause-order")]
    public async Task<IActionResult> PauseOrder(ManageWorkflowRequest request)
    {
        var instanceId = $"pizza-order-{request.OrderId}";
        
        try
        {
            _logger.LogInformation("Pausing workflow for order {OrderId}", request.OrderId);

            //TODO: Pause workflow
            await _daprWorkflowClient.SuspendWorkflowAsync(instanceId);

            _logger.LogInformation("Workflow paused successfully for order {OrderId}", request.OrderId);

            return Ok(new
            {
                order_id = request.OrderId,
                status = "paused"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pause workflow for order {OrderId}", request.OrderId);
            throw;
        }
    }

    [HttpPost("resume-order")]
    public async Task<IActionResult> ResumeOrder(ManageWorkflowRequest request)
    {
        var instanceId = $"pizza-order-{request.OrderId}";
        
        try
        {
            _logger.LogInformation("Resuming workflow for order {OrderId}", request.OrderId);

            //TODO: Resume workflow
            await _daprWorkflowClient.ResumeWorkflowAsync(instanceId);

            _logger.LogInformation("Workflow resumed successfully for order {OrderId}", request.OrderId);

            return Ok(new
            {
                order_id = request.OrderId,
                status = "resumed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resume workflow for order {OrderId}", request.OrderId);
            throw;
        }
    }

    [HttpPost("cancel-order")]
    public async Task<IActionResult> CancelOrder( ManageWorkflowRequest request)
    {
        var instanceId = $"pizza-order-{request.OrderId}";
        
        try
        {
            _logger.LogInformation("Cancelling workflow for order {OrderId}", request.OrderId);

            // TODO: Implement workflow termination call
            await _daprWorkflowClient.TerminateWorkflowAsync(instanceId);

            _logger.LogInformation("Workflow cancelled successfully for order {OrderId}", request.OrderId);

            return Ok(new
            {
                order_id = request.OrderId,
                status = "terminated"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel workflow for order {OrderId}", request.OrderId);
            throw;
        }
    }

    [HttpPost("order-status")]
    [Topic("pizzapubsub", "workflow-storefront")]
    public async Task<IActionResult> OrderStatus(OrderResultMessage message)
    {
        _logger.LogInformation($"Received workflow status for workflow {message.WorkflowId}");
        var order = MessageHelper.FillOrder(message);
        await RaiseEventAsync("OrderComplete", order, message.WorkflowId);
        return Ok(message);
    }

    [HttpPost("cook-status")]
    [Topic("pizzapubsub", "workflow-kitchen")]
    public async Task<IActionResult> CookStatus(CookResultMessage message)
    {
        _logger.LogInformation($"Received workflow status for workflow {message.WorkflowId}");
        var order = MessageHelper.FillOrder(message);
        await RaiseEventAsync("CookComplete", order, message.WorkflowId);
        return Ok(message);
    }

    [HttpPost("deliver-status")]
    [Topic("pizzapubsub", "workflow-delivery")]
    public async Task<IActionResult> DeliverStatus(DeliverResultMessage message)
    {
        _logger.LogInformation($"Received workflow status for workflow {message.WorkflowId}");
        var order = MessageHelper.FillOrder(message);
        await RaiseEventAsync("DeliverComplete", order, message.WorkflowId);
        return Ok(message);
    }

    private async Task RaiseEventAsync(string eventName, Order order, string workflowId)
    {
        await _daprWorkflowClient.RaiseEventAsync(
            instanceId: workflowId,
            eventName: eventName,
            eventPayload: order
        );

    }
}