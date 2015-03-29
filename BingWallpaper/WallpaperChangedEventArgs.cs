using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kfstorm.BingWallpaper
{
    public class WallpaperChangedEventArgs : EventArgs
    {
        public WallpaperChangedEventArgs(string wallpaperFilePath)
        {
            WallpaperFilePath = wallpaperFilePath;
        }

        public string WallpaperFilePath { get; private set; }
    }
}
