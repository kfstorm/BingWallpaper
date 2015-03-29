using System;
using System.Xml;

namespace Kfstorm.BingWallpaper
{
    public class BingWallpaperInfo
    {
        public string PictureUrl { get; set; }

        public string Copyright { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public BingWallpaperInfo(string xmlContent)
        {
            var document = new XmlDocument();
            document.LoadXml(xmlContent);
            PictureUrl = string.Format(Constants.PictureUrlFormat, document.GetElementsByTagName("urlBase")[0].InnerText);
            Copyright = document.GetElementsByTagName("copyright")[0].InnerText;
            string date1 = document.GetElementsByTagName("fullstartdate")[0].InnerText;
            string date2 = document.GetElementsByTagName("enddate")[0].InnerText;
            DateTime temp = new DateTime(int.Parse(date1.Substring(0, 4)), int.Parse(date1.Substring(4, 2)), int.Parse(date1.Substring(6, 2)), int.Parse(date1.Substring(8, 2)), int.Parse(date1.Substring(10, 2)), 0, DateTimeKind.Unspecified);
            StartDate = new DateTimeOffset(temp, Constants.TimeZone.GetUtcOffset(temp));
            temp = new DateTime(int.Parse(date2.Substring(0, 4)), int.Parse(date2.Substring(4, 2)), int.Parse(date2.Substring(6, 2)), 0, 0, 0, DateTimeKind.Unspecified);
            EndDate = new DateTimeOffset(temp, Constants.TimeZone.GetUtcOffset(temp));
        }
    }
}
