using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMetaAPI.Models;
using NuGet.Common;
using System.Diagnostics;

namespace ProjectMetaAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : ControllerBase
    {
        private readonly MetaDbContext _dbcontext;
        public HomeController(MetaDbContext _context)
        {
            _dbcontext = _context;
        }

        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetUsers(string searchValue)
        {
            try
            {

                // WITH SEARCHING    
                List<User> listUsers = (from user in _dbcontext.Users where (user.Id.ToString().Contains(searchValue)) select user).ToList();
                // GET ALL USERS
                //List<User> listUsers = _dbcontext.Users.ToList();

                if (listUsers != null)
                {
                    return Ok(listUsers);
                }
                return Ok("No users found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    return Ok(new { valid = true });
                }
                return Ok(new { valid = false, message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     
        }
}


