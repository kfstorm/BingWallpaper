using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Kfstorm.BingWallpaper
{
    public static class UrlHelper
    {
        /// <summary>
        /// 安全地使用默认浏览器打开网页
        /// </summary>
        /// <param name="url">The URL.</param>
        public static void OpenLink(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception exc1)
            {
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                if (!(exc1 is Win32Exception))
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo("IExplore.exe", url);
                        Process.Start(startInfo);
                    }
                    catch (Exception exc2)
                    {
                        // still nothing we can do so just show the error to the user here.
                        MessageBox.Show(exc2.Message);
                    }
                }
            }
        }
    }
}