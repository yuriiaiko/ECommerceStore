﻿namespace ECommerceStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        // Navigation
       // public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
