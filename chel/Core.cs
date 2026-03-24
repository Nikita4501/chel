using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chel
{
    internal class Core
    {
        public static chelmoviesEntities Context = new chelmoviesEntities();
        public static Users CurrentUser { get; set; }
    }
}
