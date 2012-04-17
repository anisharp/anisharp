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

namespace AniSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Threading.Thread _fileParser;
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
            foreach (String s in (String[])e.Data.GetData(DataFormats.FileDrop))
                lbFiles_AddFile(s);
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
            if (!isDir)
            {
                if (!System.IO.Directory.Exists(sFile))
                {
                    if (!lbFiles.Items.Contains(sFile))
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
                        if (!lbFiles.Items.Contains(s))
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
            }
            if (e.WidthChanged)
            {
                lbLog.Width = this.ActualWidth - 27;
                lbFiles.Width = this.ActualWidth - 27;
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
            Login login = new Login();
            Nullable<bool> result = login.ShowDialog();
            if (result == true)
                MessageBox.Show(login.sUser + "\n" + login.sPassword);
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

        private void lbFiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
