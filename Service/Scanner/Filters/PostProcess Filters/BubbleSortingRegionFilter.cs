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
    /// Filter for sorting regions with bubble algorithm.
    /// </summary>
    public class BubbleSortingRegionFilter : RegionFilterBase, IPostProcessFilter
    {
        #region IRegionFilter Members

        public void Apply(SRegion region) { ;}

        public void Apply(ref SRegion[] regions)
        {
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < regions.Length - 1; i++)
                {
                    if (regions[i].Size < regions[i + 1].Size)
                    {
                        sorted = false;
                        SRegion temp = regions[i];
                        regions[i] = regions[i + 1];
                        regions[i + 1] = temp;
                    }
                }
            }
        }

        #endregion
    }
}
