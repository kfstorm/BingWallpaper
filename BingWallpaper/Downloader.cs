using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Kfstorm.BingWallpaper
{
    public class Downloader : IDisposable
    {
        private readonly IWebAccessor _webAccessor;
        public State State { get; protected set; }

        private readonly bool _isJpgSupported = Environment.OSVersion.Version >= new Version(6, 0);

        Timer _timer;

        BackgroundWorker _worker;

        bool _firstTime = true;

        public Downloader(IWebAccessor webAccessor)
        {
            _webAccessor = webAccessor;
            State = LoadState();
            _worker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _worker.DoWork += worker_DoWork;

            _timer = new Timer {Interval = 600000};
            _timer.Tick += timer_Tick;
        }

        public void Run()
        {
            _worker.RunWorkerAsync();
            _timer.Start();
        }

        public void Stop()
        {
            if (_worker.IsBusy)
            {
                _worker.CancelAsync();
            }
            _timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (_worker != null && !_worker.IsBusy && State.EndDate < DateTimeOffset.Now)
            {
                _worker.RunWorkerAsync();
            }
        }

        State LoadState()
        {
            State result = null;
            try
            {
                if (File.Exists(Constants.StateFile))
                {
                    using (Stream stream = File.OpenRead(Constants.StateFile))
                    {
                        var serializer = new XmlSerializer(typeof(State));
                        result = (State)serializer.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                if (result == null)
                {
                    result = new State();
                }
            }
            return result;
        }

        void SaveState(State state)
        {
            try
            {
                using (Stream stream = File.Open(Constants.StateFile, FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(State));
                    serializer.Serialize(stream, state);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public event EventHandler DownloadCompleted;

        protected virtual void OnDownloadCompleted()
        {
            EventHandler handler = DownloadCompleted;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_worker == null) return;
            if (_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                var xmlContent = _webAccessor.DownloadString(Constants.WallpaperInfoUrl);
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var info = new BingWallpaperInfo(xmlContent);
                if (info.PictureUrl != State.PictureUrl || !File.Exists(State.PictureFilePath))
                {
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadFile(info.PictureUrl, Constants.JpgFile);
                            if (_worker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (_isJpgSupported)
                            {
                                ChangeWallpager(Constants.JpgFile);
                            }
                            else
                            {
                                ConvertJpgToBmp(Constants.JpgFile, Constants.BmpFile);
                                File.Delete(Constants.JpgFile);
                                ChangeWallpager(Constants.BmpFile);
                            }
                            State.PictureUrl = info.PictureUrl;
                            State.PictureFilePath = _isJpgSupported ? Constants.JpgFile : Constants.BmpFile;
                            State.StartDate = info.StartDate;
                            State.EndDate = info.EndDate;
                            State.Copyright = info.Copyright;
                            SaveState(State);
                            OnDownloadCompleted();
                        }
                        catch (Exception ex)
                        {
                            Log(ex.ToString());
                            try
                            {
                                foreach (var file in new[] { Constants.JpgFile, Constants.BmpFile })
                                {
                                    if (File.Exists(file))
                                    {
                                        File.Delete(file);
                                    }
                                }
                            }
                            catch (Exception ex2)
                            {
                                Log(ex2.ToString());
                            }
                        }
                    }
                }
                else
                {
                    if (_firstTime)
                    {
                        ChangeWallpager(State.PictureFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            _firstTime = false;
            GC.Collect();
        }

        private void Log(string text)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("============================================");
                sb.AppendLine(DateTime.Now.ToString(CultureInfo.CurrentCulture));
                sb.AppendLine("============================================");
                sb.AppendLine(text);
                sb.AppendLine();
                File.AppendAllText(Constants.LogFile, sb.ToString());
            }
            catch
            {
                // ignored
            }
        }

        public event EventHandler<WallpaperChangedEventArgs> WallpaperChanged;

        private void OnWallpaperChanged(string wallpaperFilePath)
        {
            if (WallpaperChanged != null)
            {
                WallpaperChanged.Invoke(null, new WallpaperChangedEventArgs(wallpaperFilePath));
            }
        }

        private void ChangeWallpager(string filename)
        {
            OnWallpaperChanged(filename);
        }

        static void ConvertJpgToBmp(string jpgPicturePath, string bmpPicturePath)
        {
            using (var image = Image.FromFile(jpgPicturePath))
            {
                image.Save(bmpPicturePath, ImageFormat.Bmp);
            }
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_worker != null)
                    {
                        Stop();
                        _timer.Dispose();
                        _timer = null;
                        _worker.Dispose();
                        _worker = null;
                    }
                }
                _disposed = true;
            }
        }
        ~Downloader()
        {
            Dispose(false);
        }

    }
}