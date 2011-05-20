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
using Service.Trainee;

namespace Service.Scanner.ScannersAndDataMaskBuilders
{
    public abstract class ImageScanner
    {
        protected ITrainee learned;
        public Bitmap Image { get; set; }

        public ImageScanner(ITrainee learned) : this(learned, string.Empty) { ;}

        public ImageScanner(ITrainee learned, string file) : this(learned, new Bitmap(file)) { ;}

        public ImageScanner(ITrainee learned, Bitmap image)
        {
            if (image.Width * image.Height > 700 * 700)
            {
                int rate = 300;
                float coef = (float)image.Height / (float)rate;
                Image = new Bitmap(image, new Size((int)((float)image.Width / coef), rate));
            }
            else
            {
                Image = new Bitmap(image);
            }

            this.learned = learned;
            this.Image = image;
        }

        public void SetTrainee(TraineeBase learned) { this.learned = learned; }

        public abstract void Execute();
    }
}
