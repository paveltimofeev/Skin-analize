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
using Service.Trainee;

namespace Service.Scanner.Filters.RegionFilters
{
    [Obsolete]
    public class FloodFillAlgorithm
    {
        int[,] regionMask;
        Size bounds;
        Bitmap image;
        TraineeBase trainset;

        public FloodFillAlgorithm(int[,] regionMask, Bitmap image, TraineeBase trainset)
        {
            this.regionMask = regionMask;
            this.bounds = image.Size;
            this.image = image;
            this.trainset = trainset;
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

            if (regionMask[x, y] != 0)
                return;

            q.Enqueue(new Point(x, y));

            while (q.Count > 0)
            {
                Point p = q.Peek();
                int x1 = p.X;
                int y1 = p.Y;

                if (isValid.Invoke(p) & !trainset.Contains(image.GetPixel(x1, y1)))
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
                regionMask[p.X, p.Y] == 0
                )
                &&
                (!trainset.Contains(image.GetPixel(p.X, p.Y)))
                )
                return true;
            else
                return false;
        }

        private void IncreaseDepth(int x, int y)
        {
            depth++;
        }

        private void FillMask(int x, int y)
        {
            regionMask[x, y] = 1;
        }
    }
}
