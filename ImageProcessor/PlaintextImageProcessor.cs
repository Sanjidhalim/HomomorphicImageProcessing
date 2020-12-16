using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageProcessor
{
    /// <summary>
    /// Plaintext image processor.
    /// </summary>
    public static class PlaintextImageProcessor
    {
        private static int[,] SobelX = new int[,]
        {
            { -1, -2, -1 },
            { 0, 0, 0 },
            { 1, 2, 1 }
        };

        private static int[,] SobelY = new int[,]
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        public static Bitmap ConvertToGrayScale(Bitmap image)
        {
            var data = new List<int>();

            var bmp = (Bitmap)image;
            var newBmp = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //get the pixel from the scrBitmap image
                    var actualColor = bmp.GetPixel(i, j);
                    int grayScale = (int)(((actualColor.R * 0.3) + (actualColor.G * 0.59) + (actualColor.B * 0.11)));
                    data.Add(grayScale);
                    newBmp.SetPixel(i, j, Color.FromArgb(actualColor.A, grayScale, grayScale, grayScale));
                }
            }

            return newBmp;
        }

        public static Bitmap DetectEdge(Bitmap image)
        {
            var bmp = ConvertToGrayScale(image);
            var newBmp = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var sumY = 0;
                    var sumX = 0;
                    var px = bmp.GetPixel(i, j);

                    for (var ii = i - 1; ii <= i + 1; ii++)
                    {
                        for (var jj = j - 1; jj <= j + 1; jj++)
                        {
                            if (ii < 0 || ii >= bmp.Width || jj < 0 || jj >= bmp.Height)
                            {
                                continue;
                            }

                            var resY = bmp.GetPixel(ii, jj).R * SobelY[ii - i + 1, jj - j + 1];
                            var resX = bmp.GetPixel(ii, jj).R * SobelX[ii - i + 1, jj - j + 1];

                            sumY += resY;
                            sumX += resX;
                        }

                    }

                    //if (sumY > 255) sumY = 255;
                    //if (sumY < 0) sumY = 0;

                    //if (sumX > 255) sumX = 255;
                    //if (sumY < 0) sumX = 0;

                    var combined = (Math.Abs(sumX) + Math.Abs(sumY)) / 2;

                    if (combined > 255) combined = 255;
                    if (combined < 0) combined = 0;

                    newBmp.SetPixel(i, j, Color.FromArgb(px.A, combined, combined, combined));

                }
            }

            return newBmp;
        }
    }
}
