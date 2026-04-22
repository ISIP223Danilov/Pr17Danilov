using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr17Danilov.Models
{
    public enum UserRole
    {
        Client,
        Master,
        Manager,
        Admin
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public bool IsFrozen { get; set; }
    }

    public class ServiceType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Comment { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsCompleted { get; set; }
        public int ClientId { get; set; }
        public int MasterId { get; set; }
        public int ServiceTypeId { get; set; }

        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }
        [ForeignKey("MasterId")]
        public virtual User Master { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }
    }

    public class MasterService
    {
        [Key]
        public int Id { get; set; }
        public int MasterId { get; set; }
        public int ServiceTypeId { get; set; }

        [ForeignKey("MasterId")]
        public virtual User Master { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }
    }

    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public int? ManufacturerId { get; set; }
        public int? ProductTypeId { get; set; }
        public double Rating { get; set; }
        public bool IsFrozen { get; set; }

        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer Manufacturer { get; set; }
        [ForeignKey("ProductTypeId")]
        public virtual ProductType ProductType { get; set; }
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PickupDate { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsCompleted { get; set; }

        [ForeignKey("ClientId")]
        public virtual User Client { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}