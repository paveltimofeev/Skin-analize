using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Scanner.Filters.RegionFilters;

namespace Service.Scanner.Labelers
{
    public interface IPostProcessFilterCollection
    {
        /// <summary>
        /// Добавляет PostProcess фильтр
        /// </summary>
        void AddFilter(IPostProcessFilter filter);

        /// <summary>
        /// Проверяет содержится ли в коллекции данный PostProcess фильтр
        /// </summary>
        bool ContainsFilter(IPostProcessFilter filter);

        /// <summary>
        /// Удаляет PostProcess фильтр
        /// </summary>
        void RemoveFilter(IPostProcessFilter filter);
    }

}
