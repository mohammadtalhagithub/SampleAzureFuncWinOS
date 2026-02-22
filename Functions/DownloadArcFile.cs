using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Xml;
using Xceed.FileSystem;

namespace BeeSys.Utilities.Functions;

public class DownloadArcFile
{
    private readonly ILogger<DownloadArcFile> _logger;
    private readonly CDownloadArcFile _cDownload;

    public DownloadArcFile
        (
        ILogger<DownloadArcFile> logger,
        CDownloadArcFile cDownload)
    {
        _logger = logger;
        _cDownload = cDownload;

        Xceed.Zip.Licenser.LicenseKey = "ZIN32-NFZUB-W4G7K-R45A";
        Xceed.Compression.Licenser.LicenseKey = "ZIN32-NFZUB-W4G7K-R45A";
        Xceed.FileSystem.Licenser.LicenseKey = "ZIN32-NFZUB-W4G7K-R45A";
    }


    private void UseTempFolder()
    {
        // Get correct base path for both local and Azure
        string basePath = Environment.GetEnvironmentVariable("HOME") != null
            ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot")
            : Directory.GetCurrentDirectory();

        // Source file (read-only location)
        string sFullPath = Path.Combine(basePath, "ArcFile", "INT BUGS.arcx");

        // Use /tmp for any temporary write operations (writable in Azure)
        string tempPath = Path.Combine(Path.GetTempPath(), "ArcFile");
        Directory.CreateDirectory(tempPath); // Create temp folder if not exists

        // Copy source file to temp location before processing
        string tempFilePath = Path.Combine(tempPath, "INT BUGS.arcx");
        File.Copy(sFullPath, tempFilePath, overwrite: true);
    }



    [Function("DownloadArcFile")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("DownloadArcFile called 001 ");
        
        string arcxFileFolder = "ArcFile";
        string arcxFileName = "INT BUGS.arcx";
        string licenseId = "007976A1-FC69-4CFD-93C8-A60E8801FD27";
        string licensee = "defaultuser";

        try
        {
            //var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query); // in isolated worker model, use req.Url.Query instead of req.Query
            //string orderId = query["orderId"];
            //string name = query["name"];

            string trace = "0";
            trace = req?.Query?["trace"];


            try
            {
                //BeeSys.WaspHandShake waspHandShake = new BeeSys.WaspHandShake();
                //string bstrPrivate = "454353322000000065876ca4993f06098545896004ee5f152d0b2780d8c8fc9c5b21472313600a6e740c3fd6120bd17949ae9698775ac0884847b215a73bc1ac590fbc4f6be9f6a5e8a1d2685f5dffd8ddec6b86de1fb03a34afe5516f1955a31ddc0f1c42af664d";
                //string bstrPublic = "454353312000000065876ca4993f06098545896004ee5f152d0b2780d8c8fc9c5b21472313600a6e740c3fd6120bd17949ae9698775ac0884847b215a73bc1ac590fbc4f6be9f6a5";

                //waspHandShake.SetTokens(bstrPrivate, bstrPublic);

                //_logger.LogInformation("DownloadArcFile --- Handshake Done");
            }
            catch (Exception ex)
            {
                _logger.LogError("DownloadArcFile :: --- Error in Handshake.");
            }

            string basePath = Environment.GetEnvironmentVariable("HOME") != null
               ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot")
               : Directory.GetCurrentDirectory();
            
            string sFullPath = Path.Combine(basePath, arcxFileFolder, arcxFileName); // //string sFullPath = Path.Combine(basePath, "ArcFile", "INT BUGS.arcx");
            //string sFullPath = PathHandler.GetPathAccordingToOS(_sPath + zipFilePath);
            
            string bkupPath = null;
            if ( ! System.IO.File.Exists(sFullPath))
            {
                _logger.LogError($"DownloadArcFile => No file at {sFullPath}");
                return new OkObjectResult($"No file at {sFullPath}");
            }
            //string sFullPath = PathHandler.GetPathAccordingToOS(_sPath + zipFilePath);
            // Generate a random file name for the backup file.
            string bkPFile = Path.GetRandomFileName();//GeneratingRandomString();  // sFullPath + ".bkp";
            string dir = Path.GetDirectoryName(sFullPath);// Get the directory of the full path where the original file resides.

            // Create a backup of the original file by copying it to the same directory with a ".bkp" extension.
            System.IO.File.Copy(sFullPath, Path.Combine(dir, bkPFile + ".bkp"), true);
            bkupPath = Path.Combine(dir, bkPFile + ".bkp"); // Set the backup path to the newly created backup file. 
            // Create a DiskFile object for the backup zip file to allow access to its contents.

            if (trace == "1")
                return new OkObjectResult("Before DiskFile m_dskZipFile ");


            DiskFile m_dskZipFile = new DiskFile(bkupPath);

            if (trace == "2")
                return new OkObjectResult("AFTER DiskFile m_dskZipFile ");
            
            
            var m_zaZipFile = new Xceed.Zip.ZipArchive(m_dskZipFile);// Open the zip file as a ZipArchive using Xceed's library.

            if (trace == "3")
                return new OkObjectResult("AFTER  var m_zaZipFile = new Xceed.Zip.ZipArchive(m_dskZipFile);");

            //CDownloadArcFile cDownload = new CDownloadArcFile();
            var resp = _cDownload.AssignLicense(m_zaZipFile, licenseId, licensee, trace);

            return new OkObjectResult(resp);
        }
        catch (Exception ex)
        {
            _logger.LogError($"DownloadArcFile => Exception => {ex}");
            return new OkObjectResult($"Exception :: {ex}");
        }

        //_logger.LogInformation("DownloadArcFile function processed a request.");
        //return new OkObjectResult("Welcome to Azure Functions!");
    }

}