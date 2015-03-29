using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Kfstorm.BingWallpaper
{
    internal class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        internal const uint SPI_SETDESKWALLPAPER = 0x0014;

        internal const uint SPIF_UPDATEINIFILE = 0x01;
        internal const uint SPIF_SENDCHANGE = 0x02;
        internal const uint SPIF_SENDWININICHANGE = 0x02;
    }
}
