using BlackHole.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.ViewModels
{

    public class SymbolViewModel
    {

        public char Symbol { get; set; }
        public long Weight { get; set; }
        public byte Code { get; set; }
        public string NewCode { get; set; }

    }

}
