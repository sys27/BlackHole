using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class ArchivedFile
    {

        private string name;
        private long originalSize;
        private long bitsLength;
        private long offset;
        private IEnumerable<SymbolCode> codes;

        public ArchivedFile(string name, long originalSize, long bitsLength, long offset, IEnumerable<SymbolCode> codes)
        {
            this.name = name;
            this.originalSize = originalSize;
            this.bitsLength = bitsLength;
            this.offset = offset;
            this.codes = codes;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public long OriginalSize
        {
            get
            {
                return originalSize;
            }
        }

        public long BitsLength
        {
            get
            {
                return bitsLength;
            }
            internal set
            {
                bitsLength = value;
            }
        }

        public long Offset
        {
            get
            {
                return offset;
            }
            internal set
            {
                offset = value;
            }
        }

        public IEnumerable<SymbolCode> Codes
        {
            get
            {
                return codes;
            }
        }

    }

}
