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
    public class RegionDrawer : IDrawer
    {
        protected Bitmap backLayer;
        protected Bitmap drawLayer;
        public Bitmap Background { get; set; }

        protected Graphics frontLayer;
        protected Size size;
        protected SRegion[] regions;
        protected Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.Green, Color.BlueViolet, Color.Coral, Color.Navy, Color.Cyan, Color.DarkRed, Color.Gold, Color.Lime, Color.Maroon, Color.Orange, Color.RoyalBlue, Color.Magenta, Color.Purple, Color.Silver, Color.Violet };

        public int SkinLayerTransparent { get; set; }
        public SkinLayerMode SkinLayer { get; set; }
        public InfoLayerModes InfoLayers { get; set; }
        public BackLayerMode BackLayer { get; set; }

        public RegionDrawer(Bitmap background, List<SRegion> regions)
        {
            SRegion[] regionsArray = new SRegion[regions.Count];
            regions.CopyTo(regionsArray);

            this.regions = regionsArray;
            this.Background = background;

            Initialize(regionsArray);
        }

        public RegionDrawer(Bitmap background, SRegion[] regions)
        {
            this.Background = background;
            Initialize(regions);
        }

        protected void Initialize(SRegion[] regions)
        {
            this.SkinLayerTransparent = 255;
            this.size = Background.Size;
            this.backLayer = new Bitmap(Background, size.Width, size.Height);
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

            if (BackLayer == BackLayerMode.BACKGROUNDIMAGE)
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
                this.DrawRegion(regionIndex, r, regionLayer);
                regionLayer.Transform.Reset();
                regionLayer.Save();

                if (BackLayer == BackLayerMode.BACKGROUNDIMAGE)
                {
                    Bitmap tempback = new Bitmap(r.RegionRectangle.Width, r.RegionRectangle.Height);

                    for (int x = 0; x < r.RegionRectangle.Width; x++)
                    {
                        for (int y = 0; y < r.RegionRectangle.Height; y++)
                        {
                            tempback.SetPixel(x, y, backLayer.GetPixel(r.RegionRectangle.X + x, r.RegionRectangle.Y + y));
                        }
                    }

                    temp = tempback;
                }
                return temp;
            }
            else
                throw new Exception(string.Format("Incredible small region (size: {0}x{1})", r.RegionRectangle.Width, r.RegionRectangle.Height));

        }

        public Bitmap DrawRegions()
        {            
            int index = 0;
            if (regions != null)
            {
                foreach (SRegion r in regions)
                {
                    DrawRegion(index, r, frontLayer);

                    index++;
                }
            }

            Bitmap result = new Bitmap(this.size.Width, this.size.Height);
            Graphics graphics = Graphics.FromImage(result);
            if (BackLayer == BackLayerMode.BACKGROUNDIMAGE)
                graphics.DrawImage(backLayer, 0, 0);
            graphics.DrawImage(drawLayer, 0, 0);

            return result;
        }

        protected virtual void DrawRegion(int index, SRegion r, Graphics regionLayer)
        {
            switch (SkinLayer)
            {
                case SkinLayerMode.SKIN:
                    DrawSkin(regionLayer, r, SkinLayerTransparent);
                    break;

                case SkinLayerMode.COLORMASK:
                    DrawRegion(regionLayer, colors[(index % colors.Length)], SkinLayerTransparent, r);
                    break;

                case SkinLayerMode.BLACKMASK:
                    DrawRegion(regionLayer, Color.Black, SkinLayerTransparent, r);
                    break;
            }

            if ((InfoLayers & InfoLayerModes.CENTER) == InfoLayerModes.CENTER)
                DrawCenterPoint(regionLayer, r);

            if ((InfoLayers & InfoLayerModes.FRAME) == InfoLayerModes.FRAME)
                DrawFrame(regionLayer, r);

            if ((InfoLayers & InfoLayerModes.INFORMATION) == InfoLayerModes.INFORMATION)
                DrawInformation(regionLayer, r);
        }

        private Bitmap MergeLayers(Bitmap bottomLayer, Bitmap topLayer)
        {
            Bitmap result = new Bitmap(bottomLayer.Size.Width, bottomLayer.Size.Height);
            Graphics graphics = Graphics.FromImage(result);

            graphics.DrawImage(bottomLayer, 0, 0);
            graphics.DrawImage(topLayer, 0, 0);

            return result;
        }

        protected virtual void DrawSkin(Graphics layer, SRegion region, int alfa)
        {
            Color c = Color.White;
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (region[x, y])
                    {
                        c = backLayer.GetPixel(x, y);

                        if (alfa != 255)
                            c = Color.FromArgb(alfa, c.R, c.G, c.B);

                        layer.DrawRectangle(new Pen(c), x, y, 1, 1);
                    }
                }
            }
        }

        protected virtual void DrawCenterPoint(Graphics layer, SRegion region)
        {
            DrawPoint(layer, region.Center, region.Orentation, Pens.LimeGreen);
        }

        protected virtual void DrawPoint(Graphics layer, Point point, Pen pen)
        {
            DrawPoint(layer, point, 45, pen);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="point"></param>
        /// <param name="orentation">0-360</param>
        /// <param name="pen"></param>
        protected virtual void DrawPoint(Graphics layer, Point point, float orentation, Pen pen)
        {
            int size = 10;

            Point p1 = new Point(-size, 0);
            Point p2 = new Point(+size, 0);
            Point p3 = new Point(0, +size);
            Point p4 = new Point(0, -size);

            Point p5 = new Point(-size / 2, -size / 2);
            Point p6 = new Point(size / 2, -size / 2);

            layer.TranslateTransform(point.X, point.Y);
            layer.RotateTransform(orentation);
            layer.DrawLines(pen, new Point[] { p5, p4, p6 });
            layer.DrawLine(pen, p1, p2);
            layer.DrawLine(pen, p3, p4);
            layer.ResetTransform();
        }

        protected virtual void DrawCircle(Graphics layer, Point point, int radius, Pen pen)
        {
            Rectangle circle = new Rectangle(new Point(point.X - radius, point.Y - radius), new Size(radius * 2, radius * 2));
            layer.DrawEllipse(pen, circle);
        }

        protected virtual void DrawCircle(Graphics layer, Point point, int radius, Brush brush)
        {
            Rectangle circle = new Rectangle(new Point(point.X - radius, point.Y - radius), new Size(radius * 2, radius * 2));
            layer.FillEllipse(brush, circle);
        }

        protected virtual void DrawCircle(Graphics layer, Point point, Pen pen)
        {
            Rectangle circle = new Rectangle(new Point(point.X, point.Y), new Size(1, 1));
            layer.DrawEllipse(pen, circle);
        }

        protected virtual void DrawRegion(Graphics layer, Color color, int alfa, SRegion region)
        {
            if (alfa != 255)
                color = Color.FromArgb(alfa, color.R, color.G, color.B);

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

        protected virtual void DrawFrame(Graphics layer, SRegion region)
        {
            layer.DrawRectangle(Pens.LimeGreen, region.RegionRectangle);

            if ((InfoLayers & InfoLayerModes.REGIONNUMBER) == InfoLayerModes.REGIONNUMBER)
            {
                layer.PageUnit = GraphicsUnit.Display;
                Font f = new Font("Arial", 8.0f);
                string s = region.RegionIndex.ToString();
                RectangleF r = new RectangleF(region.RegionRectangle.Location, layer.MeasureString(s, f));
                
                layer.FillRectangle(Brushes.LimeGreen, r);
                layer.DrawString(s, f, Brushes.White, r);
            }
        }

        protected virtual void DrawInformation(Graphics layer, SRegion region)
        {
            layer.PageUnit = GraphicsUnit.Display;
            Font f = new Font("Arial", 8);
            Point p = region.Center;

            float percent = (float)region.Size / ((float)size.Width * (float)size.Height) * 100.0F;

            StringBuilder str = new StringBuilder();

            if ((InfoLayers & InfoLayerModes.SIZE) == InfoLayerModes.SIZE)
            {
                str.AppendFormat("Size: {0}%\r\n", percent.ToString("0.00"));
                str.AppendFormat("Size: {0}pts\r\n", region.Size);
            }

            if ((InfoLayers & InfoLayerModes.FILLPERCENT) == InfoLayerModes.FILLPERCENT)
                str.AppendFormat("Fill Percent: {0}\r\n", region.FillPercent.ToString("0.00"));
            if ((InfoLayers & InfoLayerModes.BOXRATE) == InfoLayerModes.BOXRATE)
                str.AppendFormat("Box Rate: {0}\r\n", region.BoxRate.ToString("0.00"));
            if ((InfoLayers & InfoLayerModes.COMPACTNESS) == InfoLayerModes.COMPACTNESS)
                str.AppendFormat("Compactness: {0}\r\n", region.Compactness.ToString("0.00"));
            if ((InfoLayers & InfoLayerModes.ORENTATION) == InfoLayerModes.ORENTATION)
                str.AppendFormat("Orentation: {0}\r\n", region.Orentation.ToString("0.00"));
            if ((InfoLayers & InfoLayerModes.ENLONGATION) == InfoLayerModes.ENLONGATION)
                str.AppendFormat("Enlongation: {0}\r\n", region.Enlongation.ToString("0.00"));
            if ((InfoLayers & InfoLayerModes.EDGELENGHT) == InfoLayerModes.EDGELENGHT)
                str.AppendFormat("Edge Length: {0}\r\n", region.EdgeLength.ToString("0.00"));

            layer.DrawString(
                str.ToString(),
                f,
                Brushes.Blue,
                new Point(p.X + 2, p.Y - 2));
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.backLayer.Dispose();
            this.drawLayer.Dispose();
            this.frontLayer.Dispose();
        }

        #endregion
    }
}
