using System;
using System.Drawing;
namespace Service.Scanner.Drawers
{
    /// <summary>
    /// Определяет интерфейс для отрисовки регионов
    /// </summary>
    public interface IDrawer : IDisposable
    {
        /// <summary>
        /// Полупрозначность слоя кожи
        /// </summary>
        int SkinLayerTransparent { get; set; }

        /// <summary>
        /// Режимы отображения информационного слоя
        /// </summary>
        InfoLayerModes InfoLayers { get; set; }        
        
        /// <summary>
        /// Режим отображения слоя кожи
        /// </summary>
        SkinLayerMode SkinLayer { get; set; }
        
        /// <summary>
        /// Режим отображения заднего плана
        /// </summary>
        BackLayerMode BackLayer { get; set; }
        
        /// <summary>
        /// Отрисорвка всех регионов
        /// </summary>
        Bitmap DrawRegions();
        
        /// <summary>
        /// Отрисовка региона по его индексу
        /// </summary>
        /// <param name="i">Индекс региона</param>
        Bitmap DrawRegion(int i);
    }

    [Flags]
    public enum InfoLayerModes : int { NONE = 0, FRAME = 2, CENTER = 4, INFORMATION = 8, SIZE = 16, FILLPERCENT = 32, BOXRATE = 64, COMPACTNESS = 128, ORENTATION = 256, ENLONGATION = 512, EDGELENGHT = 1024, REGIONNUMBER = 2048 }

    public enum SkinLayerMode : int { NONE, SKIN, COLORMASK, BLACKMASK }
    
    public enum BackLayerMode : int { TRANSPARENT, BACKGROUNDIMAGE }
}
