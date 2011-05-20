using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Drawers
{
    public class BrightlessDrawer : RegionDrawer
    {
        public BrightlessDrawer(Bitmap background, SRegion[] regions) : base(background, regions) { ;}

        protected override void DrawSkin(Graphics layer, SRegion region, int alfa)
        {
            Color c = Color.White;
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (region[x, y])
                    {
                        byte br = (byte)(backLayer.GetPixel(x, y).GetBrightness() * 255);
                        c = Color.FromArgb(br, br, br);
                        layer.DrawRectangle(new Pen(c), x, y, 1, 1);
                    }
                }
            }
        }
    }
}
