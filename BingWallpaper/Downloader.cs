using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Kfstorm.BingWallpaper
{
    public class Downloader : IDisposable
    {
        public State State { get; protected set; }

        private readonly bool _isJpgSupported = Environment.OSVersion.Version >= new Version(6, 0);

        Timer _timer;

        BackgroundWorker _worker;

        bool _firstTime = true;

        public Downloader()
        {
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
                XmlReader reader = XmlReader.Create(Constants.WallpaperInfoUrl);
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                var document = new XmlDocument();
                document.Load(reader);
                string pictureUrl = string.Format(Constants.PictureUrlFormat, document.GetElementsByTagName("urlBase")[0].InnerText);
                string copyright = document.GetElementsByTagName("copyright")[0].InnerText;
                string date1 = document.GetElementsByTagName("fullstartdate")[0].InnerText;
                string date2 = document.GetElementsByTagName("enddate")[0].InnerText;
                DateTime temp = new DateTime(int.Parse(date1.Substring(0, 4)), int.Parse(date1.Substring(4, 2)), int.Parse(date1.Substring(6, 2)), int.Parse(date1.Substring(8, 2)), int.Parse(date1.Substring(10, 2)), 0, DateTimeKind.Unspecified);
                DateTimeOffset startDate = new DateTimeOffset(temp, Constants.TimeZone.GetUtcOffset(temp));
                temp = new DateTime(int.Parse(date2.Substring(0, 4)), int.Parse(date2.Substring(4, 2)), int.Parse(date2.Substring(6, 2)), 0, 0, 0, DateTimeKind.Unspecified);
                DateTimeOffset endDate = new DateTimeOffset(temp, Constants.TimeZone.GetUtcOffset(temp));
                if (pictureUrl != State.PictureUrl || !File.Exists(State.PictureFilePath))
                {
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadFile(pictureUrl, Constants.JpgFile);
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
                            State.PictureUrl = pictureUrl;
                            State.PictureFilePath = _isJpgSupported ? Constants.JpgFile : Constants.BmpFile;
                            State.StartDate = startDate;
                            State.EndDate = endDate;
                            State.Copyright = copyright;
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