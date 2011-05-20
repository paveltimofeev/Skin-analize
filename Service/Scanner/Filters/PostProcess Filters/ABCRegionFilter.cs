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
    /// Filter for slpit regions for A, B and C categories.
    /// </summary>
    public class ABCRegionFilter : RegionFilterBase, IPostProcessFilter
    {
        #region IRegionFilter Members

        public void Apply(SRegion region) { ;}

        public void Apply(ref SRegion[] regions)
        {
            List<SRegion> a = new List<SRegion>();
            List<SRegion> b = new List<SRegion>();
            List<SRegion> c = new List<SRegion>();

            TotalRegionsSize = base.Count(regions);

            foreach (SRegion region in regions)
            {
                float sizePercent = (float)region.Size / (float)TotalRegionsSize * 100F;
                if (sizePercent > 80)
                {
                    a.Add(region);
                }
                else if (sizePercent > 60)
                {
                    b.Add(region);
                }
                else
                {
                    c.Add(region);
                }
            }

            A = new SRegion[a.Count];
            B = new SRegion[b.Count];
            C = new SRegion[c.Count];

            a.CopyTo(A);
            b.CopyTo(B);
            c.CopyTo(C);
        }

        public int TotalRegionsSize { get; private set; }
        public SRegion[] A { get; private set; }
        public SRegion[] B { get; private set; }
        public SRegion[] C { get; private set; }

        #endregion
    }
}
