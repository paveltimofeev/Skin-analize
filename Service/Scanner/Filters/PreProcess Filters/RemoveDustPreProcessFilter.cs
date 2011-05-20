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
    /// <summary>
    /// 8-ми связанная фитьтрация мелкого шума.
    /// </summary>
    public class RemoveDustPreProcessFilter : IPreProcessFilter
    {
        #region IScanFilter Members

        /// <summary>
        /// Will remove 1x1 regions which not connected with others.
        /// </summary>
        public void Apply(Size maskSize, int[,] dataMask)
        {
            ///horisontal scanning
            for (int x = 1; x < maskSize.Width - 1; x++)
            {
                for (int y = 1; y < maskSize.Height - 1; y++)
                {
                    if (dataMask[x, y] != 0)
                    {
                        Point p = new Point(x, y);

                        if (
                            dataMask[x, y + 1] == 0 &&
                            dataMask[x, y - 1] == 0 &&
                            dataMask[x + 1, y] == 0 &&
                            dataMask[x - 1, y] == 0 &&

                            dataMask[x + 1, y + 1] == 0 &&
                            dataMask[x - 1, y + 1] == 0 &&
                            dataMask[x + 1, y - 1] == 0 &&
                            dataMask[x - 1, y - 1] == 0
                            )
                        {
                            dataMask[x, y] = 0;
                        }
                        
                    }
                }
            }
        }

        #endregion
    }
}
