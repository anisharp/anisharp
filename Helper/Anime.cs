using System;
using System.ComponentModel;
namespace AniSharp
{
    /// <summary>
    /// Anime Klasse zum verwalten einzelner files, hash und deren states
    /// </summary>
    public class Anime : INotifyPropertyChanged
    {
        String _FileName;
        String _FileState;
        /// <summary>
        /// Event, wenn eins der Elemente geaendert wurde
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Eventhandler falls eins der ueberwachten Felder ein Event triggert
        /// </summary>
        /// <param name="info"></param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// FileName des Animes.
        /// Feuert PropertyChanged wenn sich der Wert aendert.
        /// </summary>
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

        /// <summary>
        /// FileState des Animes.
        /// Feuert PropertyChanged wenn sich der Wert aendert.
        /// </summary>
        public string FileState
        {
            get
            {
                return (_FileState);
            }
            set
            {
                if (value != this._FileState)
                {
                    _FileState = value;
                    NotifyPropertyChanged("FileState");
                }
            }
        }
        
        /// <summary>
        /// FileHash des Animes.
        /// </summary>
        public string FileHash { get; set; }

        /// <summary>
        /// Konstruktor der Anime Klasse
        /// </summary>
        public Anime(String FileName, String FileState = "", String FileHash = "") 
        { 
            this.FileName = FileName; 
            this.FileState = FileState; 
            this.FileHash = FileHash; 
        }
    }
}
