using System;
using System.Drawing;

namespace ImageProcessor
{
    public static class ImageIo
    {
        public static Bitmap ReadImage(string location) => (Bitmap)Image.FromFile(location);

        public static void SaveImage(string location, Bitmap bmp) => bmp.Save(location);
    }
}
