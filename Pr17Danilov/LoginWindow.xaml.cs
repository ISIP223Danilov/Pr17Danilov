using BeautySalonApp.Models;
using Pr17Danilov.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pr17Danilov
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginBox.Text.Trim();
            var pass = PassBox.Password;

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == pass);

                if (user == null)
                {
                    ErrorText.Text = "Неверный логин или пароль";
                    return;
                }

                if (user.IsFrozen)
                {
                    ErrorText.Text = "Аккаунт заморожен";
                    return;
                }

                SessionService.CurrentUser = user;
                DialogResult = true;
                Close();
            }
        }
    }
}
