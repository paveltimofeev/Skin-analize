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
    /// Morfological filter base.
    /// </summary>
    public class MorfologicalRegionFilterBase : RegionFilterBase
    {
        protected bool[,] structElement;
        protected Size structElementSize;

        protected void SetDefaultStructuralElement(StructuralElement element)
        {
            if (element == StructuralElement.RoundBox3)
            {
                structElement = new bool[3, 3];
                structElement[0, 0] = false;
                structElement[0, 1] = true;
                structElement[0, 2] = false;
                structElement[1, 0] = true;
                structElement[1, 1] = true;
                structElement[1, 2] = true;
                structElement[2, 0] = false;
                structElement[2, 1] = true;
                structElement[2, 2] = false;

                structElementSize = new Size(3, 3);
            }

            if (element == StructuralElement.Diamond5)
            {
                structElement = new bool[5, 5];
                structElement[0, 0] = false;
                structElement[0, 1] = false;
                structElement[0, 2] = true;
                structElement[0, 3] = false;
                structElement[0, 4] = false;

                structElement[1, 0] = false;
                structElement[1, 1] = true;
                structElement[1, 2] = true;
                structElement[1, 3] = true;
                structElement[1, 4] = false;

                structElement[2, 0] = true;
                structElement[2, 1] = true;
                structElement[2, 2] = true;
                structElement[2, 3] = true;
                structElement[2, 4] = true;

                structElement[3, 0] = false;
                structElement[3, 1] = true;
                structElement[3, 2] = true;
                structElement[3, 3] = true;
                structElement[3, 4] = false;

                structElement[4, 0] = false;
                structElement[4, 1] = false;
                structElement[4, 2] = true;
                structElement[4, 3] = false;
                structElement[4, 4] = false;

                structElementSize = new Size(5, 5);
            }

            if (element == StructuralElement.RoundBox5)
            {
                structElement = new bool[5, 5];
                structElement[0, 0] = false;
                structElement[0, 1] = true;
                structElement[0, 2] = true;
                structElement[0, 3] = true;
                structElement[0, 4] = false;

                structElement[1, 0] = true;
                structElement[1, 1] = true;
                structElement[1, 2] = true;
                structElement[1, 3] = true;
                structElement[1, 4] = true;

                structElement[2, 0] = true;
                structElement[2, 1] = true;
                structElement[2, 2] = true;
                structElement[2, 3] = true;
                structElement[2, 4] = true;

                structElement[3, 0] = true;
                structElement[3, 1] = true;
                structElement[3, 2] = true;
                structElement[3, 3] = true;
                structElement[3, 4] = true;

                structElement[4, 0] = false;
                structElement[4, 1] = true;
                structElement[4, 2] = true;
                structElement[4, 3] = true;
                structElement[4, 4] = false;

                structElementSize = new Size(5, 5);
            }

            if (element == StructuralElement.Circle7)
            {
                structElement = new bool[7, 7];
                structElement[0, 0] = false;
                structElement[0, 1] = false;
                structElement[0, 2] = true;
                structElement[0, 3] = true;
                structElement[0, 4] = true;
                structElement[0, 5] = false;
                structElement[0, 6] = false;

                structElement[1, 0] = false;
                structElement[1, 1] = true;
                structElement[1, 2] = true;
                structElement[1, 3] = true;
                structElement[1, 4] = true;
                structElement[1, 5] = true;
                structElement[1, 6] = false;

                structElement[2, 0] = true;
                structElement[2, 1] = true;
                structElement[2, 2] = true;
                structElement[2, 3] = true;
                structElement[2, 4] = true;
                structElement[2, 5] = true;
                structElement[2, 6] = true;

                structElement[3, 0] = true;
                structElement[3, 1] = true;
                structElement[3, 2] = true;
                structElement[3, 3] = true;
                structElement[3, 4] = true;
                structElement[3, 5] = true;
                structElement[3, 6] = true;

                structElement[4, 0] = true;
                structElement[4, 1] = true;
                structElement[4, 2] = true;
                structElement[4, 3] = true;
                structElement[4, 4] = true;
                structElement[4, 5] = true;
                structElement[4, 6] = true;

                structElement[5, 0] = false;
                structElement[5, 1] = true;
                structElement[5, 2] = true;
                structElement[5, 3] = true;
                structElement[5, 4] = true;
                structElement[5, 5] = true;
                structElement[5, 6] = false;

                structElement[6, 0] = false;
                structElement[6, 1] = false;
                structElement[6, 2] = true;
                structElement[6, 3] = true;
                structElement[6, 4] = true;
                structElement[6, 5] = false;
                structElement[6, 6] = false;

                structElementSize = new Size(7, 7);
            }
        }

        public void Apply(ref SRegion[] regions) { ;}

        public enum StructuralElement { RoundBox3, RoundBox5, Diamond5, Circle7 }
    }
}
