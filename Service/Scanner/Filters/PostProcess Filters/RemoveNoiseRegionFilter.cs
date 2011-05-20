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
    /// <summary>
    /// Filter for remove noise (small regions) from image.
    /// </summary>
    public class RemoveNoiseRegionFilter : RegionFilterBase, IPostProcessFilter
    {
        public bool UnsafeMode { get; set; }

        /// <summary>
        /// Size of the noise to remove (in percent).
        /// </summary>
        public float NoisePercentSize { get; set; }

        #region Constructors

        /// <summary>
        /// Filter for remove noise from image with default value of 'NoisePercentSize'=0.01F.
        /// </summary>
        public RemoveNoiseRegionFilter(float noisePercentSize) : this(noisePercentSize, true) { ;}

        /// <summary>
        /// Filter for remove noise from image with default value of 'NoisePercentSize'=0.01F.
        /// </summary>
        public RemoveNoiseRegionFilter() : this(0.01F, true) { ;}

        /// <summary>
        /// Filter for remove noise from image with default value of 'NoisePercentSize'=0.01F.
        /// </summary>
        public RemoveNoiseRegionFilter(bool unsafeMode) : this(0.01F, unsafeMode) { ;}

        /// <summary>
        /// Filter for remove noise from image.
        /// </summary>
        /// <param name="noisePercentSize">0.000F - 1.000F</param>
        public RemoveNoiseRegionFilter(float noisePercentSize, bool unsafeMode)
        {
            if (noisePercentSize > 1F | noisePercentSize < 0F)
                throw new ArgumentOutOfRangeException("noisePercentSize", "Must be between 0 and 1.");
            this.NoisePercentSize = noisePercentSize;

            this.UnsafeMode = unsafeMode;
        }

        #endregion

        #region IRegionFilter Members

        public void Apply(SRegion region) { ;}

        public virtual void Apply(ref SRegion[] regions)
        {
            float count = (float)base.Count(regions);
            List<SRegion> temp = new List<SRegion>();

            if (UnsafeMode)
            {
                unsafe
                {
                    int* max = (int*)regions.Length;
                    int* min = (int*)0;
                    int incr = 1;
                    for (int* i = min; i < max; i = (int*)((int)i + incr))
                    {
                        int id = (int)i;
                        SRegion region = regions[id];

                        float size = (float)region.Size / count;
                        if (size > this.NoisePercentSize)
                            temp.Add(region);
                    }
                }
            }
            else
            {
            foreach (SRegion region in regions)
            {
                float size = (float)region.Size / count;
                if (size > this.NoisePercentSize)
                    temp.Add(region);
            }
            }

            SRegion[] result = new SRegion[temp.Count];
            temp.CopyTo(result);
            regions = result;
        }

        #endregion
    }
}
