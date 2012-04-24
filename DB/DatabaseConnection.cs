using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            try
            {
                SqlCeConnection con = new SqlCeConnection(@"Data Source=AniSharpDB.sdf; Persist Security Info=False;");
                con.Open();
                mw.lbDatabase_Add("Connection established.");
                getFromDatabase(con);
                con.Close();
                mw.lbDatabase_Add("Connection closed.");
            }
            catch (Exception)
            {
                mw.lbDatabase_Add("Connection Problem");
            }
        }
        private void getFromDatabase(SqlCeConnection con)
        {

            SqlCeDataReader rdr = null;

            try
            {
                SqlCeCommand cmd = new SqlCeCommand("select * from episode where animeId=1", con);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    mw.lbDatabase_Add(rdr.GetName(1) + '\t' + rdr.GetName(2) + '\t' + rdr.GetName(3) + '\t' + rdr.GetName(4) + "\t\t" + rdr.GetName(5) + '\t' + rdr.GetName(6) + '\t' + rdr.GetName(7) + '\t' + rdr.GetName(8) + '\t' + rdr.GetName(9) + '\t' + rdr.GetName(10) + '\t' + rdr.GetName(11) + '\t' + rdr.GetName(12) + '\t' + rdr.GetName(13) + '\t' + rdr.GetName(14) + '\t' + rdr.GetName(15) + '\t' + rdr.GetName(16) + '\t' + rdr.GetName(17) + '\t' + rdr.GetName(18) + '\t' + rdr.GetName(19));
                }
            }
            catch (Exception)
            {
                mw.lbDatabase_Add("Check Statement syntax");
            }
            try
            {
                SqlCeCommand cmd = new SqlCeCommand("select * from episode", con);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string temp = "";
                    for (int i = 1; i < rdr.FieldCount;i++)
                    {
                        temp += rdr[i].ToString() + "\t";
                        if (i == rdr.FieldCount - 1) {
                            temp += rdr[i].ToString();
                        }
                        else if (i == 1) {
                            temp += '\t';
                        }
                    }
                    mw.lbDatabase_Add(temp);
                }
            }
            catch (Exception)
            {
                mw.lbDatabase_Add("Check Statement syntax");
            }
            finally
            {
                // close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }
            }
        }
    }
}
