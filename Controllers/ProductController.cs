using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-agent", Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                return Ok(await _context
                    .Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .ToListAsync());
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível listar os produtos"});
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-agent", Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            try
            {
                return Ok(await _context
                    .Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id));
            }
            catch
            {
                return NotFound(new {message = "Produto não encontrado"});
            }
        }
        
        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-agent", Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id)
        {
            try
            {
                return Ok(await _context
                    .Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .Where(x => x.CategoryId == id)
                    .ToListAsync());
            }
            catch
            {
                return NotFound(new {message = "Categoria não encontrada"});
            }
        }
        
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post([FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível criar o produto"});
            }
        }
    }
}