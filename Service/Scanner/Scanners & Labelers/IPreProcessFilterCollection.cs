using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.Scanner.Filters.RegionFilters;
using Service.Scanner.Filters.PreProcessFilters;

namespace Service.Scanner.Labelers
{
    public interface IPreProcessFilterCollection
    {
        /// <summary>
        /// Добавляет IPreProcessFilterCollection фильтр
        /// </summary>
        void AddFilter(IPreProcessFilter filter);

        /// <summary>
        /// Проверяет содержится ли в коллекции данный IPreProcessFilterCollection фильтр
        /// </summary>
        bool ContainsFilter(IPreProcessFilter filter);

        /// <summary>
        /// Удаляет IPreProcessFilterCollection фильтр
        /// </summary>
        void RemoveFilter(IPreProcessFilter filter);
    }
}
