using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BeeSys.Utilities.Functions;

public class GetOrdersFunction
{
    private readonly ILogger<GetOrdersFunction> _logger;

    public GetOrdersFunction(ILogger<GetOrdersFunction> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// HTTP-triggered Azure Function that retrieves order data.
    /// Requires a function key passed via the <c>code</c> query parameter
    /// due to <c>AuthorizationLevel.Function</c>.
    /// </summary>
    /// <param name="req">Incoming HTTP request.</param>
    /// <param name="context">Function execution context.</param>
    /// <returns>HTTP response containing order data.</returns>
    [Function("GetOrders")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequest req)
    {
        _logger.LogInformation("Fetching orders list.");

        var orders = new List<OrderDto>
        {
            new("ORD-1001", 2500m, "Completed"),
            new("ORD-1002", 1800m, "Processing"),
            new("ORD-1003", 3200m, "Pending")
        };

        return new OkObjectResult(orders);
    }

    /// <summary>
    /// HTTP-triggered Azure Function that retrieves a single order by its identifier.
    /// Route: <c>GET /api/orders/{orderId}</c>
    /// Requires a function-level key passed via the <c>code</c> query parameter.
    /// </summary>
    /// <param name="req">The incoming <see cref="HttpRequest"/> containing route values and query parameters.</param>
    /// <returns>
    /// Returns <see cref="OkObjectResult"/> with the matching <see cref="OrderDto"/> when found,
    /// or <see cref="NotFoundResult"/> when no matching order exists.
    /// </returns>
    [Function("GetOrderById")]
    public IActionResult GetById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{orderId}")] HttpRequest req)
    {
        var orderId = req.RouteValues.ContainsKey("orderId") ? req.RouteValues["orderId"]?.ToString() : null;
        if (string.IsNullOrWhiteSpace(orderId))
        {
            orderId = req.Query["orderId"].ToString();
        }

        _logger.LogInformation("Fetching order {OrderId}.", orderId);

        var orders = new List<OrderDto>
        {
            new("ORD-1001", 2500m, "Completed"),
            new("ORD-1002", 1800m, "Processing"),
            new("ORD-1003", 3200m, "Pending")
        };

        var order = orders.FirstOrDefault(o => string.Equals(o.OrderId, orderId, StringComparison.OrdinalIgnoreCase));
        if (order is null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(order);
    }

   
}

public record OrderDto(string OrderId, decimal Amount, string Status);