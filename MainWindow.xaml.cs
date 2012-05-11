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

namespace AniSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        ObservableCollection<Anime> _AnimeCollection =
        new ObservableCollection<Anime>();
        #region Hilfsklassen
        public ObservableCollection<Anime> AnimeCollection
        { get { return _AnimeCollection; } }
        public class Anime
        {
            public string FileName { get; set; }
            public string FileState { get; set; }
            public string FileHash { get; set; }
            public Anime(String s, String d = "", String f = "") { FileName = s; FileState = d; FileHash = f; }
        }

        public class AnimeComparer : IEqualityComparer<Anime>
        {

            public bool Equals(Anime a1, Anime a2)
            {
                return a1.FileName == a2.FileName;
            }
            
            public int GetHashCode(Anime a)
            {
                int hCode = a.FileHash.GetHashCode();
                return hCode.GetHashCode();
            }

        }
        #endregion
        private FileParser fileParser = null;
        private API.ApiSession conn = null;
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

        public MainWindow()
        {
            InitializeComponent();
            tbRenamePattern.Text = AniSharp.Properties.Settings.Default.RenamePattern;
        }

        public void lvFiles_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Move;
        }

        void lvFiles_Drop(object sender, DragEventArgs e)
        {
            fileParser = new FileParser(this, FileFilter);

            foreach (String s in (String[])e.Data.GetData(DataFormats.FileDrop))
            {
                fileParser.setFile(s);
                System.Threading.Thread thread = new System.Threading.Thread(fileParser.ParseFile);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            btStart.IsEnabled = false;
            lbLog.Items.Clear();
            System.Threading.Thread hash = new System.Threading.Thread(hashGen);
            hash.Start();
        }

        public void activateStart()
        {
            Dispatcher.Invoke(new Action(() => { btStart.IsEnabled = true; }));
        }
        
        private void hashGen()
        {
            foreach (Anime s in AnimeCollection)
            {
                lbLog_Add(new Hash.Ed2kHashGenerator(s.FileName).ToString());

                System.GC.Collect();
            }
            activateStart();
        }

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
                lbDatabase.Height = this.ActualHeight - 100;
            }
            if (e.WidthChanged)
            {
                tabControl1.Width = this.ActualWidth-20;
                lbLog.Width = this.ActualWidth - 27;
                lvFiles.Width = this.ActualWidth - 27;
                lbDatabase.Width = this.ActualWidth - 27;
            }
        }

        private void btFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                fileParser = new FileParser(this, FileFilter);
                fileParser.setFile(dlg.FileName);
                System.Threading.Thread thread = new System.Threading.Thread(fileParser.ParseFile);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            if (btLogin.Content.ToString() == "Login")
            {
                Login login = new Login();
                Nullable<bool> result = login.ShowDialog();
                if (result == true)
                {
                    conn = new API.ApiSession();
                    conn.ApiSessionStatusChanged += onApiSessionStatusChange;
                    conn.login(login.sUser, login.sPassword);
                    btLogin.Content = "Logout";
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

        private void btFolders_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                fileParser = new FileParser(this, FileFilter);
                fileParser.setFile(dlg.SelectedPath);
                System.Threading.Thread thread = new System.Threading.Thread(fileParser.ParseFile);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void lvFiles_Add(String sText)
        {
            Dispatcher.Invoke(new Action(() => { _AnimeCollection.Add(new Anime(sText,"Wait/Hash"));}));
        }

        public void lbLog_Add(String sText)
        {
            Dispatcher.Invoke(new Action(() => { lbLog.Items.Add(sText); }));
        }

        public void lbDatabase_Add(String sText)
        {
            Dispatcher.Invoke(new Action(() => { lbDatabase.Items.Add(sText); }));
        }

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

        private void chkAdd_Unchecked(object sender, RoutedEventArgs e)
        {
            cbState.IsEnabled = false;
            tbSource.IsEnabled = false;
            tbStorage.IsEnabled = false;
            chkWatched.IsEnabled = false;
            tbOther.IsEnabled = false;
        }

        private void chkAdd_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                cbState.IsEnabled = true;
                tbSource.IsEnabled = true;
                tbStorage.IsEnabled = true;
                chkWatched.IsEnabled = true;
                tbOther.IsEnabled = true;
            }
            catch (Exception) { }//fängt fehler bei der initialisierung
        }

        private void btDatabase_Click(object sender, RoutedEventArgs e)
        {
            DatabaseConnection dc = new DatabaseConnection();
            episode ep = dc.getEpisode("1q2w3e");
            MessageBox.Show(ep.epKanjiName);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // logout
            if (conn != null)
            {
                conn.shutdown();
                conn = null;
            }
        }
        
        public void onApiSessionStatusChange(bool loggedIn, bool shouldRetry, string Message)
        {
            /*
            if (loggedIn)
            {
                System.Diagnostics.Debug.Print("logged in, query starts");
                ApiAnswer aa = conn.query(new AnimeRequest(1));
                System.Diagnostics.Debug.Print("we have an answer...");

                if (aa is AnimeAnswer)
                {
                    AnimeAnswer ana = (AnimeAnswer)aa;
                    serie s = (serie)ana;
                    System.Diagnostics.Debug.Print("WOOHOO!");
                    MessageBox.Show("ID 1 == " + s.englishName);
                }
                else
                {
                    System.Diagnostics.Debug.Print("did not work .. will fail");
                    FailedLoginAnswer e = (FailedLoginAnswer)aa;
                }
            }
            else
            {
                System.Diagnostics.Debug.Print("something wrong..." + loggedIn + " " + shouldRetry + ". msg " + Message);
            }
            */
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
