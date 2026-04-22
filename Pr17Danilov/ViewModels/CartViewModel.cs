using Pr17Danilov.Commands;
using Pr17Danilov.Helpers;
using Pr17Danilov.Models;
using Pr17Danilov.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pr17Danilov.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        private readonly CosmicLodgeDBEntities _db;
        private ObservableCollection<CartItem> _cartItems;
        private DateTime _selectedPickupDate;
        private string _selectedPaymentMethod;
        private decimal _totalAmount;

        public CartViewModel()
        {
            _db = new CosmicLodgeDBEntities();
            CartItems = CartHelper.CartItems;
            SelectedPickupDate = DateTime.Today.AddDays(1);

            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem);
            IncreaseQuantityCommand = new RelayCommand(ExecuteIncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(ExecuteDecreaseQuantity);
            PlaceOrderCommand = new RelayCommand(ExecutePlaceOrder);
            GoBackCommand = new RelayCommand(ExecuteGoBack);
            ContinueShoppingCommand = new RelayCommand(ExecuteContinueShopping);

            CalculateTotal();
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set { _cartItems = value; OnPropertyChanged(); }
        }

        public DateTime SelectedPickupDate
        {
            get => _selectedPickupDate;
            set { _selectedPickupDate = value; OnPropertyChanged(); }
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set { _selectedPaymentMethod = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        public DateTime MinPickupDate => DateTime.Today.AddDays(1);
        public DateTime MaxPickupDate => DateTime.Today.AddDays(7);

        public RelayCommand RemoveItemCommand { get; }
        public RelayCommand IncreaseQuantityCommand { get; }
        public RelayCommand DecreaseQuantityCommand { get; }
        public RelayCommand PlaceOrderCommand { get; }
        public RelayCommand GoBackCommand { get; }
        public RelayCommand ContinueShoppingCommand { get; }

        private void CalculateTotal()
        {
            TotalAmount = CartHelper.GetTotal();
        }

        private void ExecuteRemoveItem(object parameter)
        {
            if (parameter is CartItem item)
            {
                CartHelper.RemoveFromCart(item);
                CalculateTotal();
            }
        }

        private void ExecuteIncreaseQuantity(object parameter)
        {
            if (parameter is CartItem item)
            {
                item.Quantity++;
                CalculateTotal();
            }
        }

        private void ExecuteDecreaseQuantity(object parameter)
        {
            if (parameter is CartItem item && item.Quantity > 1)
            {
                item.Quantity--;
                CalculateTotal();
            }
        }

        private void ExecutePlaceOrder(object parameter)
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Корзина пуста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                MessageBox.Show("Выберите способ оплаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Подтвердите оформление заказа", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var order = new Order
                {
                    ClientId = AuthService.CurrentUser.Id,
                    OrderDate = DateTime.Now,
                    PickupDate = SelectedPickupDate,
                    PaymentMethod = SelectedPaymentMethod,
                    IsCompleted = false,
                    TotalAmount = TotalAmount
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var item in CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity
                    };
                    _db.OrderItems.Add(orderItem);
                }

                _db.SaveChanges();
                CartHelper.Clear();

                MessageBox.Show($"Заказ №{order.Id} успешно оформлен!\nДата получения: {SelectedPickupDate:dd.MM.yyyy}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteGoBack(object parameter)
        {
            NavigationService.GoBack();
        }

        private void ExecuteContinueShopping(object parameter)
        {
            NavigationService.NavigateTo(new ProductsPage());
        }
    }
}
