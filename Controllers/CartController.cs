using Microsoft.AspNetCore.Mvc;
using ProjectMetaAPI.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Directory = ProjectMetaAPI.Models.Directory;

namespace ProjectMetaAPI.Controllers
{

    [ApiController]
    [Route("api")]
    public class CartController : ControllerBase
    {
        private readonly MetaDbContext _dbcontext;
        public CartController(MetaDbContext _context)
        {
            _dbcontext = _context;
        }

        [HttpPost]
        [Route("order/submit")]
        public async Task<IActionResult> PostCart(int Customer_ID, int Product_Id, int quantity, string Delivery_Status, int Mode_of_Payment)
        {
            Cart cus = _dbcontext.Carts.FirstOrDefault(c => c.CustomerId == Customer_ID);
            Directory directory = _dbcontext.Directories.FirstOrDefault(m => m.Id == Mode_of_Payment);

            int newQuan = 0;
            int total = 0;
            var prod = _dbcontext.Products.FirstOrDefault(p => p.ProductId ==Product_Id);
            if(prod == null)
            {
                return BadRequest(new { message = "Product ID does not exist" });
            }
            else
            {
                var price = _dbcontext.Products.FirstOrDefault(pr=>pr.ProductId == Product_Id);
                newQuan = quantity;
                decimal vOut = Convert.ToDecimal(newQuan);
                total = (int)(newQuan * price.Price);
            }

            var ord = new Order()
            {
                CustomerId = Customer_ID,
                TotalAmount = total,
                DeliveryStatus = Delivery_Status
            };
            var list = await _dbcontext.Orders.FirstOrDefaultAsync(or => or.CustomerId == ord.CustomerId);

            if (cus!=null && directory!=null)
            {
                if (list != null)
                {
                    return BadRequest(new { Message = "Customer ID already exists" });
                }
                else
                {
                    _dbcontext.Add(ord);
                    _dbcontext.SaveChanges();
                    return Ok(new { Message = "Success Submission", ord.OrderId });
                }
            }
            if (cus == null)
            {
                return BadRequest(new { Message = "Customer ID does not exist" });
            }
            if (directory == null)
            {
                return BadRequest(new { Message = "Mode of Payment does not exist" });
            }

            return BadRequest("Submission Failed");
        }

        [HttpGet]
        [Route("cart/remove")]
        public async Task<IActionResult> RemoveCart(int ID, int Order_ID)
        {
            Customer cust = _dbcontext.Customers.FirstOrDefault(c => c.CustomerId == ID);
            Order order = _dbcontext.Orders.FirstOrDefault(o => o.OrderId == Order_ID);
            Order orderlist = _dbcontext.Orders.FirstOrDefault(o => o.OrderId == Order_ID && o.CustomerId == ID);
            var Delcart = _dbcontext.Carts.Where(cr => cr.CustomerId == ID).FirstOrDefault();

            if (cust != null && orderlist != null)
            {
                _dbcontext.Carts.Remove(Delcart);
                _dbcontext.SaveChanges();
                return Ok(new { Message = "Success" });
            }
            if (cust == null)
            {
                return BadRequest(new { Message = "Invalid Customer ID or Customer ID does not exist" });
            }
            if (order == null)
            {
                return BadRequest(new { Message = "Invalid Order ID or Order ID does not exist" });
            }
            if (orderlist == null)
            {
                return BadRequest(new { Message = "Customer ID does not match with Order ID" });
            }
            if (cust == null && order == null)
            {
                return BadRequest(new { Message = "Customer ID and Order ID do not exist" });
            }

            return BadRequest(new { Message = "Deletion Failed. Please try again" });

        }

        [HttpGet]
        [Route("cart/view")]
        public IActionResult ViewTransaction(string userId)
        {
            using var context = new MetaDbContext();
            var transaction = context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new
                {
                    t.TransId,
                    t.ProductId,
                    t.Product.ProductName,
                    t.TransQuan,
                    t.TransTotal
                })
                .ToList();

            if (transaction == null || !transaction.Any())
            {
                return NotFound();
            }

            return Ok(transaction);
        }
        [HttpPost]
        [Route("Cart/Edit")]
        public IActionResult EditProductInCart(string userId, int currentProductId, int newProductId, int quantity)
        {
            int newPrice = 0;
            int newQuan = 0;
            var exTrans = _dbcontext.Transactions.FirstOrDefault(c => c.UserId == userId && c.ProductId == currentProductId);
            if (exTrans == null)
            {
                return Ok(new { message = "Transaction does not exit." });
            }
            else
            {
                var product = _dbcontext.Products.FirstOrDefault(c => c.ProductId == exTrans.ProductId);
                newQuan = quantity;
                decimal vOut = Convert.ToDecimal(newQuan);
                newPrice = (int)(newQuan * product.Price);
            }



            var transaction = _dbcontext.Transactions
                .FirstOrDefault(t => t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            var currentTrans = _dbcontext.Transactions.FirstOrDefault(td => td.ProductId == currentProductId);

            if (currentTrans == null)
            {
                return NotFound();
            }

            var newProduct = _dbcontext.Products.FirstOrDefault(p => p.ProductId == newProductId);

            if (newProduct == null)
            {
                return NotFound();
            }
            string quan = quantity.ToString();
            transaction.ProductId = newProductId;
            transaction.TransTotal = newPrice;
            transaction.TransQuan = quan;
            transaction.Product = newProduct;

            _dbcontext.SaveChanges();

            return Ok();
        }
    }
}