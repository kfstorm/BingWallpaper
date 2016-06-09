using System.Threading.Tasks;

namespace Kfstorm.BingWallpaper
{
    public interface IWebAccessor
    {
        Task DownloadFileAsync(string url, string fileName);
        Task<string> DownloadStringAsync(string url);
    }
}
