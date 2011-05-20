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
using Service.Scanner.Filters.PreProcessFilters;
using Service.Scanner.Filters.RegionFilters;
using Service.Trainee;

namespace Service.Scanner.Labelers
{
    public class FloodFillLabeler : ImageScanner, ILabeler
    {
        private int[,] regionMask = null;
        private SRegion[] regions = null;
        public SRegion[] Regions { get { return regions; } }

        public FloodFillLabeler(ITrainee trainee, Bitmap image) : base(trainee, image) { ;}

        public override void Execute()
        {
            foreach (IPreProcessFilter filter in preProcessFilters)
            {
                filter.Apply(base.Image.Size, this.regionMask);
            }

            int regionIndex = 1;
            regionMask = new int[base.Image.Width, base.Image.Height];
            for (int x = 0; x < base.Image.Width; x++)
            {
                for (int y = 0; y < base.Image.Height; y++)
                {
                    if (regionMask[x, y] == 0 & learned.Contains(base.Image.GetPixel(x, y)))
                    {
                        ///Flood-Fill algorithm
                        Queue<Point> q = new Queue<Point>();

                        if (regionMask[x, y] != 0)
                            continue;

                        regionIndex++;

                        q.Enqueue(new Point(x, y));

                        while (q.Count > 0)
                        {
                            Point p = q.Peek();
                            int x1 = p.X;
                            int y1 = p.Y;

                            if (IsValid(p) & learned.Contains(base.Image.GetPixel(x1, y1)))
                            {
                                regionMask[x1, y1] = regionIndex;
                            }

                            if (IsValid(new Point(x1, y1 + 1)))
                            {
                                regionMask[x1, y1 + 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 + 1));
                            }

                            if (IsValid(new Point(x1, y1 - 1)))
                            {
                                regionMask[x1, y1 - 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 - 1));
                            }

                            if (IsValid(new Point(x1 + 1, y1)))
                            {
                                regionMask[x1 + 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 + 1, y1));
                            }

                            if (IsValid(new Point(x1 - 1, y1)))
                            {
                                regionMask[x1 - 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 - 1, y1));
                            }

                            q.Dequeue();
                        }
                        ///end of Flood-Fill algorithm
                    }
                }
            }

            ///TODO: Check this.
            ///scan regionsMask pixel-by-pixel and forming regions groups
            Dictionary<int, SRegion> regionDictionary = new Dictionary<int, SRegion>();
            for (int x = 0; x < base.Image.Size.Width; x++)
            {
                for (int y = 0; y < base.Image.Size.Height; y++)
                {
                    int regionId = regionMask[x, y];
                    if (regionId > 0)
                    {
                        if (regionDictionary.ContainsKey(regionId))
                        {
                            regionDictionary[regionId].AddPoint(x, y);
                        }
                        else
                        {
                            //SRegion region = new SRegion(base.Image.Size);
                            SRegion region = new SRegion(ref regionMask, regionIndex, base.Image.Size);
                            region.AddPoint(x, y);
                            regionDictionary.Add(regionId, region);
                        }
                    }
                }
            }

            regions = new SRegion[regionDictionary.Count];
            int index = 0;
            foreach (SRegion r in regionDictionary.Values)
            {
                regions[index] = r;
                index++;
            }

            foreach (IPostProcessFilter filter in regionFilters)
            {
                filter.Apply(ref regions);
            }

            foreach (IPostProcessFilter filter in regionFilters)
            {
                foreach (SRegion region in regions)
                {
                    filter.Apply(region);
                }
            }
        }

        protected bool IsValid(Point p)
        {

            if (
                (
                p.X > 1 &
                p.Y > 1 &
                p.X < base.Image.Size.Width - 1 &
                p.Y < base.Image.Size.Height - 1
                )
                &&
                (
                regionMask[p.X, p.Y] == 0
                )
                &&
                (
                learned.Contains(base.Image.GetPixel(p.X, p.Y))
                )
                )
                return true;
            else
                return false;
        }

        #region Filters

        protected List<IPostProcessFilter> regionFilters = new List<IPostProcessFilter>();
        public void AddFilter(IPostProcessFilter filter)
        {
            regionFilters.Add(filter);
        }
        public void RemoveFilter(IPostProcessFilter filter)
        {
            regionFilters.Remove(filter);
        }
        public bool ContainsFilter(IPostProcessFilter filter)
        {
            return regionFilters.Contains(filter);
        }

        protected List<IPreProcessFilter> preProcessFilters = new List<IPreProcessFilter>();
        public void AddFilter(IPreProcessFilter filter)
        {
            preProcessFilters.Add(filter);
        }
        public void RemoveFilter(IPreProcessFilter filter)
        {
            preProcessFilters.Remove(filter);
        }
        public bool ContainsFilter(IPreProcessFilter filter)
        {
            return preProcessFilters.Contains(filter);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
