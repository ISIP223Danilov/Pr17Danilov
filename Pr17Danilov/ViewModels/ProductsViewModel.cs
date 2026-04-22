using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Pr17Danilov.Commands;
using Pr17Danilov.Helpers;
using Pr17Danilov.Models;
using Pr17Danilov.Services;
using Pr17Danilov.Views;

namespace Pr17Danilov.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly CosmicLodgeDBEntities _db;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Product> _filteredProducts;
        private ObservableCollection<Manufacturer> _manufacturers;
        private ObservableCollection<ProductType> _productTypes;
        private string _searchText;
        private Manufacturer _selectedManufacturer;
        private ProductType _selectedProductType;
        private string _selectedSort;

        public ProductsViewModel()
        {
            _db = new CosmicLodgeDBEntities();
            LoadData();

            GoBackCommand = new RelayCommand(ExecuteGoBack);
            GoToCartCommand = new RelayCommand(ExecuteGoToCart);
            AddToCartCommand = new RelayCommand(ExecuteAddToCart);
            ShowProductDetailsCommand = new RelayCommand(ExecuteShowProductDetails);
            ApplyFiltersCommand = new RelayCommand(ExecuteApplyFilters);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufacturers;
            set { _manufacturers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set { _productTypes = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ExecuteApplyFilters(null); }
        }

        public Manufacturer SelectedManufacturer
        {
            get => _selectedManufacturer;
            set { _selectedManufacturer = value; OnPropertyChanged(); ExecuteApplyFilters(null); }
        }

        public ProductType SelectedProductType
        {
            get => _selectedProductType;
            set { _selectedProductType = value; OnPropertyChanged(); ExecuteApplyFilters(null); }
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set { _selectedSort = value; OnPropertyChanged(); ExecuteApplyFilters(null); }
        }

        public RelayCommand GoBackCommand { get; }
        public RelayCommand GoToCartCommand { get; }
        public RelayCommand AddToCartCommand { get; }
        public RelayCommand ShowProductDetailsCommand { get; }
        public RelayCommand ApplyFiltersCommand { get; }

        private void LoadData()
        {
            Products = new ObservableCollection<Product>(_db.Products.Where(p => p.IsFrozen != true).ToList());
            Manufacturers = new ObservableCollection<Manufacturer>(_db.Manufacturers.ToList());
            ProductTypes = new ObservableCollection<ProductType>(_db.ProductTypes.ToList());
            FilteredProducts = new ObservableCollection<Product>(Products);
        }

        private void ExecuteApplyFilters(object parameter)
        {
            var query = Products.AsEnumerable();

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()));

            // Фильтр по производителю
            if (SelectedManufacturer != null)
                query = query.Where(p => p.ManufacturerId == SelectedManufacturer.Id);

            // Фильтр по типу товара
            if (SelectedProductType != null)
                query = query.Where(p => p.ProductTypeId == SelectedProductType.Id);

            // Сортировка
            if (SelectedSort == "По возрастанию")
                query = query.OrderBy(p => p.Rating);
            else if (SelectedSort == "По убыванию")
                query = query.OrderByDescending(p => p.Rating);

            FilteredProducts = new ObservableCollection<Product>(query);
        }

        private void ExecuteAddToCart(object parameter)
        {
            if (!AuthService.IsAuthenticated)
            {
                MessageBox.Show("Пожалуйста, войдите в аккаунт для добавления товаров в корзину",
                    "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.NavigateTo(new LoginPage());
                return;
            }

            if (parameter is Product product)
            {
                CartHelper.AddToCart(product);
                MessageBox.Show($"{product.Name} добавлен в корзину", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteShowProductDetails(object parameter)
        {
            if (parameter is Product product)
            {
                var detailsWindow = new ProductDetailsWindow(product);
                detailsWindow.ShowDialog();
            }
        }

        private void ExecuteGoToCart(object parameter)
        {
            if (!AuthService.IsAuthenticated)
            {
                MessageBox.Show("Пожалуйста, войдите в аккаунт для просмотра корзины",
                    "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.NavigateTo(new LoginPage());
                return;
            }
            NavigationService.NavigateTo(new CartPage());
        }

        private void ExecuteGoBack(object parameter)
        {
            NavigationService.GoBack();
        }
    }
}