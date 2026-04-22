using Pr17Danilov.Commands;
using Pr17Danilov.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr17Danilov.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _login;
        private string _password;
        private string _errorMessage;
        private readonly CosmicLodgeDBEntities _db;

        public LoginViewModel()
        {
            _db = new CosmicLodgeDBEntities();
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public RelayCommand LoginCommand { get; }

        private void ExecuteLogin(object parameter)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Login == Login && u.Password == Password);

                if (user == null)
                {
                    ErrorMessage = "Неверный логин или пароль";
                    return;
                }

                if (user.IsFrozen == true)
                {
                    ErrorMessage = "Ваш аккаунт заморожен. Обратитесь к администратору.";
                    return;
                }

                AuthService.CurrentUser = user;

                switch (user.Role)
                {
                    case "Client":
                        NavigationService.NavigateTo(new StartPage());
                        break;
                    case "Master":
                        NavigationService.NavigateTo(new MasterPage());
                        break;
                    case "Manager":
                        NavigationService.NavigateTo(new ManagerPage());
                        break;
                    case "Admin":
                        NavigationService.NavigateTo(new AdminPage());
                        break;
                    default:
                        NavigationService.NavigateTo(new StartPage());
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }
    }
}
