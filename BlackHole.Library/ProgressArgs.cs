using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class ProgressArgs
    {

        public int CurrentFile { get; set; }
        public int TotalFiles { get; set; }

        public long CurrentSize { get; set; }
        public long TotalSize { get; set; }

    }

}
