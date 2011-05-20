using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Service.Trainee
{
    public class RgbHCrCbTrainee : TraineeBase
    {
        public override int Execute()
        {
            return 0;
        }

        public override bool Contains(Color c)
        {
            //if ((DayLightRGB(c) | FlashLightRGB(c)) & (CbCr(c)) & (H(c)))
            if ((DayLightRGB(c) | FlashLightRGB(c)) & (H(c)))
                return true;
            else
                return false;
        }

        #region Rule A

        private bool DayLightRGB(Color c)
        {
            return DayLightRGB(c.R, c.G, c.B);
        }

        private bool DayLightRGB(byte R, byte G, byte B)
        {
            if (
                (R > 95 & G > 40 & B > 20) &
                (Math.Max(Math.Max(R, G), B) - Math.Min(Math.Min(R, G), B) > 15) &
                (Math.Abs(R - G) > 15 & (R > B) & (R > B))
               )
                return true;
            else
                return false;
        }

        private bool FlashLightRGB(Color c)
        {
            return FlashLightRGB(c.R, c.G, c.B);
        }

        private bool FlashLightRGB(byte R, byte G, byte B)
        {
            if (
                R > 200 & G > 210 & B > 170 &
                Math.Abs(R - G) > 15 & (R > B) & (R > B)
               )
                return true;
            else
                return false;
        }

        #endregion

        #region Rule B

        private bool CbCr(Color c)
        {
            YCbCr y = c.ToYCbCr();

            if (
                (y.cr <= 1.5682F * y.cb + 20) &
                (y.cr <= 0.3448F * y.cb + 76.2069F) &
                (y.cr <= -4.5652F * y.cb + 234.5652F) &
                (y.cr <= -1.15F * y.cb + 301.75F) &
                (y.cr <= -2.2857F * y.cb + 432.85F)
              )
                return true;
            else
                return false;
        }

        #endregion

        #region Rule C

        private bool H(byte r, byte g, byte b)
        {
            //return H(Color.FromArgb(r, g, b));

            float h = 0;
            float mx = Math.Max(Math.Max(r, g), b);
            float mn = Math.Min(Math.Min(r, g), b);

            if (mx != mn)
            {
                if (mx == r)
                    h = (g - b) / (mx - mn);
                else if (mx == g)
                    h = 2 + ((g - r) / (mx - mn));
                else
                    h = 4 + ((r - g) / (mx - mn));

                h = h * 60;
                if (h < 0)
                    h = h + 360;
            }
            else
                h = 0;

            if (h < 25 | h > 230)
                return true;
            else
                return false;
        }

        private bool H(Color c)
        {
            float h = c.GetHue();
            if (h < 25 | h > 230)
                return true;
            else
                return false;
        }

        #endregion

        public override bool Contains(byte r, byte g, byte b)
        {
            if ((DayLightRGB(r, g, b) | FlashLightRGB(r, g, b)) & (H(r, g, b)))
                return true;
            else
                return false;
        }
    }
}
