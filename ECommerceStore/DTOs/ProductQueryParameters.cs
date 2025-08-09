namespace ECommerceStore.DTOs
{
    public class ProductQueryParameters
    {
        public int Page { get; set; } = 1;            // Default to first page
        public int PageSize { get; set; } = 10;       // Default page size

        public string? Search { get; set; }           // Optional search term
        public int? CategoryId { get; set; }          // Optional category filter
    }
}
