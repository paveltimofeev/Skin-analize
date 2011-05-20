using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Drawers
{
    public class InterestPointDrawer: RegionDrawer
    {
        Pen pen;
        Brush brush;
        const int rad = 2;

        public InterestPointDrawer(Bitmap background, SRegion[] regions) : base(background, regions) 
        {
            Color c = Color.FromArgb(125, Color.Red);
            pen = new Pen(c);
            brush = new SolidBrush(c);
        }

        protected override void DrawRegion(int index, SRegion r, Graphics regionLayer)
        {
            base.DrawRegion(index, r, regionLayer);

            Rectangle frame = new Rectangle(new Point(0, 0), base.Background.Size);
            for (int x = frame.Left + rad + 1; x < frame.Size.Width - rad - 1; x++)
            {
                for (int y = frame.Top + rad + 1; y < frame.Size.Height - rad - 1; y++)
                {
                    bool ab = r[x + rad, y + rad];
                    bool bc = r[x + rad, y - rad];
                    bool cd = r[x - rad, y - rad];
                    bool da = r[x - rad, y + rad];

                    bool a = true, b = true, c = true, d = true;
                    int p = 0;

                    while (p < rad)
                    {
                        a &= r[x, y + p];
                        b &= r[x + p, y];
                        c &= r[x, y - p];
                        d &= r[x - p, y];
                        p++;
                    }

                    if (
                        (a & b & ab & !c & !d & !bc & !cd & !da) |
                        (b & c & bc & !a & !d & !ab & !cd & !da) |
                        (c & d & cd & !a & !b & !ab & !bc & !da) |
                        (a & d & da & !b & !c & !ab & !bc & !cd)
                        )
                    {
                        this.DrawCircle(regionLayer, new Point(x, y), rad, pen);
                    }
                }
            }
        }
    }
}
