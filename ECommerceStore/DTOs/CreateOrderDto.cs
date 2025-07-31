namespace ECommerceStore.DTOs
{
    public class CreateOrderDto
    {
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
