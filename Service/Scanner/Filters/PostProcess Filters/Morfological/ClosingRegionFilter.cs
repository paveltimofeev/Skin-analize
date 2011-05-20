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
    /// Morfological Closing filter.
    /// </summary>
    public class ClosingRegionFilter : MorfologicalRegionFilterBase, IPostProcessFilter
    {
        public ClosingRegionFilter(bool[,] structElement, Size structElementSize) { this.structElement = structElement; this.structElementSize = structElementSize; }

        public ClosingRegionFilter(StructuralElement element)
        {
            SetDefaultStructuralElement(element);
        }

        public void Apply(SRegion region)
        {
            DilationRegionFilter dilation = new DilationRegionFilter(structElement, structElementSize);
            dilation.Apply(region);

            ErosionRegionFilter erosion = new ErosionRegionFilter(structElement, structElementSize);
            erosion.Apply(region);
        }
    }
}
