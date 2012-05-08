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

namespace AniSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        public MainWindow()
        {
            InitializeComponent();
        }

        public void lbFiles_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Move;
        }

        void lbFiles_Drop(object sender, DragEventArgs e)
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

        /// <summary>
        /// fügt alle Dateien bzw. den Inhalt des Verzeichnisses und 
        /// Unterverzeichnis (rekursiv) der Listbox lbFiles hinzu.
        /// </summary>
        /// <remarks>Erste Test implementierung</remarks>
        /// <param name="sFile">Name der/des Dateie/Verzeichnis</param>
        /// <param name="isDir">Übergabe ist ein Verzeichnis</param>
        void lbFiles_AddFile(String sFile,bool isDir=false)
        {
            Regex rg = new Regex(FileFilter);
            if (!isDir)
            {
                if (!System.IO.Directory.Exists(sFile))
                {
                    if (rg.IsMatch(sFile)&&!lbFiles.Items.Contains(sFile))
                        lbFiles_Add(sFile);
                }
                else
                    lbFiles_AddFile(sFile, true);
            }
            else
            {
                try
                {
                    foreach (String s in Directory.GetFiles(sFile))
                    {
                        if (rg.IsMatch(s) && !lbFiles.Items.Contains(s))
                            lbFiles_Add(s);
                    }
                }
                catch (Exception) { }
                try
                {
                    foreach (String s in Directory.GetDirectories(sFile))
                        lbFiles_AddFile(s, true);
                }
                catch (Exception) { }
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
            Hash.HashGenerator hash;
            foreach (String s in lbFiles.Items)
            {
                hash = new Hash.HashGenerator(s);
                hash.Generate(Hash.HashType.Ed2k);
                lbLog_Add(hash.ToString());
                hash = null;
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
                lbLog.Height = this.ActualHeight - 100;
                lbFiles.Height = this.ActualHeight - 100;
                lbDatabase.Height = this.ActualHeight - 100;
            }
            if (e.WidthChanged)
            {
                lbLog.Width = this.ActualWidth - 27;
                lbFiles.Width = this.ActualWidth - 27;
                lbDatabase.Width = this.ActualWidth - 27;
            }
        }

        private void btFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
               lbFiles_AddFile(dlg.FileName);
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
                lbFiles_AddFile(dlg.SelectedPath, true);
        }

        public void lbFiles_Add(String sText)
        {
            Dispatcher.Invoke(new Action(() => { lbFiles.Items.Add(sText); }));
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
                //lbExtensions.Items.Add(new ListBoxItem(tbExtension.Text));
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
            DatabaseConnection dc = new DatabaseConnection(this);
            dc.testConnectivity();
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
    }
}
