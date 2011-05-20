using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Service.Trainee
{
    public class Trainer
    {
        ITrainee strategy;

        public Trainer(ITrainee defaultStrategy)
        {
            this.strategy = defaultStrategy;
        }

        public void SetStrategy(ITrainee strategy)
        {
            this.strategy = strategy;
        }

        public int Analize()
        {
            return this.strategy.Execute();
        }
    }
}
