using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wifi_QR_Code_Scanner_Legacy.ImageUtils
{
    public class ImageUtils
    {
        private BitmapImage bitmapImage;
        private MemoryStream memoryStream;
        public ImageUtils()
        {
            this.bitmapImage = new BitmapImage();
            this.memoryStream = new MemoryStream();
        }
        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;

            bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}
