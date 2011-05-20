using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Service.Trainee
{
    public class FileTrainee : TraineeBase
    {
        public Bitmap TrainSetFile { get; set; }

        protected const int coloursCount = 256;
        protected int labelsCount = 0;
        protected bool[, ,] skinColourLabels = new bool[256, 256, 256];

        List<HSV> skinHSV = new List<HSV>();

        public int LearnedColours { get { return labelsCount; } }
        public int LearnedHSV { get { return skinHSV.Count; } }
        public int TotalPixels { get; protected set; }

        public FileTrainee() { ;}

        public FileTrainee(Bitmap trainSetFile)
        {
            this.TrainSetFile = trainSetFile;
        }

        public override int Execute()
        {
            this.AlalizeFile(TrainSetFile);
            return 1;
        }

        private void AlalizeFile(Bitmap file)
        {
            for (int x = 0; x < file.Width; x++)
            {
                for (int y = 0; y < file.Height; y++)
                {
                    Color c = file.GetPixel(x, y);
                    this.Add(c);
                }
            }

            for (int r = 0; r < coloursCount; r++)
            {
                for (int g = 0; g < coloursCount; g++)
                {
                    for (int b = 0; b < coloursCount; b++)
                    {
                        if (skinColourLabels[r, g, b])
                        {
                            labelsCount++;
                        }
                    }
                }
            }

            TotalPixels += file.Width * file.Height;
        }

        private void Add(Color colour)
        {
            skinColourLabels[colour.R, colour.G, colour.B] = true;
        }

        public override bool Contains(Color colour)
        {
            bool hsv = false;
            bool rgb = false;

            HSV h = colour.ToHSV();
            if ((h.H > 0 & h.H < 35 & h.S > 0.23d & h.S < 0.68) | Contains(h))
                hsv = true;

            rgb = skinColourLabels[colour.R, colour.G, colour.B];

            return rgb | hsv;
        }

        public bool Contains(HSV hsv)
        {
            return skinHSV.Contains(hsv);
        }

        public void SaveAs(string file)
        {
            List<Color> skinColour = new List<Color>();

            for (int r = 0; r < coloursCount; r++)
            {
                for (int g = 0; g < coloursCount; g++)
                {
                    for (int b = 0; b < coloursCount; b++)
                    {
                        if (skinColourLabels[r, g, b])
                        {
                            skinColour.Add(Color.FromArgb(r, g, b));
                        }
                    }
                }
            }

            int h = skinColour.Count / 1024;
            Bitmap map = new Bitmap(1024, h);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    int p = map.Width * y + x;

                    if (p < skinColour.Count)
                    {
                        map.SetPixel(x, y, skinColour[p]);
                    }
                    else
                    {
                        map.SetPixel(x, y, skinColour[0]);
                    }
                }
            }

            map.Save(file);
        }

        public override bool Contains(byte r, byte g, byte b)
        {
            throw new NotImplementedException();
        }
    }
}
