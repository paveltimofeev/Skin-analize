using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Service.Trainee
{
    public abstract class TraineeBase : ITrainee
    {
        public virtual int Execute() { return 0; }

        public abstract bool Contains(Color c);

        public abstract bool Contains(byte r, byte g, byte b);

    }
}
