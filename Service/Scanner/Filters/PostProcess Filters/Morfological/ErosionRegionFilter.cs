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
    /// Morfological Erosion filter.
    /// </summary>
    public class ErosionRegionFilter : MorfologicalRegionFilterBase, IPostProcessFilter
    {
        public ErosionRegionFilter(bool[,] structElement, Size structElementSize) { this.structElement = structElement; this.structElementSize = structElementSize; }

        public ErosionRegionFilter(StructuralElement element)
        {
            SetDefaultStructuralElement(element);
        }

        public void Apply(SRegion region)
        {
            Rectangle bounds = region.RegionRectangle;

            unsafe
            {
                int* dilationCenterX =  (int*)((structElementSize.Width - 1) / 2);
                int* dilationCenterY =  (int*)((structElementSize.Height - 1) / 2);
                int* dilationMaxX =     (int*)(structElementSize.Width - 1);
                int* dilationMaxY =     (int*)(structElementSize.Height - 1);

                int* x_min = (int*)bounds.X;
                int* x_max = (int*)(bounds.X + bounds.Width - (int)dilationMaxX);
                int* y_min = (int*)bounds.Y;
                int* y_max = (int*)(bounds.Y + bounds.Height - (int)dilationMaxY);

                for (int* x = x_min; x < x_max; x++)
                {
                    for (int* y = y_min; y < y_max; y++)
                    {
                        bool bitVal = region[(int)x, (int)y];

                        for (int* sX = (int*)0; sX < dilationMaxX; sX++)
                        {
                            for (int* sY = (int*)0; sY < dilationMaxY; sY++)
                            {
                                if (structElement[(int)sX, (int)sY] &
                                    region[(int)x + (int)sX, (int)y + (int)sY] &
                                    !region[(int)x + (int)dilationCenterX, (int)y + (int)dilationCenterX])
                                {
                                    region.RemovePoint((int)x + (int)sX, (int)y + (int)sY);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
