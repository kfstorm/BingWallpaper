using System.IO;
using System.Net;
using System.Text;

namespace Kfstorm.BingWallpaper
{
    internal class WebAccessor : IWebAccessor
    {
        private readonly WebClient _client = new WebClient();

        public void DownloadFile(string url, string fileName)
        {
            _client.DownloadFile(url, fileName);
        }

        public string DownloadString(string url)
        {
            return new StreamReader(new MemoryStream(_client.DownloadData(url)), Encoding.UTF8).ReadToEnd();
        }
    }
}
