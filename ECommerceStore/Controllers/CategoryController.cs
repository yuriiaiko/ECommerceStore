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
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        //GET: /api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var category = await _context.Categories.ToListAsync();
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        //GET : api/category{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        //POST: api/category
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        //PUT: api/category/5
        [Authorize (Roles ="Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if(category == null)
                return NotFound();
            category.Name = dto.Name;
            

            await _context.SaveChangesAsync();  
            return Ok(category);

        }

        //DELETE: api/category/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(category);

        }
    }
}
