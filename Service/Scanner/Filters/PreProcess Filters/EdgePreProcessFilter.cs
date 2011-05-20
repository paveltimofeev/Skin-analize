using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Scanner.Filters.PreProcessFilters;
using System.Drawing;

namespace Service.Scanner.Filters.PreProcessFilters
{
    /// <summary>
    /// Remoove internal content of region and leave only it's edge (internal edge).
    /// </summary>
    public class EdgePreProcessFilter : IPreProcessFilter
    {
        #region IScanFilter Members

        /// <summary>
        /// Will remove 1x1 regions which not connected with others.
        /// </summary>
        public void Apply(Size maskSize, int[,] dataMask)
        {
            int[,] temp = new int[maskSize.Width, maskSize.Height];

            ///horisontal scanning
            for (int x = 1; x < maskSize.Width - 1; x++)
            {
                for (int y = 1; y < maskSize.Height - 1; y++)
                {
                    if (dataMask[x, y] != 0)
                    {
                        int region = dataMask[x, y];

                        if (
                            dataMask[x    , y + 1] == region &
                            dataMask[x    , y - 1] == region &
                            dataMask[x + 1, y    ] == region &
                            dataMask[x - 1, y    ] == region &
                            dataMask[x + 1, y + 1] == region &
                            dataMask[x - 1, y + 1] == region &
                            dataMask[x + 1, y - 1] == region &
                            dataMask[x - 1, y - 1] == region
                            )
                        {
                            //-1 is skin pixel, 0 is not skin pixel
                            temp[x, y] = 1; //mark point as fill if around is same points
                        }
                    }
                }
            }

            TotalEdgeLenth = 0;
            for (int x = 1; x < maskSize.Width - 1; x++)
            {
                for (int y = 1; y < maskSize.Height - 1; y++)
                {
                    if (temp[x, y] == 1)
                    {
                        dataMask[x, y] = 0; //clear point if it was marked as fill
                        TotalEdgeLenth++;
                    }
                }
            }

            temp = null;
        }

        public int TotalEdgeLenth { get; private set; }

        #endregion
    }
}
