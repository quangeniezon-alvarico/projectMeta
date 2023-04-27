using Microsoft.AspNetCore.Mvc;
using ProjectMetaAPI.Models;
using System.Diagnostics;
using System.Net;
using System.Data.SqlTypes;

namespace ProjectMetaAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProdController : ControllerBase
    {
        private readonly MetaDbContext _dbcontext;
        public ProdController(MetaDbContext _context)
        {
            _dbcontext = _context;
        }

        [HttpPost]
        [Route("inventory/add")]
        public async Task<IActionResult> PostProduct(string Prod_Name, string Prod_Description, decimal Prod_Price, string Prod_category, int Availability, string Prod_Status)
        {
            var product = new Product()
            {
                ProductName = Prod_Name,
                Description = Prod_Description,
                Price = Prod_Price,
                Category = Prod_category,
                AvailableQuantity = Availability,
                ProductStatus = Prod_Status
                
            };
            if (Prod_Name != null && Prod_Description != null && Prod_Price != null && Prod_category != null && Availability != null && Prod_Status != null)
            {
                _dbcontext.Products.Add(product);
                _dbcontext.SaveChanges();

                return Ok(new { product.ProductId ,Message = "Success"});
            }
            return NotFound();
            
        }
    }
}