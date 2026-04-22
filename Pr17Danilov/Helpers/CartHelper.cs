using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr17Danilov.Helpers
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total => (Product.Price ?? 0) * Quantity * (1 - (Product.Discount ?? 0) / 100);
    }

    public static class CartHelper
    {
        public static ObservableCollection<CartItem> CartItems { get; set; } = new ObservableCollection<CartItem>();

        public static void AddToCart(Product product, int quantity = 1)
        {
            var existing = CartItems.FirstOrDefault(x => x.Product.Id == product.Id);
            if (existing != null)
                existing.Quantity += quantity;
            else
                CartItems.Add(new CartItem { Product = product, Quantity = quantity });
        }

        public static void RemoveFromCart(CartItem item)
        {
            CartItems.Remove(item);
        }

        public static decimal GetTotal() => CartItems.Sum(x => x.Total);

        public static void Clear() => CartItems.Clear();
    }
}
