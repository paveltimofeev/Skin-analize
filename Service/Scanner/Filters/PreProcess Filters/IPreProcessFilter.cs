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

namespace Service.Scanner.Filters.PreProcessFilters
{
    public interface IPreProcessFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskSize"></param>
        /// <param name="dataMask">-1 is skin pixel, 0 is not skin pixel</param>
        void Apply(Size maskSize, int[,] dataMask);
    }
}
