namespace ECommerceStore.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }

        public int TotalProducts { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public Dictionary<string, int> OrdersPerMonth { get; set; } = new();
    }
}
