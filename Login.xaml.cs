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

        /// <summary>
        /// Standard Konstruktor der unteranderem den Usernamen aus der Settings File liest.
        /// </summary>
        public Login()
        {
            InitializeComponent();
            tbUser.Text = AniSharp.Properties.Settings.Default.Username ?? "";
        }

        /// <summary>
        /// Ueberprueft ob User und Password nicht leer sind, ist dies der fall gibt er als Ergebnis true zurueck, ansonst false. Das Ergebnis wird in der MainWindow btLogin_Click funktion weiterverarbeitet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginCommand_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sUser) || string.IsNullOrWhiteSpace(sPassword))
                this.DialogResult = false;
            else
            {
                this.DialogResult = true;
                /* uncomment if ready to publish
                 * if (AniSharp.Properties.Settings.Default.Username != tbUser.Text)
                   {
                 *      AniSharp.Properties.Settings.Default.Username = tbUser.Text;
                 *      AniSharp.Properties.Settings.Default.Save();
                 * }
                 */
            }
        }

        /// <summary>
        /// Wenn innerhalb des Password Felds Enter gedrueckt wird loest dieses Event das LoginCommand_Click Event aus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginCommand_Click(this, null);
        }
    }
}
