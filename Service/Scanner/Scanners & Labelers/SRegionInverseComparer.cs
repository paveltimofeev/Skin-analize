using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.ScannersAndDataMaskBuilders
{
    public class SRegionInverseComparer : IComparer<SRegion>
    {
        #region IComparer<SRegion> Members

        public int Compare(SRegion x, SRegion y)
        {
            return (x.Size == y.Size) ? 0 : (x.Size > y.Size) ? 0 : 1;
        }

        #endregion
    }
}
