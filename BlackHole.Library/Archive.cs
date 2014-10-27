using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Library
{

    public class Archive : IEnumerable<ArchivedFile>
    {

        private List<ArchivedFile> files;
        private long originalSize;

        public Archive() { }

        public Archive(IEnumerable<ArchivedFile> files)
        {
            this.files = new List<ArchivedFile>();
            if (files != null && this.files.Count > 0)
            {
                this.files.AddRange(files);
                foreach (var file in files)
                    originalSize += file.OriginalSize;
            }
        }

        public ArchivedFile this[int index]
        {
            get
            {
                return files[index];
            }
        }

        public IEnumerator<ArchivedFile> GetEnumerator()
        {
            return files.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ArchivedFile file)
        {
            files.Add(file);
            originalSize += file.OriginalSize;
        }

        public void Remove(ArchivedFile file)
        {
            if (files.Remove(file))
                originalSize -= file.OriginalSize;
        }

        public int FilesCount
        {
            get
            {
                return files.Count;
            }
        }

        public long OriginalSize
        {
            get
            {
                return originalSize;
            }
        }

    }

}
