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
using Service.Scanner.Filters.RegionFilters;
using Service.Scanner.ScannersAndDataMaskBuilders;

namespace Service.Scanner.Labelers
{
    /// <summary>
    /// Определяет интерфейс реализации логики бинаризации и объединения регионов
    /// </summary>
    public interface ILabeler : IPreProcessFilterCollection, IPostProcessFilterCollection, IDisposable
    {
        /// <summary>
        /// Возвращает сканируемое изображение
        /// </summary>
        Bitmap Image { get; }

        /// <summary>
        /// Возвращает объединённые регионы
        /// </summary>
        SRegion[] Regions { get; }

        /// <summary>
        /// Выполнение бинаризации изображения и объединения регионов
        /// </summary>
        void Execute();
    }
}
