using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using Service.Scanner.ScannersAndDataMaskBuilders;
using Service.Scanner.Analysers;
using Service.Scanner.Labelers;

namespace Service.Scanner.Drawers
{
    internal class UnsafeRegionDrawer
    {
        private Bitmap backLayer;
        private Bitmap drawLayer;
        private Graphics frontLayer;
        private Size size;
        private SRegion[] regions;
        private Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.Green, Color.BlueViolet, Color.Coral, Color.Navy, Color.Cyan, Color.DarkRed, Color.Gold, Color.Lime, Color.Maroon, Color.Orange, Color.RoyalBlue, Color.Magenta, Color.Purple, Color.Silver, Color.Violet};

        public Bitmap Background { get; set; }
        public ILabeler Labeler { get; set; }
        public bool DisplayBackground { get; set; }
        public bool OnlyMask { get; set; }
        public bool OnlySkin { get; set; }
        public bool DrawCenter { get; set; }
        public bool DrawTransparent { get; set; }
        public bool DrawPercent { get; set; }
        public bool DrawRegionFrame { get; set; }
        public bool DrawColorMask { get; set; }

        public UnsafeRegionDrawer(List<SRegion> regions)
        {
            SRegion[] regionsArray = new SRegion[regions.Count];
            regions.CopyTo(regionsArray);
            this.regions = regionsArray;

            Initialize(Background, regionsArray);
        }

        public UnsafeRegionDrawer(SRegion[] regions)
        {
            Initialize(Background, regions);
        }

        protected void Initialize(Bitmap background, SRegion[] regions)
        {
            this.size = background.Size;
            this.backLayer = new Bitmap(background, size.Width, size.Height);
            this.drawLayer = new Bitmap(size.Width, size.Height);
            this.frontLayer = Graphics.FromImage((Image)drawLayer);
            this.regions = regions;
        }

        public Bitmap[] DrawSplittedRegions()
        {
            Bitmap[] arr = new Bitmap[regions.Length];
            for (int i = 0; i < regions.Length; i++)
            {
                arr[i] = DrawRegion(i);
            }

            Bitmap result = new Bitmap(this.size.Width, this.size.Height);
            Graphics graphics = Graphics.FromImage(result);
            if (DisplayBackground)
                graphics.DrawImage(backLayer, 0, 0);
            graphics.DrawImage(drawLayer, 0, 0);

            return arr;
        }

        public Bitmap DrawRegion(int regionIndex)
        {
            SRegion r = regions[regionIndex];

            if (r.RegionRectangle.Width > 0 & r.RegionRectangle.Height > 0)
            {

                Bitmap temp = new Bitmap(r.RegionRectangle.Width, r.RegionRectangle.Height);
                Graphics regionLayer = Graphics.FromImage(temp);

                regionLayer.TranslateTransform(-r.RegionRectangle.X, -r.RegionRectangle.Y);

                Color c;

                if (DrawTransparent)
                    c = Color.FromArgb(125, colors[(regionIndex % colors.Length)]);
                else
                    c = colors[(regionIndex % colors.Length)];

                if (OnlyMask)
                    c = Color.Black;

                if (!OnlyMask & !OnlySkin)
                    DrawRegion(regionLayer, c, r);
                else if (!OnlyMask & OnlySkin)
                    DrawSkin(regionLayer, r);

                if (DrawRegionFrame)
                    DrawFrame(regionLayer, r);

                if (DrawPercent)
                    DrawPercentInfo(regionLayer, r);

                if (DrawCenter)
                    DrawCenterPoint(regionLayer, r);

                regionLayer.Transform.Reset();
                regionLayer.Save();

                if (DisplayBackground)
                {
                    //temp = LayerDrawer.DrawBackgroundUnsafe(backLayer, r.RegionRectangle);
                    temp = LayerDrawer.DrawBackground(backLayer, r.RegionRectangle);
                }

                return temp;
            }
            else
                return null;
        }

        public Bitmap DrawRegions()
        {            
            int index = 0;
            foreach (SRegion r in regions)
            {
                Color c;

                if (DrawTransparent)
                    c = Color.FromArgb(125, colors[(index % colors.Length)]);
                else
                    c = colors[(index % colors.Length)];

                if (OnlyMask)
                    c = Color.Black;

                if (!OnlySkin & DrawColorMask)
                    DrawRegion(frontLayer, c, r);
                else if (OnlySkin & DrawColorMask)
                    DrawSkin(frontLayer, r);

                if (DrawRegionFrame)
                    DrawFrame(frontLayer, r);

                if (DrawPercent)
                    DrawPercentInfo(frontLayer, r);

                if (DrawCenter)
                    DrawCenterPoint(frontLayer, r);

                index++;
            }

            Bitmap result = new Bitmap(this.size.Width, this.size.Height);
            Graphics graphics = Graphics.FromImage(result);
            if (DisplayBackground)
                graphics.DrawImage(backLayer, 0, 0);
            graphics.DrawImage(drawLayer, 0, 0);

            return result;
        }

        private Bitmap MergeLayers(Bitmap bottomLayer, Bitmap topLayer)
        {
            Bitmap result = new Bitmap(bottomLayer.Size.Width, bottomLayer.Size.Height);
            Graphics graphics = Graphics.FromImage(result);

            graphics.DrawImage(bottomLayer, 0, 0);
            graphics.DrawImage(topLayer, 0, 0);

            return result;
        }

        private void DrawSkin(Graphics layer, SRegion region)
        {
            LayerDrawer.DrawSkinUnsafe(layer, backLayer, region);
        }

        private void DrawCenterPoint(Graphics layer, SRegion region)
        {
            Point p = region.Center;
            Point p1 = new Point(p.X - 2, p.Y - 2);
            Point p2 = new Point(p.X + 2, p.Y + 2);
            Point p3 = new Point(p.X - 2, p.Y + 2);
            Point p4 = new Point(p.X + 2, p.Y - 2);
            Pen pen = Pens.LimeGreen;

            layer.DrawLine(pen, p1, p2);
            layer.DrawLine(pen, p3, p4);
        }

        private void DrawRegion(Graphics layer, Color color, SRegion region)
        {
            Brush brush = Brushes.LimeGreen;
            Brush regionBrush = new SolidBrush(color);

            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (region[x, y])
                    {
                        layer.FillRectangle(regionBrush, x, y, 1, 1);
                    }
                }
            }
        }

        private void DrawFrame(Graphics layer, SRegion region)
        {
            layer.DrawRectangle(Pens.LimeGreen, region.RegionRectangle);
        }

        private void DrawPercentInfo(Graphics layer, SRegion region)
        {
            layer.PageUnit = GraphicsUnit.Display;
            float percent = (float)region.Size / ((float)size.Width * (float)size.Height) * 100.0F;
            Font f = new Font("Arial", 10);
            Point p = region.Center;
            layer.DrawString(
                string.Format(
                    "Size: {0}% \r\nFill: {1}% \r\nBoxRate: {2}\r\n",
                    new string[]{
                    percent.ToString("0.00"),
                    region.FillPercent.ToString("0.00"),
                    region.BoxRate.ToString("0.00") 
                    }),
                f,
                Brushes.Black,
                new Point(p.X + 2, p.Y - 2));
        }
    }

    public class LayerDrawer
    {
        #region DrawSkin

        static Graphics drawSkin_layer;
        static Bitmap drawSkin_background;
        static SRegion drawSkin_region;

        public static void DrawSkinUnsafe(Graphics layer, Bitmap background, SRegion region)
        {
            drawSkin_background = background;
            drawSkin_layer = layer;
            drawSkin_region = region;

            background.UnsafeScan(new UnsafeScanHandler(UnsafeReadBackLayer), ImageLockMode.ReadOnly);
        }

        private static void UnsafeReadBackLayer(int x, int y, byte r, byte g, byte b)
        {
            if (drawSkin_region[x, y])
            {
                drawSkin_layer.DrawRectangle(new Pen(Color.FromArgb(r, g, b)), x, y, 1, 1);
            }
        }

        #endregion

        #region DrawBackground

        static Bitmap drawBackground_background;
        static Rectangle drawBackground_regionRectangle;
        static Bitmap tempback;

        public static Bitmap DrawBackgroundUnsafe(Bitmap background, Rectangle regionRectangle)
        {
            drawBackground_background = background;
            drawBackground_regionRectangle = regionRectangle;
            tempback = new Bitmap(regionRectangle.Width, regionRectangle.Height);

            background.UnsafeScan(new UnsafeScanHandler(DrawBackgroundUnsafeHandler), ImageLockMode.ReadOnly);

            return tempback;
        }

        private static void DrawBackgroundUnsafeHandler(int x, int y, byte r, byte g, byte b)
        {
            Color c = drawBackground_background.GetPixel(
                drawBackground_regionRectangle.X + x, 
                drawBackground_regionRectangle.Y + y);

            tempback.SetPixel(x, y, c);
        }

        public static Bitmap DrawBackground(Bitmap background, Rectangle regionRectangle)
        {
            Bitmap tempback = new Bitmap(regionRectangle.Width, regionRectangle.Height);

            for (int x = 0; x < regionRectangle.Width; x++)
            {
                for (int y = 0; y < regionRectangle.Height; y++)
                {
                    tempback.SetPixel(x, y, background.GetPixel(regionRectangle.X + x, regionRectangle.Y + y));
                }
            }

            return tempback;
        }

        #endregion
    }
}
