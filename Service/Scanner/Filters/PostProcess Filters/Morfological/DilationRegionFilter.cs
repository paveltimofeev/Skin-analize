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
    /// Morfological Dilation filter.
    /// </summary>
    public class DilationRegionFilter : MorfologicalRegionFilterBase, IPostProcessFilter
    {
        public DilationRegionFilter(bool[,] structElement, Size structElementSize) { this.structElement = structElement; this.structElementSize = structElementSize; }

        public DilationRegionFilter(StructuralElement element)
        {
            SetDefaultStructuralElement(element);
        }

        #region IRegionFilter Members

        public void Apply(SRegion region)
        {
            Rectangle bounds = region.RegionRectangle;       
            unsafe
            {
                int dilationCenterX =   (structElementSize.Width - 1) / 2;
                int dilationCenterY =   (structElementSize.Height - 1) / 2;
                int dilationMaxX =      structElementSize.Width - 1;
                int dilationMaxY =      structElementSize.Height - 1;

                int* _dilationCenterX = (int*)dilationCenterX;
                int* _dilationCenterY = (int*)dilationCenterY;
                int* _dilationMaxX = (int*)dilationMaxX;
                int* _dilationMaxY = (int*)dilationMaxX;

                int* x_min = (int*)bounds.X;
                int* x_max = (int*)(bounds.X + bounds.Width - (int)_dilationCenterX);
                int* y_min = (int*)bounds.Y;
                int* y_max = (int*)(bounds.Y + bounds.Height - (int)_dilationCenterY);

                for (int* x = x_min; x < x_max; x++)
                {
                    for (int* y = y_min; y < y_max; y++)
                    {
                        bool bitVal = region[(int)x, (int)y];

                        for (int* sX = (int*)0; sX < _dilationMaxX; sX++)
                        {
                            for (int* sY = (int*)0; sY < _dilationMaxY; sY++)
                            {
                                if (structElement[(int)sX, (int)sY] & region[(int)x + (int)sX, (int)y + (int)sY])
                                {
                                    region.AddPoint((int)x + dilationCenterX, (int)y + dilationCenterY);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
