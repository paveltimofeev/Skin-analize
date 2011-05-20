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
    public abstract class RegionFilterBase
    {
        public int Count(SRegion[] regions)
        {
            int count = 0;

            foreach (SRegion region in regions) 
            {
                count += region.Size; 
            }


            return count;
        }
    }
}
