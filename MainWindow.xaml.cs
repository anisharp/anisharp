#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using AniSharp.API.Model.Request;
using AniSharp.API.Model.Answer;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows.Threading;
#endregion

namespace AniSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        FileRenamer _fr = FileRenamer.getInstance();

        ObservableCollection<String> _Log =
        new ObservableCollection<String>();
        ObservableCollection<Anime> _AnimeCollection =
        new ObservableCollection<Anime>();

        public ObservableCollection<String> Log
        {
            get { return _Log; }
        }
        public ObservableCollection<Anime> AnimeCollection
        {
            get { return _AnimeCollection; }
        }

        public static RoutedCommand DeleteCmd = new RoutedCommand();
        public static RoutedCommand CopyCmd = new RoutedCommand();
        private API.Application.ApiSession conn = null;
        /// <summary>
        /// gibt einen FileFilter zurueck der die gewuenschten Dateiendungen enthaelt.
        /// </summary>
        public String FileFilter
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(".*.(?i)(");
                foreach (ListBoxItem s in lbExtensions.Items)
                    sb.Append(s.Content).Append("|");
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")$");
                return sb.ToString();
            }
        }
        #endregion

        /// <summary>
        /// Konstruktor fuer das MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            tbRenamePattern.Text = AniSharp.Properties.Settings.Default.RenamePattern;
            tbMove.Text = AniSharp.Properties.Settings.Default.MovePattern;
            _fr.setPattern(AniSharp.Properties.Settings.Default.RenamePattern);
            _fr.setPath(AniSharp.Properties.Settings.Default.MovePattern);
            _fr.setMainWindow(this);
        }

        /// <summary>
        /// Aktiviert den Start Knopf
        /// </summary>
        public void activateStart()
        {
            Dispatcher.Invoke(new Action(() => { btStart.IsEnabled = true; }));
        }
        #region window functions
        /// <summary>
        /// Passt die ListBoxen an die größe des Fensters an
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.HeightChanged)
            {
                tabControl1.Height = this.ActualHeight-70;
                lbLog.Height = this.ActualHeight - 100;
                lvFiles.Height = this.ActualHeight - 100;
            }
            if (e.WidthChanged)
            {
                tabControl1.Width = this.ActualWidth-20;
                lbLog.Width = this.ActualWidth - 27;
                lvFiles.Width = this.ActualWidth - 27;
            }
        }

        /// <summary>
        /// Event, dass beim schliessen des Hauptfenster alle verbleibenden Verbindungen beendet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // logout
            if (conn != null)
            {
                conn.shutdown();
                conn = null;
            }
        }
        #endregion

        #region button functions

        /// <summary>
        /// Startet den Vorgang um die in der Liste als Wait/Start eingetragenen Dateien zu verarbeiten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            btStart.IsEnabled = false;
            //lbLog.Items.Clear();

            foreach (Anime s in _AnimeCollection)
            {
                if (s.FileState.CompareTo("Wait/Start") == 0)
                {
                    Glue g = new Glue(s, conn, this);
                    System.Threading.Thread pattexing = new System.Threading.Thread(g.run);
                    pattexing.Start();
                }
            }
            GC.Collect();
            activateStart();
        }

        /// <summary>
        /// Oeffnet einen File Dialog um einzelne Dateien zur Liste hinzuzufuegen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            StringBuilder sb = new StringBuilder();
            foreach (ListBoxItem s in lbExtensions.Items)
                sb.Append("*.").Append(s.Content).Append(";");
            sb.Remove(sb.Length - 1, 1);
            dlg.Filter = "Videodateien ("+sb.ToString()+")|"+sb.ToString();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                FileParser fp = new FileParser(dlg.FileNames, this, FileFilter);
                System.Threading.Thread t = new System.Threading.Thread(fp.ParseFile);
                t.Start();
            }
        }

        /// <summary>
        /// Oeffnet einen Login Screen mit dem sich der Benutzer anmelden kann. Bei erfolgreicher Eingabe wird die verbindung zum Server hergestellt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            if (btLogin.Content.ToString() == "Login")
            {
                Login login = new Login();
                Nullable<bool> result = login.ShowDialog();
                if (result == true)
                {
                    btLogin.IsEnabled = false;
                    try
                    {
                        conn = new API.Application.ApiSession();
                    }
                    catch (SocketException)
                    {
                        MessageBox.Show("AniDB is unreachable", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    conn.ApiSessionStatusChanged += onApiSessionStatusChange;
                    conn.login(login.sUser, login.sPassword);
                    //btLogin.Content = "Logout";
                }
            }
            else
            {
                if (conn != null)
                {
                    conn.shutdown();
                    conn = null;
                }
                btLogin.Content = "Login";
            }
                
        }

        /// <summary>
        /// oeffnet einen Folder Dialog um einen bestimmten Ordner in die Liste hinzuzufuegen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFolders_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FileParser fp = new FileParser(dlg.SelectedPath, this, FileFilter);
                System.Threading.Thread t = new System.Threading.Thread(fp.ParseFile);
                t.Start();
            }
        }

        /// <summary>
        /// Opens a browser dialog to set the path the anime files should be moved to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                tbMove.Text = dlg.SelectedPath;
        }

        /// <summary>
        /// Button um das gewaehlte Pattern in den Einstellungen zu speichern.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (AniSharp.Properties.Settings.Default.RenamePattern != tbRenamePattern.Text)
            {
                AniSharp.Properties.Settings.Default.RenamePattern = tbRenamePattern.Text;
                AniSharp.Properties.Settings.Default.Save();
                _fr.setPattern(tbRenamePattern.Text);
            }
            if (AniSharp.Properties.Settings.Default.MovePattern != tbMove.Text)
            {
                AniSharp.Properties.Settings.Default.MovePattern = tbMove.Text;
                AniSharp.Properties.Settings.Default.Save();
                _fr.setPath(tbMove.Text);
            }
        }
        #endregion

        #region delegate functions


        /// <summary>
        /// returns the information if the file should be added to AniDB or not
        /// </summary>
        /// <returns></returns>
        public bool? getAdd()
        {
            bool? ret=false;
            Dispatcher.Invoke((Action)(() => { ret = chkAdd.IsChecked; }));
            return ret;
        }

        /// <summary>
        /// returns the state of the anime file
        /// </summary>
        /// <returns>int state</returns>
        public int getState()
        {
            int ret = 0;
            Dispatcher.Invoke((Action)(() =>
            {
                switch (cbState.Text)
                {
                    case "Unknown": ret = 0; break;
                    case "On HDD": ret = 1; break;
                    case "On CD": ret = 2; break;
                    case "Deleted": ret = 3; break;
                    default: ret = 0; break;
                }
            }));
            return ret;
        }

        /// <summary>
        /// returns the viewed state of the anime
        /// </summary>
        /// <returns>true,false,null</returns>
        public bool? getViewed()
        {
            bool? ret = false;
            Dispatcher.Invoke((Action)(() =>
            { ret = chkWatched.IsChecked; }));
            return ret;
        }

        /// <summary>
        /// returns the value of Source
        /// </summary>
        /// <returns>string source</returns>
        public string getSource()
        {
            string ret="";
            Dispatcher.Invoke((Action)(() =>
            { ret = tbSource.Text; }));
            return ret;
        }

        /// <summary>
        /// returns the value of Storage
        /// </summary>
        /// <returns>string storage</returns>
        public string getStorage()
        {
            string ret = "";
            Dispatcher.Invoke((Action)(() =>
            { ret = tbStorage.Text; }));
            return ret;
        }

        /// <summary>
        /// returns the value of Other
        /// </summary>
        /// <returns>string other</returns>
        public string getOther()
        {
            string ret="";
            Dispatcher.Invoke((Action)(() =>
            { ret = tbOther.Text; }));
            return ret;
        }
  
        /// <summary>
        /// delegate der es anderen Threads erlaubt Dateien zur AnimeListe hinzuzufuegen
        /// </summary>
        /// <param name="sText"></param>
        public void lvFiles_Add(String sText)
        {
            Dispatcher.Invoke(new Action(() => { _AnimeCollection.Add(new Anime(sText,"Wait/Start"));}));
        }

        /// <summary>
        /// delegate der es anderen Threads erlaubt Status Updates in den Log zu schreiben
        /// </summary>
        /// <param name="sText"></param>
        public void lbLog_Add(String sText)
        {
            Dispatcher.Invoke(new Action(() => { Log.Add(sText); }));
        }

        #endregion

        #region other events

        /// <summary>
        /// Event, dass den Mauszeiger anpasst sobald gültige Dateien in die Liste gezogen werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void lvFiles_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Move;
        }

        /// <summary>
        /// Event, dass das einfuegen von Dateien per drag and drop erlaubt und diese der Liste hinzufuegt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lvFiles_Drop(object sender, DragEventArgs e)
        {
            FileParser fp = new FileParser((String[])e.Data.GetData(DataFormats.FileDrop), this, FileFilter);
            System.Threading.Thread t = new System.Threading.Thread(fp.ParseFile);
            t.Start();
        }

        /// <summary>
        /// Eventhandler der im falle von Enter den in der TextBox Extensions eingegebenen Text zu den erlaubten Dateiendungen hinzuzufügen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbExtension_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrEmpty(tbExtension.Text.ToString()))
            {
                ListBoxItem lb = new ListBoxItem();
                lb.Content = tbExtension.Text;
                lbExtensions.Items.Add(lb);
                tbExtension.Clear();
            }
        }

        /// <summary>
        /// Event das im falle eines Login versuchs in AniDB eine benachrichtigung empfaengt ob er erfolgreich war oder gescheitert ist. 
        /// Im falle eines erfolgreichen Logins wird der Start Button aktiviert.
        /// </summary>
        /// <param name="loggedIn"></param>
        /// <param name="shouldRetry"></param>
        /// <param name="Message"></param>
        public void onApiSessionStatusChange(bool loggedIn, bool shouldRetry, string Message)
        {
            if (loggedIn)
            {
                btLogin.Content = "Logout";
                MessageBox.Show("Logged in");
                activateStart();
            }
            else
            {
                MessageBox.Show("Not logged in: " + shouldRetry + " msg == " + Message);
            }
            btLogin.IsEnabled = true;
        }
        #endregion

        #region CommandEventHandler

        /// <summary>
        /// Funktion die bei erfolgreichem Rechtsklick auf ein Eintrag in der Liste ausgefuehrt werden kann.
        /// Loescht den ausgewhaehlten Eintrag aus der Liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (lvFiles.SelectedItems != null)
            {
                Anime[] anim = new Anime[lvFiles.SelectedItems.Count];
                lvFiles.SelectedItems.CopyTo(anim, 0);
                foreach (Anime a in anim)
                {
                    AnimeCollection.Remove((Anime)a);
                }
            }
        }

        /// <summary>
        /// Ueberprueft ob die Funktion Delete ausgewaehlt werden darf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        /// <summary>
        /// Funktion die bei erfolgreichem Rechtsklick auf ein Eintrag in der Liste ausgefuehrt werden kann.
        /// Kopiert den ausgewaehlten Eintrag aus der Liste in die Zwischenablage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (lbLog.SelectedItem != null)
                Clipboard.SetData(DataFormats.Text, (Object)lbLog.SelectedItem);
        }

        /// <summary>
        /// Ueberprueft ob die Funktion Copy ausgewaehlt werden darf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        /// <summary>
        /// Aktualisiert das Move Pattern sobald der Text verändert wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMove_TextChanged(object sender, TextChangedEventArgs e)
        {
            _fr.setPath(tbMove.Text);
        }
    }
}