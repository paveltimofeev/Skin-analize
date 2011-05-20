using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using Service.Scanner.Filters.PreProcessFilters;

namespace Service.Scanner.ScannersAndDataMaskBuilders
{
    public class SRegion : IComparable<SRegion>
    {
        #region Fields

        private readonly int regionIndex;
        private readonly Size max = new Size(0, 0);

        public int RegionIndex { get { return regionIndex;} }
        public int[,] Mask { get; private set; }

        public bool IsLandscape
        {
            get
            {
                return RegionRectangle.Width > RegionRectangle.Height;
            }
        }
        public float FillPercent
        {
            get
            {
                return (float)Size / (float)(RegionRectangle.Height * RegionRectangle.Width);
            }
        }
        public float BoxRate
        {
            get
            {
                return RegionRectangle.Height > 0 ? ((float)RegionRectangle.Width / (float)RegionRectangle.Height) : 0;
            }
        }

        private int? size = null;
        public int Size 
        {
            get
            {
                if (!size.HasValue)
                {
                    size = 0;
                    for (int x = 0; x < max.Width; x++)
                    {
                        for (int y = 0; y < max.Height; y++)
                        {
                            if (Mask[x, y] == regionIndex)
                            {
                                size++;
                            }
                        }
                    }
                }

                return size.Value;
            }
        }

        Rectangle? regionRectangle = null;
        public Rectangle RegionRectangle
        {
            get
            {
                if (!regionRectangle.HasValue)
                {
                    int fx = Int32.MaxValue; //min
                    int fy = Int32.MaxValue; //min
                    int fw = 0, fh = 0;      //max

                    unchecked
                    {
                        for (int x = 0; x < max.Width; x++)
                        {
                            for (int y = 0; y < max.Height; y++)
                            {
                                if (Mask[x, y] == regionIndex)
                                {
                                    fx = x < fx ? x : fx;
                                    fy = y < fy ? y : fy;

                                    fw = x > fw ? x : fw;
                                    fh = y > fh ? y : fh;
                                }
                            }
                        }
                    }

                    regionRectangle = new Rectangle(fx, fy, fw - fx, fh - fy);
                }

                return regionRectangle.Value;
            }
        }

        Point? center = null;
        public Point Center
        {
            get
            {
                if (!center.HasValue)
                {
                    int sum_x = 0;
                    int sum_y = 0;
                    int total = 0;

                    for (int x = 0; x < max.Width; x++)
                    {
                        for (int y = 0; y < max.Height; y++)
                        {
                            if (Mask[x, y] == regionIndex)
                            {
                                sum_x += x;
                                sum_y += y;
                                total++;
                            }
                        }
                    }

                    size = total;
                    float coef = 1.0F / (float)total;
                    center = new Point((int)(coef * (float)sum_x), (int)(coef * (float)sum_y));
                }

                return center.Value;
            }
        }

        int? edgeLength = null;
        public int EdgeLength
        {
            get
            {
                if (!edgeLength.HasValue)
                {
                    int[,] temp = (int[,])Mask.Clone();
                    EdgePreProcessFilter filter = new EdgePreProcessFilter();
                    filter.Apply(this.max, temp);
                    edgeLength = filter.TotalEdgeLenth;
                }

                return edgeLength.Value;
            }
        }

        float? compactness = null;
        public float Compactness
        {
            get
            {
                if(!compactness.HasValue)
                    compactness = (float)(EdgeLength * EdgeLength / size);

                return compactness.Value;
            }
        }

        double? enlongation = null;
        public double Enlongation
        {
            get
            {
                if (!enlongation.HasValue)
                    enlongation = (m(2, 0) + m(0, 2) + Math.Sqrt(Math.Pow(m(2, 0) - m(0, 2), 2) + 4.0D * Math.Pow(m(1, 1), 2)))
                                            / (m(2, 0) + m(0, 2) - Math.Sqrt(Math.Pow(m(2, 0) - m(0, 2), 2) + 4.0D * Math.Pow(m(1, 1), 2)));

                return enlongation.Value;
            }
        }

        float? orentation = null;
        public float Orentation
        {
            get
            {
                if (!orentation.HasValue)
                    orentation = 0.5F * (float)Math.Atan(2 * m(1, 1) / (m(2, 0) - m(0, 2))) * (180.0F / (float)Math.PI);

                return orentation.Value;
            }
        }

        private void ResetCalculatedValues()
        {
            edgeLength = null;
            compactness = null;
            enlongation = null;
            orentation = null;
            regionRectangle = null;
            center = null;
        }

        #endregion

        public SRegion(ref int[,] binaryImage, int regionIndex, Size bounders)
        {
            this.Mask = binaryImage;
            this.regionIndex = regionIndex;
            this.max = bounders;

            this.size = 0;
        }

        public void AddPoint(Point point)
        {
            AddPoint(point.X, point.Y);
        }

        public void AddPoint(int x, int y)
        {
            if (Mask[x, y] != regionIndex)
            {
                Mask[x, y] = regionIndex;
            }

            size++;

            ResetCalculatedValues();
        }

        public void RemovePoint(Point point)
        {
            RemovePoint(point.X, point.Y);
        }

        public void RemovePoint(int x, int y)
        {
            if (Mask[x, y] == regionIndex)
            {
                Mask[x, y] = 0;
            }
            size--;

            ResetCalculatedValues();
        }

        private unsafe Rectangle GetRegionRectangleUnsafe_()
        {
            int* fx = (int*)Int32.MaxValue, fy = (int*)Int32.MaxValue, fw = (int*)0, fh = (int*)0;
            int* _min = (int*)0;
            int* _maxx = (int*)max.Width;
            int* _maxy = (int*)max.Height;

            for (int* x = _min; x < _maxx; x = (int*)((int)x + 1))
            {
                for (int* y = _min; y < _maxy; y = (int*)((int)y + 1))
                {
                    if (Mask[(int)x, (int)y] == regionIndex)
                    {
                        fx = x < fx ? x : fx;
                        fy = y < fy ? y : fy;
                        fw = (int)x - (int)fx > (int)fw ? (int*)((int)x - (int)fx) : fw;
                        fh = (int)y - (int)fy > (int)fh ? (int*)((int)y - (int)fy) : fh;
                    }
                }
            }

            Rectangle result = new Rectangle((int)fx, (int)fy, (int)fw, (int)fh);

            return result;
        }

        /// <summary>
        /// DiscreteCentralMoment
        /// </summary>
        private double m(int i, int j)
        {
            Point center = this.Center;

            double m = 0;
            for (int x = 0; x < max.Width; x++)
            {
                for (int y = 0; y < max.Height; y++)
                {
                    if (Mask[x, y] == regionIndex)
                    {
                        m += Math.Pow((x - center.X), i) * Math.Pow((y - center.Y), j);
                    }
                }
            }

            return m;
        }

        #region IComparable<SRegion> Members

        public int CompareTo(SRegion other)
        {
            return this.Size.CompareTo(other.Size) * (-1);
        }

        #endregion

        public bool this[int x, int y]
        {
            get
            {
                if (x < max.Width & x >= 0 & y < max.Height & y >= 0)
                    return Mask[x, y] == regionIndex;
                else
                    return false;
            }
        }
        
        public override bool Equals(object obj)
        {
            SRegion region = obj as SRegion;
            if (region != null)
                return this.regionIndex == region.regionIndex;
            else
                return false;
        }
        
        public override int GetHashCode()
        {
            return this.regionIndex;
        }
        
        public override string ToString()
        {
            return string.Format("Region number {0}", this.regionIndex);
        }

        public static SRegion operator + (SRegion r1, SRegion r2)
        {
            if (!r1.Mask.Equals(r2.Mask))
                throw new Exception("Invalid cast! r1 and r2 have different masks!");

            for (int x = 0; x < r2.max.Width; x++)
            {
                for (int y = 0; y < r2.max.Height; y++)
                {
                    if (r1[x, y] & r2[x, y])
                        r1.Mask[x, y] = r2.regionIndex;
                }
            }

            return r1;
        }
    }
}
