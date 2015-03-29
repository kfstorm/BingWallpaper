using System;

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
