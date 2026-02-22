using BeeSys.Utilities.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BeeSys.Utilities.Functions
{
    public class AuthFunction
    {
        private readonly ILogger<AuthFunction> _logger;

        public AuthFunction(ILogger<AuthFunction> logger)
        {
            _logger = logger;
        }

        //[Authorize]
        //[AllowAnonymous]
        [Function("ValidateUserFunction")]
        public IActionResult ValidateUser
            (
            [HttpTrigger("get", Route = "validate")]
            HttpRequestData req, string licenseID
            )
        {
            try
            {
                // Get query parameter
                var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
                if (string.IsNullOrWhiteSpace(licenseID))
                {
                    licenseID = query["licenseID"];
                }

                return new OkObjectResult(new
                {
                    message = $"licenseID == {licenseID}!",
                    utc = DateTimeOffset.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuthFunction");
                return new OkObjectResult(new
                {
                    data = "Exception",
                    status = false
                });
            }
            
        }
    }
}
