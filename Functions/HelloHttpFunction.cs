using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeeSys.Utilities.Functions;

public sealed class HelloHttpFunction
{
    private readonly ILogger<HelloHttpFunction> _logger;

    public HelloHttpFunction(ILogger<HelloHttpFunction> logger)
    {
        _logger = logger;
    }

    [Function("HelloHttp")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var name = req.Query["name"].ToString();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "world";
        }

        _logger.LogInformation("HelloHttp triggered. name={Name}", name);

        return new OkObjectResult(new
        {
            message = $"Hello, {name}!",
            utc = DateTimeOffset.UtcNow
        });
    }

    [Function("GetData")]
    public IActionResult GetData(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        try
        {


            return new OkObjectResult(new
            {
                data = "Sample data from Function ",
                status = true
            });

        }
        catch (Exception ex)
        {
            return new OkObjectResult(new
            {
                data = "Exception",
                status = false
            });
        }
    }

#if false

    [Function("GetData")]
    public IActionResult GetData(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        try
        {
            Type typeHostManager = GetHostManager();
            ClassLibraryContracts.ICustomService hostMnger = null; // Initialize with a default value
            if (typeHostManager != null)
            {
                hostMnger = Activator.CreateInstance(typeHostManager) as ClassLibraryContracts.ICustomService;
                var data = hostMnger?.GetData();
                var status = hostMnger?.CheckStatus();

                return new OkObjectResult(new
                {
                    data = data,
                    status = status
                });
            }

            return new OkObjectResult(new
            {
                data = "typeHostManager Not found",
                status = false
            });
        }
        catch (Exception ex)
        {
            return new OkObjectResult(new
            {
                data = "Exception",
                status = false
            });
        }
    } 
#endif


    private static Type GetHostManager()
    {
        try
        {
            string dllPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Services",
                "ClassLibraryTest.Host.dll"
            );

            if (!File.Exists(dllPath))
            {
                //LogWriterCore.WriteLog("GetHostManager", $" File Not Found : {dllPath}");
                return null;
            }

            Assembly assembly = Assembly.LoadFrom(dllPath);
            return assembly.GetType("ClassLibraryTest.CustomService");
        }
        catch (Exception ex)
        {
            //LogWriterCore.WriteLog("GetHostManager", ex);
            return null;
        }
    }
}
