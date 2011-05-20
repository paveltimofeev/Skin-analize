using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace Service
{
    public static class Exstensions
    {
        /// <summary>
        /// Regurns neighbours of the point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point[] Neighbors(this Point point)
        {
            int sensetive = 1;

            int mX = point.X - sensetive;
            int mY = point.Y - sensetive;
            int pX = point.X + sensetive;
            int pY = point.Y + sensetive;

            Point[] pts = new Point[8];

            pts[0] = new Point(mX, mY);
            pts[1] = new Point(point.X, mY);
            pts[2] = new Point(pX, mY);
            pts[3] = new Point(mX, point.Y);
            pts[4] = new Point(pX, point.Y);
            pts[5] = new Point(mX, pY);
            pts[6] = new Point(point.X, pY);
            pts[7] = new Point(pX, pY);

            return pts;
        }

        /// <summary>
        /// Regurns neighbours of the point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static List<Point> LabeledNeighbors(this Point point, bool[,] dataMask, Size max)
        {
            int mX = point.X - 1;
            int mY = point.Y - 1;
            int pX = point.X + 1;
            int pY = point.Y + 1;

            if (pX >= max.Width)
                pX = point.X;

            if (pY >= max.Height)
                pY = point.Y;

            if (mX < 0)
                mX = 0;

            if (mY < 0)
                mY = 0;

            List<Point> pts = new List<Point>();

            if (dataMask[mX, mY])
            {
                pts.Add(new Point(mX, mY));
            }

            if (dataMask[point.X, mY])
            {
                pts.Add(new Point(point.X, mY));
            }

            if (dataMask[pX, mY])
            {
                pts.Add(new Point(pX, mY));
            }

            if (dataMask[mX, point.Y])
            {
                pts.Add(new Point(mX, point.Y));
            }

            if (dataMask[pX, point.Y])
            {
                pts.Add(new Point(pX, point.Y));
            }

            if (dataMask[mX, pY])
            {
                pts.Add(new Point(mX, pY));
            }

            if (dataMask[point.X, pY])
            {
                pts.Add(new Point(point.X, pY));
            }

            if (dataMask[pX, pY])
            {
                pts.Add(new Point(pX, pY));
            }

            return pts;
        }

        /// <summary>
        /// Convert RGS to normalized RGB-colour (Normalized RGB color space removes the luminance component through the normalization)
        /// </summary>
        /// <param name="c">Tipical RGB</param>
        /// <returns>Normalized RGB</returns>
        private static Color NormalizeRGB(this Color c)
        {
            int r, g, b, sum;
            sum = c.R + c.G + c.B;

            if (sum > 0)
            {
                r = c.R / sum;
                g = c.G / sum;
                b = c.B / sum;
            }
            else
            {
                r = 0; g = 0; b = 0;
            }

            Color normalizedRGB = Color.FromArgb(r, g, b);

            return normalizedRGB;
        }

        /// <summary>
        /// Converts color from RGB space to HSV space (Hue: 0-360; Saturation: 0-1; Value: 0-1).
        /// </summary>
        /// <param name="c">RGB color (Red: 0-255; Green: 0-255; Blue: 0-255)</param>
        /// <returns>HSV color space (Hue: 0-360; Saturation: 0-1; Value: 0-1)</returns>
        public static HSV ToHSV(this Color c)
        {
            float r = c.R;
            float g = c.G;
            float b = c.B;
            float sum = r + g + b;
            float h = 0;
            float s = 0;
            float v = 0;
            float mx = Math.Max(Math.Max(r, g), b);
            float mn = Math.Min(Math.Min(r, g), b);
            float dif = mx - mn;
            
            if (mx != mn)
            {
                if (mx == r)
                {
                    h = (g - b) / dif;
                }
                else if (mx == g)
                {
                    h = 2 + ((g - r) / dif);
                }
                else
                {
                    h = 4 + ((r - g) / dif);
                }

                h = h * 60;
                if (h < 0)
                {
                    h = h + 360;
                }
            }
            else
            {
                h = 0;
            }

            if (sum != 0)
            {
                s = 1 - (3 * mn / sum);
                v = (1 / 3) * sum;
            }
            else
            {
                s = 1;
                v = 0;
            }

            HSV hsv = new HSV(h, s, v);

            return hsv;
        }

        /// <summary>
        /// Converts color from RGB space to YCbCr space.
        /// </summary>
        /// <param name="c">RGB color (Red: 0-255; Green: 0-255; Blue: 0-255)</param>
        /// <returns></returns>
        public static YCbCr ToYCbCr(this Color c)
        {
            float y, cb, cr;
            float r = c.R;
            float g = c.G;
            float b = c.B;

            y = 16 + (65.738F * r / 256.0F) + (129.057F * g / 256.0F) + (25.064F * b / 256.0F);
            cb = 128 + (-37.945F * r / 256.0F) - (74.494F * g / 256.0F) + (112.439F * b / 256.0F);
            cr = 128 + (112.439F * r / 256.0F) - (94.154F * g / 256.0F) - (18.285F * b / 256.0F);

            YCbCr result = new YCbCr(y, cb, cr);
            return result;
        }

        /// <summary>
        /// Converts color from RGB space to YCbCr space.
        /// By http://www.couleur.org/index.php?page=transformations#YCbCr
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static YCbCr ToYCbCr2(this Color c)
        {
            double y, cb, cr;
            double r = c.R;
            double g = c.G;
            double b = c.B;

            y = 0.2989D * r + 0.5866d * g + 0.1145d * b;
            cb = -0.1688d * r - 0.3312d * g + 0.5000d * b;
            cr = 0.5000d * r - 0.4184d * g - 0.0816d * b;

            YCbCr result = new YCbCr((float)y, (float)cb, (float)cr);
            return result;
        }

        /// <summary>
        /// Converts color from YCbCr space to RGB space.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>RGB color (Red: 0-255; Green: 0-255; Blue: 0-255)</returns>
        public static Color ToRGB(this YCbCr c)
        {
            float y = c.y;
            float cb = c.cb;
            float cr = c.cr;
            float r = 0;
            float g = 0;
            float b = 0;

            r = Math.Abs((298.082F * y) / 256 + (408.583F * cr) / 256 - 222.921F);
            g = Math.Abs((298.082F * y) / 256 - (100.291F * cb) / 256 - (208.120F * cr) / 256 + 135.576F);
            b = Math.Abs((298.082F * y) / 256 + (516.412F * cb) / 256 - 276.836F);

            r = r > 255 ? 255 : r;
            g = g > 255 ? 255 : g;
            b = b > 255 ? 255 : b;

            Color result = Color.FromArgb((int)r, (int)g, (int)b);
            return result;
        }

        /// <summary>
        /// Quick unsafe scanning bitmap and invoke ScanHandler delegate.
        /// </summary>
        /// <param name="img">Bitmap image</param>
        /// <param name="handler">ScanHandler delegate</param>
        /// <returns></returns>
        public static Bitmap UnsafeScan(this Bitmap img, UnsafeScanHandler handler, ImageLockMode mode)
        {

            unsafe
            {
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), mode, PixelFormat.Format32bppArgb);
            int y, x;

                byte* imagePointer = (byte*)bitmapData.Scan0;
                for (y = 0; y < bitmapData.Height; y++)
                {
                    for (x = 0; x < bitmapData.Width; x++)
                    {
                        handler.Invoke(
                            x,
                            y,
                            imagePointer[2],
                            imagePointer[1],
                            imagePointer[0]);

                        imagePointer += 4;
                    }
                    imagePointer += (bitmapData.Stride - (bitmapData.Width * 4));
                }

            img.UnlockBits(bitmapData);
            }

            return img;
        }

        /// <summary>
        /// Convert colors of image to Brightness map, where each pixel has color from brightness of it.
        /// And retun average value of brightness (from 0 to 255).
        /// </summary>
        /// <param name="img"></param>
        public static float UnsafeBrightnessMap(this Bitmap img)
        {
            int y, x, totalBr = 0;

            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* imagePointer = (byte*)bitmapData.Scan0;
                for (y = 0; y < bitmapData.Height; y++)
                {
                    for (x = 0; x < bitmapData.Width; x++)
                    {
                        byte br = (byte)(Color
                                            .FromArgb(imagePointer[2], imagePointer[1], imagePointer[0])
                                                .GetBrightness() * 255.0f);
                        imagePointer[2] = br;
                        imagePointer[1] = br;
                        imagePointer[0] = br;

                        totalBr += br;

                        imagePointer += 4;
                    }
                    imagePointer += (bitmapData.Stride - (bitmapData.Width * 4));
                }
            }
            img.UnlockBits(bitmapData);

            float avgBr = (float)totalBr / (float)(img.Width * img.Height);
            return avgBr;
        }

        /// <summary>
        /// Преобразует изображение в карту отклонений от средней яркости в диапазоне от 0 до 255.
        /// 0 - (тёмные области) минимальное отклонение, 255 - (светлые области) максимальное отклонение от средней яркости (точки акцента).
        /// Также возвращает среднее значение отклонения.
        /// </summary>
        /// <param name="img"></param>
        public static float UnsafeDeviationFromBrightnessMap(this Bitmap img)
        {
            int y, x, totalBr = 0;

            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                float tempTotalBr = 0f;
                byte* imagePointer = (byte*)bitmapData.Scan0;
                for (y = 0; y < bitmapData.Height; y++)
                {
                    for (x = 0; x < bitmapData.Width; x++)
                    {
                        tempTotalBr += Color.FromArgb(imagePointer[2], imagePointer[1], imagePointer[0]).GetBrightness();
                        imagePointer += 4;
                    }
                    imagePointer += (bitmapData.Stride - (bitmapData.Width * 4));
                }
                float avgBr = tempTotalBr / (float)(img.Width * img.Height);

                ////////////////////////

                imagePointer = (byte*)bitmapData.Scan0;
                for (y = 0; y < bitmapData.Height; y++)
                {
                    for (x = 0; x < bitmapData.Width; x++)
                    {
                        Color c = Color.FromArgb(imagePointer[2], imagePointer[1], imagePointer[0]);
                        byte d = (byte)(Math.Abs(avgBr - c.GetBrightness()) * 255.0F);

                        imagePointer[2] = d;
                        imagePointer[1] = d;
                        imagePointer[0] = d;
                        totalBr += d;

                        imagePointer += 4;
                    }
                    imagePointer += (bitmapData.Stride - (bitmapData.Width * 4));
                }
            }
            img.UnlockBits(bitmapData);

            return (float)totalBr / (float)(img.Width * img.Height);
        }

        /// <summary>
        /// Returns tumblnail image fit into the max size square.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="maxSize"></param>
        public static Bitmap GetThumbnailImage(this Bitmap img, int maxSize)
        {
            Size tumb = img.Width > img.Height ? new Size(maxSize, (int)((float)img.Height / ((float)img.Width / (float)maxSize))) : new Size((int)((float)img.Width / ((float)img.Height / (float)maxSize)), maxSize);
            return new Bitmap(img.GetThumbnailImage(tumb.Width, tumb.Height, null, IntPtr.Zero));
        }
    }

    /// <summary>
    /// Represent the method that will handle scan of point event.
    /// </summary>
    /// <param name="x">X-axis coordinate</param>
    /// <param name="y">Y-axis coordinate</param>
    /// <param name="r">Red value of point</param>
    /// <param name="g">Green value of point</param>
    /// <param name="b">Blue value of point</param>
    public delegate void UnsafeScanHandler(int x, int y, byte r, byte g, byte b);

    /// <summary>
    /// YCbCr color space (Y - luma signal (16-235) , Cb and Cr - chroma components (16-240))
    /// </summary>
    public struct YCbCr
    {
        public float y;
        public float cb;
        public float cr;

        public YCbCr(float y, float cb, float cr)
        {
            this.y = y;
            this.cr = cr;
            this.cb = cb;
        }
    }
    
    /// <summary>
    /// HSV color space (Hue: 0-360; Saturation: 0-1; Value: 0-1)
    /// </summary>
    public struct HSV
    {
        public double H;
        public double S;
        public double V;

        public HSV(double h, double s, double v)
        {
            if ((h < 0 | h > 360) | (s < 0 | s > 1) | (v < 0 | v > 1))
                throw new ArgumentOutOfRangeException();

            this.H = h;
            this.S = s;
            this.V = v;
        }

        public HSV(Color c)
        {
            this = c.ToHSV();
        }

        public override string ToString()
        {
            return string.Format("Hue:{0} Saturation:{1} Value:{2}", H.ToString("0"), S.ToString("0.00"), V.ToString("0.00"));
        }
    }

    /// <summary>
    /// Класс для простого замера временных интервалов
    /// </summary>
    public static class TimeMark
    {
        static DateTime? d;
        /// <summary>
        /// При первом вызове делает метку и возвращает 0, при втором сбрасывает метку и возвращает интервал в милисекундах прошедший с момента создания первой метки.
        /// </summary>
        /// <returns></returns>
        public static double Mark()
        {
            double result = 0.0D;

            if (!d.HasValue)
            {
                d = DateTime.Now;
            }
            else
            {
                result = ((TimeSpan)(DateTime.Now - d)).TotalMilliseconds;
                Reset();
            }

            return result;
        }
        /// <summary>
        /// Метод аналогичен Mark, но возвращает значение в строковом формате '#.##ms'.
        /// </summary>
        /// <returns></returns>
        public static string MarkToString()
        {
            return Mark().ToString("#.##ms");
        }
        /// <summary>
        /// Сброс метки
        /// </summary>
        public static void Reset()
        {
            d = null;
        }

        /// <summary>
        /// Показывает прошедший интервал времени с момента создания первой метки, не сбрасывая его.
        /// </summary>
        public static TimeSpan Span
        {
            get
            {
                if (d.HasValue)
                {
                    return (DateTime.Now - d.Value);
                }
                else
                    return new TimeSpan(0);
            }
        }
    }
}
