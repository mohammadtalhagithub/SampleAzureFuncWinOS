using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BeeSys.Utilities.Helpers
{
    public class PathHandler
    {
        public static string GetPathAccordingToOS(string windowsPath)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return windowsPath;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return ConvertWinPathToLinux(windowsPath);
                }
            }
            catch (Exception ex)
            {
                //LogWriterCore.WriteLog("EmbedManifest", ex);
            }
            return string.Empty;
        }

        private static string ConvertWinPathToLinux(string windowsPath)
        {
            try
            {
                string linuxPath = windowsPath.Replace("\\", "/");

                if (linuxPath.Length > 2 && linuxPath[1] == ':')
                {
                    linuxPath = linuxPath.Substring(2).Replace('\\', '/');
                }
                if (linuxPath.Contains("//"))
                {
                    linuxPath = linuxPath.Replace("//", "/");
                }
                return linuxPath;

            }
            catch (Exception ex)
            {
                //LogWriterCore.WriteLog("ConvertWinPathToLinux", ex);
            }
            return string.Empty;
        }
    }
}
