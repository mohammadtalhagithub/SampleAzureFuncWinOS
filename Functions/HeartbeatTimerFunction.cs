using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BeeSys.Utilities.Functions;

public sealed class HeartbeatTimerFunction
{
    private readonly ILogger<HeartbeatTimerFunction> _logger;

    public HeartbeatTimerFunction(ILogger<HeartbeatTimerFunction> logger)
    {
        _logger = logger;
    }

    // Runs every 1 minutes.
    [Function("HeartbeatTimer1")]

    public void Run1([TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation(
            "HeartbeatTimer fired at {UtcNow}. ScheduleStatus={ScheduleStatus}",
            DateTimeOffset.UtcNow,
            timerInfo?.ScheduleStatus);
    }

    // Runs every 5 minutes.
    [Function("HeartbeatTimer5")]
    public void Run5([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation(
            "HeartbeatTimer fired at {UtcNow}. ScheduleStatus={ScheduleStatus}",
            DateTimeOffset.UtcNow,
            timerInfo?.ScheduleStatus);
    }
}



