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
using System.Diagnostics;

namespace Service.Scanner.Labelers
{
    /// <summary>
    /// FloodFillLabeler uses FloodFill algorithm to detect skin areas and connect them to regions.
    /// </summary>
    public class FloodFillLabelerUnsafe : ImageScanner, ILabeler
    { 
        /// <summary>
        /// Rarity. разреженность floodfill-сканирования (1 - сканировать всё, 2 - через строку, 3 - через две строки и т.д.)
        /// </summary>
        readonly int rarity = 1; 
        
        /// <summary>
        /// Binary image.
        /// </summary>
        protected int[,] binaryMask;

        protected SRegion[] regions = null;

        /// <summary>
        /// Araay of detected skin regions
        /// </summary>
        public SRegion[] Regions { get { return regions; } }

        public FloodFillLabelerUnsafe(ITrainee learned, Bitmap image, int rarity) : base(learned, image) { this.rarity = rarity;}

        /// <summary>
        /// Run scanning
        /// </summary>
        public override void Execute()
        {
            this.binaryMask = new int[this.Image.Size.Width, this.Image.Size.Height];

            ///Quick binarization via unsafe pixel-by-pixel scan
            /// 0 is non-skin pixel
            ///-1 is skin pixel
            base.Image.UnsafeScan(new UnsafeScanHandler(MarkPoint), ImageLockMode.ReadOnly);

            ///Apply pre process filters (Runs before labelling, but ufter binarization)
            this.RunPreProcessFilters();

            ///Labelling via Flood-Fill algorithm and Create Region Collection
            this.FloodFillLabellingRerefied();

            ///Apply post process filters
            RunPostProcessFilters();
        }

        /// <summary>
        /// Labelling and Create Region Collection in one method
        /// </summary>
        protected virtual void FloodFillLabellingRerefied()
        {
            Dictionary<int, SRegion> regs = new Dictionary<int, SRegion>();
           
            int regionIndex = 1;
            for (int x = 0; x < this.Image.Size.Width - rarity; x += rarity)
            {
                for (int y = 0; y < this.Image.Size.Height - rarity; y += rarity)
                {
                    if (binaryMask[x, y] == -1)
                    {
                        ///Flood-Fill algorithm
                        Queue<Point> q = new Queue<Point>();

                        if (binaryMask[x, y] != -1)
                            continue;

                        regionIndex++;

                        q.Enqueue(new Point(x, y));

                        while (q.Count > 0)
                        {
                            Point p = q.Peek();
                            int x1 = p.X;
                            int y1 = p.Y;

                            if (IsValid(p))
                            {
                                binaryMask[x1, y1] = regionIndex;

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1, y1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1, y1 + 1)))
                            {
                                binaryMask[x1, y1 + 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 + 1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1, y1 + 1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1, y1 - 1)))
                            {
                                binaryMask[x1, y1 - 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 - 1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1, y1 - 1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1 + 1, y1)))
                            {
                                binaryMask[x1 + 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 + 1, y1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1 + 1, y1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            if (IsValid(new Point(x1 - 1, y1)))
                            {
                                binaryMask[x1 - 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 - 1, y1));

                                if (regs.ContainsKey(regionIndex))
                                    regs[regionIndex].AddPoint(x1 - 1, y1);
                                else
                                    regs.Add(regionIndex, new SRegion(ref binaryMask, regionIndex, Image.Size));
                            }

                            q.Dequeue();
                        }
                        ///end of Flood-Fill algorithm
                    }
                }
            }

            regions = new SRegion[regs.Count];
            int index = 0;
            foreach (SRegion r in regs.Values)
            {
                regions[index] = r;
                index++;
            }
        }

        ///Quick binarization via unsafe image scan
        /// 0 is non-skin pixel
        ///-1 is skin pixel
        protected virtual void MarkPoint(int x, int y, byte r, byte g, byte b)
        {
            if (learned.Contains(r, g, b))
                binaryMask[x, y] = -1; //-1 is skin pixel, 0 is not skin
        }

        /// <summary>
        /// Check validity of point location at the image.
        /// </summary>
        /// <param name="p">Point</param>
        protected virtual bool IsValid(Point p)
        {
            if (
                (
                p.X > 1 &
                p.Y > 1 &
                p.X < Image.Size.Width - 1 &
                p.Y < Image.Size.Height - 1
                )
                &&
                binaryMask[p.X, p.Y] == -1
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
        
        protected void RunPreProcessFilters()
        {
            foreach (IPreProcessFilter filter in preProcessFilters)
            {
                filter.Apply(this.Image.Size, this.binaryMask);
            }
        }

        protected void RunPostProcessFilters()
        {
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

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Obsolete methods

        /// <summary>
        /// Labelling
        /// </summary>
        [Obsolete("Too low method, use FloodFillLabellingRerefied instead of FloodFillLabelling and CreateRegionCollection methods.", true)]
        private int FloodFillLabelling()
        {
            int rarity = 10; //разреженность сканирования (1 - сканировать всё, 2 - через строку, 3 - через две строки и т.д.)

            int regionIndex = 1;
            for (int x = 0; x < this.Image.Size.Width - rarity; x += rarity)
            {
                for (int y = 0; y < this.Image.Size.Height - rarity; y += rarity)
                {
                    if (binaryMask[x, y] == -1)
                    {
                        ///Flood-Fill algorithm
                        Queue<Point> q = new Queue<Point>();

                        if (binaryMask[x, y] != -1)
                            continue;

                        regionIndex++;

                        q.Enqueue(new Point(x, y));

                        while (q.Count > 0)
                        {
                            Point p = q.Peek();
                            int x1 = p.X;
                            int y1 = p.Y;

                            if (IsValid(p))
                            {
                                binaryMask[x1, y1] = regionIndex;
                            }

                            if (IsValid(new Point(x1, y1 + 1)))
                            {
                                binaryMask[x1, y1 + 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 + 1));
                            }

                            if (IsValid(new Point(x1, y1 - 1)))
                            {
                                binaryMask[x1, y1 - 1] = regionIndex;
                                q.Enqueue(new Point(x1, y1 - 1));
                            }

                            if (IsValid(new Point(x1 + 1, y1)))
                            {
                                binaryMask[x1 + 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 + 1, y1));
                            }

                            if (IsValid(new Point(x1 - 1, y1)))
                            {
                                binaryMask[x1 - 1, y1] = regionIndex;
                                q.Enqueue(new Point(x1 - 1, y1));
                            }

                            q.Dequeue();
                        }
                        ///end of Flood-Fill algorithm
                    }
                }
            }

            return regionIndex;
        }

        /// <summary>
        /// Fill arrays of refions
        /// </summary>
        [Obsolete("Too low method, use FloodFillLabellingRerefied instead of FloodFillLabelling and CreateRegionCollection methods.", true)]
        private unsafe void CreateRegionCollection()
        {
            ///Create regions collection
            Dictionary<int, SRegion> regionDictionary = new Dictionary<int, SRegion>();
            unchecked
            {
                for (int x = 0; x < Image.Size.Width; x++)
                {
                    for (int y = 0; y < Image.Size.Height; y++)
                    {
                        int regionId = binaryMask[x, y];
                        if (regionId > 0)
                        {
                            if (regionDictionary.ContainsKey(regionId))
                            {
                                regionDictionary[regionId].AddPoint(x, y);
                            }
                            else
                            {
                                SRegion region = new SRegion(ref binaryMask, regionId, base.Image.Size);
                                region.AddPoint(x, y);
                                regionDictionary.Add(regionId, region);
                            }
                        }
                    }
                }
            }

            ///Convert region connection to array of regions
            regions = new SRegion[regionDictionary.Count];
            int index = 0;
            foreach (SRegion r in regionDictionary.Values)
            {
                regions[index] = r;
                index++;
            }
        }

        #endregion
    }
}
