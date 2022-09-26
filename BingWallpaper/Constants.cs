using System;
using System.IO;

namespace Kfstorm.BingWallpaper
{
    public static class Constants
    {
        public static string DataPath
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), @"K.F.Storm\BingWallpaper");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string LogFile
        {
            get { return "log.log"; }
        }

        public static string StateFile
        {
            get { return "state.xml"; }
        }

        public static string JpgFile
        {
            get { return "image.jpg"; }
        }

        public static string BmpFile
        {
            get { return "image.bmp"; }
        }

        public static string WallpaperInfoUrl
        {
            get { return string.Format("https://www.bing.com/hpimagearchive.aspx?format=xml&idx={0}&n={1}&mkt={2}", PictureIndex, PictureCount, Market); }
        }

        public static int PictureCount
        {
            get { return 1; }
        }

        public static int PictureIndex
        {
            get { return 0; }
        }

        public static string Market
        {
            get { return "zh-cn"; }
        }

        public static string PictureUrlFormat
        {
            get { return "https://www.bing.com{0}_UHD.jpg"; }
        }

        public static TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"); }
        }
    }
}
