using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardon___Exportacion_SIFERE.Exeptions
{
    public class Exceptions : Exception
    {
        public Exceptions(string message) : base(message) { }
    }
}
