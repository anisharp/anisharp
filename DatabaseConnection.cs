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
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Data;

namespace AniSharp
{
    class DatabaseConnection
    {
        private MainWindow mw = null;
        public DatabaseConnection(MainWindow mw)
        {
            this.mw = mw;
        }
        public void testConnectivity()
        {
            //MainWindow mw = new MainWindow();
            try {
                SqlCeConnection con = new SqlCeConnection(@"Data Source=AniSharpDB.sdf; Persist Security Info=False;");
                con.Open();  
                mw.lbDatabase_Add("OK");
                con.Close();             
            }
            catch (Exception) {
                mw.lbDatabase_Add("Failed !");
            }
        }
    }
}
