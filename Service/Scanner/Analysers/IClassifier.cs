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
using Service.Scanner.Drawers;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Analysers
{
    /// <summary>
    /// Определяет интерфейс для анализа регионов
    /// </summary>
    public interface IClassifier
    {
        /// <summary>
        /// Задаёт или возвращает ссылку на ILabeler
        /// </summary>
        ILabeler Labeler { get; set; }

        /// <summary>
        /// Возвращает массив регионов
        /// </summary>
        /// <returns></returns>
        SRegion[] GetRegions();
    }
}
