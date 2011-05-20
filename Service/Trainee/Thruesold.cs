using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace Service.Trainee
{
    public class ColorDiapasons : Darkest
    {
        Diapason[] diapasons;

        public ColorDiapasons(Diapason[] diapasons)
        {
            this.diapasons = diapasons;
        }

        public override bool Contains(Color c)
        {
            bool result = false;
            float br = c.GetBrightness();

            for (int i = 0; i < diapasons.Length; i++)
            {
                Color since = diapasons[i].since;
                Color to = diapasons[i].to;

                if (c.R > since.R & c.G > since.G & c.B > since.B &
                    c.R < to.R & c.G < to.G & c.B < to.B)
                    return true;
            }

            return result;
        }

        public struct Diapason
        {
            public Color since;
            public Color to;

            public Diapason(Color since, Color to)
            {
                this.since = since;
                this.to = to;
            }

            public Diapason(byte sinceR, byte sinceG, byte sinceB, byte toR, byte toG, byte toB)
            {
                this.since = Color.FromArgb(sinceR, sinceG, sinceB);
                this.to = Color.FromArgb(toR, toG, toB);
            }
        }
    }


    public class BrightnessDiapasons : Darkest
    {
        Diapason[] diapasons;

        /// <summary>
        /// Default diapason since 0.7 to 0.9
        /// </summary>
        public BrightnessDiapasons() : this(new Diapason[] { new Diapason(0.7F, 0.9F) }) { ;}

        public BrightnessDiapasons(float since, float to) : this(new Diapason[] { new Diapason(since, to) }) { ;}

        public BrightnessDiapasons(Diapason[] diapasons)
        {
            this.diapasons = diapasons;
        }

        public override bool Contains(Color c)
        {
            bool result = false;
            float br = c.GetBrightness();

            for (int i = 0; i < diapasons.Length; i++)
            {
                if (!result)
                    result = br >= diapasons[i].since & br <= diapasons[i].to;

                if (result)
                    return true;
            }

            return result;
        }

        public struct Diapason
        {
            public float since;
            public float to;

            public Diapason(float since, float to)
            {
                this.since = since;
                this.to = to;
            }

            public Diapason(Color since, Color to)
            {
                this.since = since.GetBrightness();
                this.to = to.GetBrightness();
            }
        }
    }

    public class Darkest : ITrainee
    {
        Color margin = Color.FromArgb(100, 100, 100);

        public Darkest() { ;}

        public Darkest(byte maxR, byte maxG, byte maxB)
        {
            margin = Color.FromArgb(maxR, maxG, maxB);
        }

        #region ITrainee Members

        public bool Contains(byte r, byte g, byte b)
        {
            return Contains(Color.FromArgb(r, g, b));
        }

        public virtual bool Contains(Color c)
        {
            return c.R < margin.R & c.G < margin.G & c.B < margin.B;
        }

        public int Execute()
        {
            return 1;
        }

        #endregion
    }

    public class Thruesold : ITrainee
    {
        float thruesold = 0.5F;

        public Thruesold(float thruesold) { this.thruesold = thruesold; }

        #region ITrainee Members

        public bool Contains(byte r, byte g, byte b)
        { 
            return Contains(Color.FromArgb(r, g, b)); 
        }

        public bool Contains(Color c)
        {
            float d = Math.Abs(
                (                
                (float)(
                        (int)(c.GetBrightness() * 10)
                       ) / 10.0F
                ) -
                c.GetBrightness());


            return d > 0.05f;// c.GetBrightness() > thruesold;
        }

        public int Execute()
        { return 1; }

        #endregion
    }
}
