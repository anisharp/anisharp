using System;
using System.ComponentModel;
namespace AniSharp
{
    public class Anime : INotifyPropertyChanged
    {
        String _FileName;
        String _FileState;
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (value != _FileName)
                {
                    _FileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }
        public string FileState 
        { 
            get { 
                return (_FileState); 
            } 
            set {
                if (value != this._FileState)
                {
                    _FileState = value;
                    NotifyPropertyChanged("FileState");
                }
            } 
        }
        public string FileHash { get; set; }
        public Anime(String s, String d = "", String f = "") { FileName = s; FileState = d; FileHash = f; }
    }
}
