using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Kfstorm.BingWallpaper
{
    internal static class Program
    {
        private static Downloader _downloader;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            bool runInBackground = args.Contains("/background");

            bool createdNew;
            using (new Mutex(true, "{1223FEA2-6E7E-48AF-AB44-DA16CD88443A}", out createdNew))
            {
                if (!createdNew)
                {
                    if (!runInBackground)
                    {
                        SendMessage("/show");
                    }
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Environment.CurrentDirectory = Constants.DataPath;

                _downloader = new Downloader();
                CreateMainForm();
                _downloader.DownloadCompleted += _downloader_DownloadCompleted;
                _downloader.WallpaperChanged += _downloader_WallpaperChanged;
                _downloader.Run();
                if (!runInBackground)
                {
                    ShowMainForm();
                }
                StartListen();
                Application.Run();
            }
        }

        private static void _downloader_WallpaperChanged(object sender, WallpaperChangedEventArgs e)
        {
            //NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETDESKWALLPAPER, 0, Path.GetFullPath(filename),
            //                                   NativeMethods.SPIF_UPDATEINIFILE | NativeMethods.SPIF_SENDCHANGE);
            _mainForm.Invoke((MethodInvoker)delegate
            {
                IActiveDesktop iad = shlobj.GetActiveDesktop();
                iad.SetWallpaper(Path.GetFullPath(e.WallpaperFilePath), 0);
                iad.ApplyChanges(AD_Apply.ALL | AD_Apply.FORCE | AD_Apply.BUFFERED_REFRESH);
                Marshal.ReleaseComObject(iad);
            });

        }

        static void _downloader_DownloadCompleted(object sender, EventArgs e)
        {
            _mainForm.BeginInvoke(new Action(() => _mainForm.UpdateState(_downloader.State)));
        }

        private static readonly string RunOnStartupCommandLine = string.Format("\"{0}\" {1}", Application.ExecutablePath,
                                                                               "/background");

        private const string RunOnStartupSubKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string RunOnStartupValueName = "BingWallpaper";

        public static bool GetRunOnStartup()
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(RunOnStartupSubKeyName) ??
                                  Registry.CurrentUser.CreateSubKey(RunOnStartupSubKeyName);
                return reg != null && RunOnStartupCommandLine == (string) reg.GetValue(RunOnStartupValueName);
            }
            catch
            {
                return false;
            }
        }

        public static void SetRunOnStartup(bool startup)
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(RunOnStartupSubKeyName, true) ??
                                  Registry.CurrentUser.CreateSubKey(RunOnStartupSubKeyName);
                if (reg == null) return;
                if (startup)
                {
                    reg.SetValue(RunOnStartupValueName, RunOnStartupCommandLine);
                }
                else
                {
                    reg.DeleteValue(RunOnStartupValueName);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void Exit()
        {
            _downloader.Stop();
            _downloader = null;
            Application.Exit();
        }

        private static MainForm _mainForm;

        public static void CreateMainForm()
        {
            if (_mainForm == null)
            {
                _mainForm = new MainForm();
                _mainForm.UpdateState(_downloader.State);

                double opacity = _mainForm.Opacity;
                bool showInTaskbar = _mainForm.ShowInTaskbar;
                
                _mainForm.Opacity = 0;
                _mainForm.ShowInTaskbar = false;
                _mainForm.SetShowWithoutActivation(true);

                _mainForm.Show();
                _mainForm.Hide();

                _mainForm.Opacity = opacity;
                _mainForm.ShowInTaskbar = showInTaskbar;
                _mainForm.SetShowWithoutActivation(false);
            }
        }

        public static void ShowMainForm()
        {
            CreateMainForm();
            _mainForm.TopMost = true;
            _mainForm.TopMost = false;
            _mainForm.Show();
            _mainForm.Activate();
        }

        private static void StartListen()
        {
            var server = new NamedPipeServerStream("{6E790457-02D2-4FC2-9A7C-DA8751D4FC96}",
                                                   PipeDirection.In);
            StreamReader reader = null;
            ThreadPool.QueueUserWorkItem(o =>
                {
                    while (true)
                    {
                        server.WaitForConnection();
                        if (reader == null)
                        {
                            reader = new StreamReader(server);
                        }
                        var line = reader.ReadLine();
                        if (line == "/show")
                        {
                           _mainForm.BeginInvoke(new Action(ShowMainForm));
                        }
                        server.Disconnect();
                    }
                    // ReSharper disable once FunctionNeverReturns
                });
        }

        private static void SendMessage(string message)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", "{6E790457-02D2-4FC2-9A7C-DA8751D4FC96}",
                                                              PipeDirection.Out))
                {
                    client.Connect(1000);
                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}