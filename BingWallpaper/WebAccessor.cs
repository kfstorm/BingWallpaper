using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kfstorm.BingWallpaper
{
    internal class WebAccessor : IWebAccessor
    {
        private readonly WebClient _client = new WebClient {Encoding = Encoding.UTF8};

        public Task DownloadFileAsync(string url, string fileName)
        {
            return _client.DownloadFileTaskAsync(url, fileName);
        }

        public Task<string> DownloadStringAsync(string url)
        {
            return _client.DownloadStringTaskAsync(url);
        }
    }
}
