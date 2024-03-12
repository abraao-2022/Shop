using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;
        
        public CategoryController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-agent", Location = ResponseCacheLocation.Any, Duration = 60)]
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get()
        {
            try
            {
                return Ok(await _context.Categories.AsNoTracking().ToListAsync());
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível listar as categorias"});
            }
        }
        
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-agent", Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            try
            {
                return Ok(await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id));
            }
            catch
            {
                return BadRequest(new {message = "Categoria não encontrada"});
            }
        }
        
        [HttpPost]
        [Route("")]
        // [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post([FromBody] Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível criar a categoria"});
            }
        }
        
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model)
        {
            if (id != model.Id)
            {
                return NotFound(new {message = "categoria invalida"});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new {message = "Este registro já foi atualizado"});
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível atualizar a categoria"});
            }
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new {message = "Categoria não encontrada"});
            }
            
            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok(new {message = "Categoria removida com sucesso"});
            }
            catch
            {
                return BadRequest(new {message = "Não foi possível remover a categoria"});
            }
        }
    }
}