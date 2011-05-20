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
using Service.Scanner.Labelers;
using Service.Scanner.Filters.RegionFilters;
using Service.Scanner.ScannersAndDataMaskBuilders;
using Service.Scanner.Drawers;

namespace Service.Scanner.Analysers
{
    public class NClassifier : ClassifierBase
    {
        public NClassifier(ILabeler labeler) : base(labeler) { ;}

        public override SRegion[] GetRegions()
        {
            Array.Sort(Labeler.Regions, new SRegionInverseComparer());

            int totalSize = 0;
            for (int i = 0; i < Labeler.Regions.Length; i++)
            {
                totalSize += Labeler.Regions[i].Size;
            }

            //1Вся дальнейшие выкрутасы связаны с этими регионами
            //1если самый большой регион содержит меньше 35% кожаных пикселей, 
            //1второй и третий — меньше 30% каждый, — это логотип Timberland, а не голая девушка; 
            //3если самый большой регион содержит меньше 45% кожаных пикселей — это карта Европы без России, а не голая девушка; 
            //4если кожаных пикселей меньше 30%, а регионы сильно разбросаны по фотографии — это мясорубка, а не порнография; 
            //5если количество регионов больше 60, но основная часть фотографии неяркая — это фотография из Коммерсанта, а не Playboy. 
            //И если вдруг ни одно из этих условий не сбылось, ликуем — тут есть на что посмотреть!

            if (Labeler.Regions.Length >= 3)
            {

                bool rule1 =
                    (
                    ((float)Labeler.Regions[0].Size / (float)totalSize) > 0.35F
                    ) &
                    (
                    ((float)Labeler.Regions[1].Size / (float)totalSize) > 0.30
                    ) &
                    (
                    ((float)Labeler.Regions[2].Size / (float)totalSize) > 0.30
                    )
                    ? true : false;

                bool rule2 = ((float)Labeler.Regions[0].Size / (float)totalSize) > 0.45F ? true : false;
                bool rule3 = ((float)totalSize / (float)(Labeler.Image.Height * Labeler.Image.Width)) > 0.30F ? true : false;
                bool rule4 = Labeler.Regions.Length < 60 ? true : false;

                if (rule1 & rule2 & rule3 & rule4)
                    return Labeler.Regions;
                else
                    return null;
            }
            else
                return Labeler.Regions;
        }
    }

    public class GroupClassifier : ClassifierBase
    {
        public GroupClassifier(ILabeler labeler) : base(labeler) { ;}

        public static bool IsRegionGroup(SRegion region1, SRegion region2)
        {
            if (region1.RegionRectangle.IntersectsWith(region2.RegionRectangle))
                return true;
            else
                return false;
        }

        public override SRegion[] GetRegions()
        {
            List<SRegion> faces = new List<SRegion>();

            for (int i = 0; i < Labeler.Regions.Length; i++)
            {
                for (int j = 0; j < Labeler.Regions.Length; j++)
                {
                    if (i != j && IsRegionGroup(Labeler.Regions[i], Labeler.Regions[j]))
                        faces.Add(Labeler.Regions[i] + Labeler.Regions[j]);
                }
            }

            SRegion[] result = new SRegion[faces.Count];
            faces.CopyTo(result, 0);
            return result;
        }
    }

    public class FaceClassifier : ClassifierBase
    {
        public FaceClassifier(ILabeler labeler) : base(labeler) { ;}

        public static bool IsRegionFace(SRegion region)
        {
            if (region.FillPercent > 0.45 & region.FillPercent < 0.8 & region.BoxRate > 0.4F & region.BoxRate < 1.0F)
                return true;
            else
                return false;
        }

        public override SRegion[] GetRegions()
        {
            List<SRegion> faces = new List<SRegion>();

            for (int i = 0; i < Labeler.Regions.Length; i++)
            {
                if (IsRegionFace(Labeler.Regions[i]))
                    faces.Add(Labeler.Regions[i]);
            }

            SRegion[] result = new SRegion[faces.Count];
            faces.CopyTo(result, 0);
            return result;
        }
    }
}
