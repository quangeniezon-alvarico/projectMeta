//controller
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMetaAPI.Models;
using System.Diagnostics;

namespace ProjectMetaAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ShopController : ControllerBase
    {
        private readonly MetaDbContext _dbcontext;
        public ShopController(MetaDbContext _context)
        {
            _dbcontext = _context;
        }

        [HttpGet]
        [Route("Shop/Items")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _dbcontext.Products.FindAsync(id);
            if (product == null)
                return BadRequest("Product not found");
            return Ok(product);
        }

        [HttpPost]
        [Route("Shop/Update")]
        public async Task<ActionResult<List<Product>>> UpdateProduct(int id, [FromBody] ProductUpdateRequest request)
        {
            var exproduct = await _dbcontext.Products.FindAsync(id);
            if (exproduct == null)
                return NotFound("Product not found");

            exproduct.ProductName = request.ProductName;
            exproduct.Description = request.Description;
            exproduct.Price = request.Price;
            exproduct.Category = request.Category;
            exproduct.AvailableQuantity = request.AvailableQuantity;
            exproduct.ProductStatus = request.ProductStatus;

            await _dbcontext.SaveChangesAsync();

            return Ok(await _dbcontext.Products.ToListAsync());
        }
    }
}


