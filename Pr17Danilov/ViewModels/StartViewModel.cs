using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Pr17Danilov.Commands;
using Pr17Danilov.Models;
using Pr17Danilov.Services;
using Pr17Danilov.Views;

namespace Pr17Danilov.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        private readonly CosmicLodgeDBEntities _db;
        private ObservableCollection<ServiceType> _serviceTypes;
        private ObservableCollection<User> _masters;
        private ObservableCollection<MasterService> _masterServices;
        private ServiceType _selectedServiceType;
        private User _selectedMaster;
        private bool _isAuthenticated;

        public StartViewModel()
        {
            _db = new CosmicLodgeDBEntities();
            LoadData();

            GoToLoginCommand = new RelayCommand(ExecuteGoToLogin);
            GoToProductsCommand = new RelayCommand(ExecuteGoToProducts);
            GoToAccountCommand = new RelayCommand(ExecuteGoToAccount, _ => IsAuthenticated);
            SelectServiceCommand = new RelayCommand(ExecuteSelectService);
            SelectMasterCommand = new RelayCommand(ExecuteSelectMaster);
            BookCommand = new RelayCommand(ExecuteBook);
        }

        public ObservableCollection<ServiceType> ServiceTypes
        {
            get => _serviceTypes;
            set { _serviceTypes = value; OnPropertyChanged(); }
        }

        public ObservableCollection<User> Masters
        {
            get => _masters;
            set { _masters = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MasterService> MasterServices
        {
            get => _masterServices;
            set { _masterServices = value; OnPropertyChanged(); }
        }

        public ServiceType SelectedServiceType
        {
            get => _selectedServiceType;
            set { _selectedServiceType = value; OnPropertyChanged(); UpdateMasters(); }
        }

        public User SelectedMaster
        {
            get => _selectedMaster;
            set { _selectedMaster = value; OnPropertyChanged(); }
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set { _isAuthenticated = value; OnPropertyChanged(); }
        }

        public RelayCommand GoToLoginCommand { get; }
        public RelayCommand GoToProductsCommand { get; }
        public RelayCommand GoToAccountCommand { get; }
        public RelayCommand SelectServiceCommand { get; }
        public RelayCommand SelectMasterCommand { get; }
        public RelayCommand BookCommand { get; }

        private void LoadData()
        {
            ServiceTypes = new ObservableCollection<ServiceType>(_db.ServiceTypes.ToList());
            Masters = new ObservableCollection<User>(_db.Users.Where(u => u.Role == "Master" && u.IsFrozen != true).ToList());
            MasterServices = new ObservableCollection<MasterService>(_db.MasterServices.ToList());
            IsAuthenticated = AuthService.IsAuthenticated;
        }

        private void UpdateMasters()
        {
            if (SelectedServiceType == null) return;

            var masterIds = _db.MasterServices
                .Where(ms => ms.ServiceTypeId == SelectedServiceType.Id)
                .Select(ms => ms.MasterId)
                .ToList();

            Masters = new ObservableCollection<User>(
                _db.Users.Where(u => masterIds.Contains(u.Id) && u.Role == "Master" && u.IsFrozen != true)
            );
        }

        private void ExecuteGoToLogin(object parameter)
        {
            NavigationService.NavigateTo(new LoginPage());
        }

        private void ExecuteGoToProducts(object parameter)
        {
            NavigationService.NavigateTo(new ProductsPage());
        }

        private void ExecuteGoToAccount(object parameter)
        {
            NavigationService.NavigateTo(new AccountPage());
        }

        private void ExecuteSelectService(object parameter)
        {
            // Показываем мастеров для выбранной услуги
            UpdateMasters();
        }

        private void ExecuteSelectMaster(object parameter)
        {
            if (SelectedMaster != null && SelectedServiceType != null)
            {
                NavigationService.NavigateTo(new AppointmentsPage(SelectedMaster.Id, SelectedServiceType.Id));
            }
        }

        private void ExecuteBook(object parameter)
        {
            if (!IsAuthenticated)
            {
                MessageBox.Show("Пожалуйста, войдите в аккаунт для записи", "Вход required", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.NavigateTo(new LoginPage());
                return;
            }

            if (SelectedMaster == null || SelectedServiceType == null)
            {
                MessageBox.Show("Выберите услугу и мастера", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NavigationService.NavigateTo(new AppointmentsPage(SelectedMaster.Id, SelectedServiceType.Id));
        }
    }
}