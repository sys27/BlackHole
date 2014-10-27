using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole
{
    
    public class FileViewModel : INotifyPropertyChanged
    {

        private string fullName;
        private string name;
        private long size;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileViewModel(string fullName, string name, long size)
        {
            this.name = name;
            this.size = size;
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public string FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
                OnPropertyChanged("fullName");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public long Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

    }

}
