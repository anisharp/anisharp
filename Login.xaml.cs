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
using System.Windows.Shapes;

namespace AniSharp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public String sUser
        {
            get { return tbUser.Text; }
        }
        public String sPassword
        {
            get { return pbPassword.Password; }
        }
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sUser) || string.IsNullOrWhiteSpace(sPassword))
                this.DialogResult = false;
            else
                this.DialogResult = true;
        }
    }
}
