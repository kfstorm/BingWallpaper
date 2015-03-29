namespace Kfstorm.BingWallpaper
{
    public interface IWebAccessor
    {
        void DownloadFile(string url, string fileName);
        string DownloadString(string url);
    }
}
