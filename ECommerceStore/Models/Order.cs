namespace ECommerceStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        // Navigation Property
        public List<OrderItem> OrderItems { get; set; } = new();
        public User User { get; set; } = null!;

        // Payment related fields 
        public bool isPaid { get; set; } = false;
        public  DateTime PaymentDate { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
