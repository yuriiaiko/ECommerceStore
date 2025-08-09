namespace ECommerceStore.DTOs
{
    public class ProductImageUploadDTO
    {
        public int ProductId { get; set; }

        public IFormFile Image { get; set; } = null!;
    }
}
