using ProjectMetaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace ProjectMetaAPI.Controllers
{
    [ApiController]
    [Route("api/ACCOUNTS")]
    public class UserController : ControllerBase
    {
        private readonly MetaDbContext _dbcontext;
        public UserController(MetaDbContext _context)
        {
            _dbcontext = _context;
        }

        [HttpPost]
        [Route("ADD")]
        public async Task<IActionResult> PostCustomer(User customer)
        {
            //already existing account

            Dictionary<string, string> errors = new Dictionary<string, string>();
            //List<string> errors = new List<string>();
            var existingCustomer = await _dbcontext.Users.FirstOrDefaultAsync(c => c.Username == customer.Username);
            if (existingCustomer != null)
            {
                errors.Add("username", "Username already exist");
                //errors.Add(new string { username = "Username already exists", });
                //errors.Add("Username already exist");
                //return Ok(new { message = "Username already exists", customerId = existingCustomer.CustomerId });
            }

            var existingPhone = await _dbcontext.Users.FirstOrDefaultAsync(c => c.Phone == customer.Phone);
            if (existingPhone != null)
            {
                errors.Add("phone", "Phone Number already exist");
                //errors.Add("Phone Number already exist");
                //return Ok(new { message = "Phone Number already exists", customerId = existingPhone.CustomerId });
            }
            if (errors.Count != 0)
            {
                return BadRequest(new { errors = errors });
            }
            //==              
            //{
            //    "errors": {
            //        "username": "Username already exist",
            //         "phone": "Phone Number already exist"
            //    }
            //}

            //validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //initial
            var adduser = new User()
            {
                Id = customer.Id,
                Username = customer.Username,
                Password = customer.Password,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Address = customer.Address,
                Phone = customer.Phone,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbcontext.Users.Add(adduser);
            await _dbcontext.SaveChangesAsync();

            if (adduser != null)
            {
                return Ok(new { message = "Account created successfully", customerId = adduser.Id });
            }
            return Ok(new { message = "Account already exist" });
        }

        //Add account validation



        //[HttpGet]
        //[Route("Users")]
        //public async Task<IActionResult> GetUsers(string searchValue)
        //{
        //    try
        //    {

        //        // WITH SEARCHING    
        //        List<Customer> listUsers = (from customer in _dbcontext.Customers where customer.CustomerId.ToString().Contains(searchValue) || (customer.FirstName.ToString().Contains(searchValue)) || (customer.LastName.Contains(searchValue)) select customer).ToList();

        //        // GET ALL USERS
        //        //List<User> listUsers = _dbcontext.Users.ToList();

        //        if (listUsers != null)
        //        {
        //            return Ok(listUsers);
        //        }
        //        return Ok("No users found");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("Users/customerId")]
        public async Task<IActionResult> GetUsers(int customerId)
        {
            try
            {
                User user = await _dbcontext.Users.FindAsync(customerId);

                if (user != null)
                {
                    return Ok(user);
                }
                return Ok("No user found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> GetLogin(string username, string password)
        {
            try
            {


                var login = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

                if (login != null)
                {
                    return Ok(new { valid = true, message = "Successfully logged in" });
                }
                return Ok(new { valid = false, message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Shop/Update")]
        public async Task<ActionResult<List<User>>> UpdateProduct(int id, [FromBody] User request)
        {
            var exaccount = await _dbcontext.Users.FindAsync(id);
            if (exaccount == null)
                return NotFound("Product not found");

            exaccount.Username = request.Username;
            exaccount.Password = request.Password;
            exaccount.Firstname = request.Firstname;
            exaccount.Lastname = request.Lastname;
            exaccount.Address = request.Address;
            exaccount.Phone = request.Phone;

            await _dbcontext.SaveChangesAsync();

            return Ok(await _dbcontext.Customers.ToListAsync());

        }

        [HttpDelete("User/Delete")]
        public async Task<ActionResult<List<User>>> Delete(int id)
        {
            var delaccount = await _dbcontext.Users.FindAsync(id);
            if (delaccount == null)
                return NotFound("Product not found");

            _dbcontext.Users.Remove(delaccount);
            await _dbcontext.SaveChangesAsync();


            var login = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (login == null)
            {
                return Ok(new { valid = true, message = "Successfully Deleted" });
            }

            return Ok(await _dbcontext.Users.ToListAsync());

        }
    }
}



