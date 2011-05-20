using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Scanner.Filters.RegionFilters;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Filters.PostProcess_Filters
{
    /// <summary>
    /// Filter for remove noise (small regions) from image.
    /// </summary>
    public class RegionSizePostProcessFilter : RegionFilterBase, IPostProcessFilter
    {
        float minMargin = 0;
        float maxMargin = Int32.MaxValue;

        public RegionSizePostProcessFilter(float minMargin, float maxMargin)
        {
            this.minMargin = minMargin;
            this.maxMargin = maxMargin;
        }

        #region IPostProcessFilter Members

        public void Apply(SRegion region) { ;}

        public void Apply(ref SRegion[] regions)
        {
            float count = (float)base.Count(regions);
            List<SRegion> temp = new List<SRegion>();

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
                    if (size > this.minMargin & size < this.maxMargin)
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
