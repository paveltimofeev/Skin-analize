using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Service.Trainee
{
    public class GeneratedTrainee : FileTrainee
    {
        List<HSV> skinHSV = new List<HSV>();

        public override int Execute()
        {
            this.AlalizeFile(TrainSetFile);
            return 1;
        }

        private void AlalizeFile(Bitmap file)
        {
            for (int r = 0; r < 255; r++)
            {
                for (int g = 0; g < 255; g++)
                {
                    for (int b = 0; b < 255; b++)
                    {
                        bool rgbClassifier = (r > 95)
                            & (g > 40 & g < 100)
                            & (b > 20)
                            & (((Math.Max(Math.Max(r, g), b) - Math.Min(Math.Min(r, g), b)) > 15)
                            & (Math.Abs(r - g) > 15)
                            & (r > g)
                            & (r > b));

                        Color c = Color.FromArgb(r, g, b);
                        HSV h = c.ToHSV();

                        bool hsvClassifier = (h.H > 0 & h.H < 35 & h.S > 0.23d & h.S < 0.68);

                        if (
                            rgbClassifier | hsvClassifier
                        )
                        {
                            skinColourLabels[r, g, b] = true;
                            base.TotalPixels++;
                            base.labelsCount++;
                        }
                    }
                }
            }
        }
    }
}
