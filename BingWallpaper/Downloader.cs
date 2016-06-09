using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Kfstorm.BingWallpaper
{
    public class Downloader : IDisposable
    {
        private readonly IWebAccessor _webAccessor;
        public State State { get; protected set; }

        private readonly bool _isJpgSupported = Environment.OSVersion.Version >= new Version(6, 0);

        bool _firstTime = true;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _task;

        public Downloader(IWebAccessor webAccessor)
        {
            _webAccessor = webAccessor;
            State = LoadState();
        }

        public void Run()
        {
            if (_task != null)
            {
                throw new InvalidOperationException("Already started running");
            }
            _task = DoWork(_cts.Token);
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

        private async Task DoWork(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var xmlContent = await _webAccessor.DownloadStringAsync(Constants.WallpaperInfoUrl);
                    ct.ThrowIfCancellationRequested();
                    var info = new BingWallpaperInfo(xmlContent);
                    if (info.PictureUrl != State.PictureUrl || !File.Exists(State.PictureFilePath))
                    {
                        try
                        {
                            await _webAccessor.DownloadFileAsync(info.PictureUrl, Constants.JpgFile);
                            ct.ThrowIfCancellationRequested();

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
                            State.Copyright = info.Copyright;
                            SaveState(State);
                            OnDownloadCompleted();
                        }
                        catch (Exception ex)
                        {
                            Log(ex.ToString());
                            try
                            {
                                foreach (var file in new[] {Constants.JpgFile, Constants.BmpFile})
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

                await Task.Delay(TimeSpan.FromMinutes(10), ct);
            }
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

        public void Dispose()
        {
            if (_task != null)
            {
                _cts.Cancel();
                try
                {
                    _task.Wait();
                    _task = null;
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}