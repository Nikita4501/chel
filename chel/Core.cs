using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chel
{
    /// <summary>
    /// Глобальный класс для доступа к контексту базы данных и текущему пользователю.
    /// </summary>
    public static class Core
    {
        public static chelmoviesEntities Context = new chelmoviesEntities();
        public static Users CurrentUser { get; set; }
    }
}