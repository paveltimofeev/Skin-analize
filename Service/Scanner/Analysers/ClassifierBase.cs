using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Scanner.Labelers;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Analysers
{
    /// <summary>
    /// Реализует базовый функционал для всех наследующих классов.
    /// </summary>
    public abstract class ClassifierBase : IClassifier
    {
        public ILabeler Labeler { get; set; }
        public abstract SRegion[] GetRegions();

        public ClassifierBase(ILabeler labeler)
        {
            this.Labeler = labeler;
        }
    }
}
