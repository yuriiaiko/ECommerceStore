using ECommerceStore.Data;
using ECommerceStore.DTOs;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        //GET : /api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductQueryParameters query)
        {
            var productsQuery = _context.Products.AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(query.Search) ||
                    p.Description.Contains(query.Search));
            }

            // 📂 Category Filter
            if (query.CategoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            // 🧮 Total count before pagination
            var totalCount = await productsQuery.CountAsync();

            // 📄 Pagination
            var products = await productsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // 📨 Return response with pagination metadata in header
            Response.Headers.Append("X-Total-Count", totalCount.ToString());


            return Ok(products);
        }

        

        //GET : /api/product/{id}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        //POST: /api/product
        [Authorize (Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId

            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        // PUT: /api/product/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        // DELETE: /api/product/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Deleted successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadProductImage([FromForm] ProductImageUploadDTO dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return NotFound("Product not found.");

            if (dto.Image == null || dto.Image.Length == 0)
                return BadRequest("No image uploaded.");

            // 📁 Ensure the images folder exists
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            // 📝 Create unique file name
            var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
            var filePath = Path.Combine(imageFolder, fileName);

            // 💾 Save the file to wwwroot/images
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            // 🌍 Build the public URL
            var imageUrl = $"/images/{fileName}";
            product.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();

            return Ok(new { imageUrl });
        }

    }
}
