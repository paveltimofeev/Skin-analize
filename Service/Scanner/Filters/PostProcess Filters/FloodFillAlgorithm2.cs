using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Filters.RegionFilters
{
    public class FloodFillAlgorithm2
    {
        bool border = false;
        bool[,] regionMask;
        Size bounds;
        SRegion region;

        public FloodFillAlgorithm2(bool[,] regionMask, Rectangle regionRectangle)
        {
            this.regionMask = regionMask;
            bounds = regionRectangle.Size;
        }

        public void FloodFill(int x, int y, SRegion region)
        {
            this.region = region;
            ExecuteAlgorithm(x, y, new IsValidHandler(IsValid), new PointActionHandler(FillMask));
        }

        public void SetAction(int x, int y)
        {
            ExecuteAlgorithm(x, y, new IsValidHandler(IsValid), new PointActionHandler(FillMask));
        }

        int depth = 0;
        public int GetDepth(int x, int y)
        {
            depth = 0;
            ExecuteAlgorithm(x, y, new IsValidHandler(IsValid), new PointActionHandler(IncreaseDepth));

            return depth;
        }

        private void ExecuteAlgorithm(int x, int y, IsValidHandler isValid, PointActionHandler action)
        {
            ///Flood-Fill algorithm
            Queue<Point> q = new Queue<Point>();

            if (regionMask[x, y])
                return;

            q.Enqueue(new Point(x, y));

            while (q.Count > 0)
            {
                Point p = q.Peek();
                int x1 = p.X;
                int y1 = p.Y;

                if (isValid.Invoke(p))
                {
                    action.Invoke(p.X, p.Y);
                }

                if (isValid.Invoke(new Point(x1, y1 + 1)))
                {
                    action.Invoke(x1, y1 + 1);
                    q.Enqueue(new Point(x1, y1 + 1));
                }

                if (isValid.Invoke(new Point(x1, y1 - 1)))
                {
                    action.Invoke(x1, y1 - 1);
                    q.Enqueue(new Point(x1, y1 - 1));
                }

                if (isValid.Invoke(new Point(x1 + 1, y1)))
                {
                    action.Invoke(x1 + 1, y1);
                    q.Enqueue(new Point(x1 + 1, y1));
                }

                if (isValid.Invoke(new Point(x1 - 1, y1)))
                {
                    action.Invoke(x1 - 1, y1);
                    q.Enqueue(new Point(x1 - 1, y1));
                }

                q.Dequeue();

                if (border)
                {
                    q.Clear();
                    depth = Int32.MaxValue;
                }
            }
            ///end of Flood-Fill algorithm
        }

        private bool IsValid(Point p)
        {

            if (
                (
                p.X > 1 &
                p.Y > 1 &
                p.X < bounds.Width - 1 &
                p.Y < bounds.Height - 1
                )
                &&
                (
                !regionMask[p.X, p.Y]
                )
                )
                return true;
            else
                return false;
        }

        private void IncreaseDepth(int x, int y)
        {
            depth++;
            regionMask[x, y] = true;
        }

        private void FillMask(int x, int y)
        {
            region.AddPoint(x, y);
        }
    }
}
