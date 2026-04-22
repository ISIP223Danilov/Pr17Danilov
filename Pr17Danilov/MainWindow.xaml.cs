using Pr17Danilov.Models;
using Pr17Danilov.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pr17Danilov
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new StartPage());
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            if (SessionService.IsAuthenticated)
            {
                LoginBtn.Visibility = Visibility.Collapsed;
                AccountBtn.Visibility = Visibility.Visible;
                LogoutBtn.Visibility = Visibility.Visible;
            }
        }

        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SessionService.CurrentUser?.Role == UserRole.Client)
            {
                MainFrame.Navigate(new AccountPage());
            }
        }

        private void ProductsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionService.Logout();
            LoginBtn.Visibility = Visibility.Visible;
            AccountBtn.Visibility = Visibility.Collapsed;
            LogoutBtn.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new StartPage());
        }
    }
}
