using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMetaAPI.Models;

namespace ProjectMetaAPI.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MetaDbContext _dbcontext;
        public AccountController(MetaDbContext _context)
        {
            _dbcontext = _context;
        }
        private readonly IJWT _jwtmanager;
        public AccountController(IJWT Jwtmanager)
        {
            _jwtmanager = Jwtmanager;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody]User credential)
        {
            var token = _jwtmanager.Authenticate(credential.Username, credential.Password);
            if (string .IsNullOrEmpty(token))
                return Unauthorized();
            return Ok(token);
        }
        //Veiw the customers Accounts
        [HttpGet]
        [Route("ACCOUNTS")]
        public async Task<IActionResult> GetUsers(string searchValue = null)
        {
            try
            {
                IQueryable<User> query = _dbcontext.Users;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(u => u.Firstname.Contains(searchValue) || u.Lastname.Contains(searchValue));
                }

                var listUsers = await query.ToListAsync();

                if (listUsers.Any())
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
        //For Update
        [HttpPost]
        [Route("Accounts/Update")]
        public async Task<IActionResult> UpdateUser(int id, Dictionary<string, string> changes)
        {
            try
            {
                var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                foreach (var change in changes)
                {
                    if (change.Key.Equals("Firstname", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Firstname = change.Value;
                    }
                    else if (change.Key.Equals("Middlename", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Middlename = change.Value;
                    }
                    else if (change.Key.Equals("Lastname", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Lastname = change.Value;
                    }
                    else if (change.Key.Equals("Phone", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Phone = change.Value;
                    }
                    else if (change.Key.Equals("Address", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Address = change.Value;
                    }
                    else
                    {
                        return BadRequest($"Invalid property name: {change.Key}");
                    }
                }
                _dbcontext.Users.Add(user);
                await _dbcontext.SaveChangesAsync();
                return Ok(new
                {
                    user.Id,
                    user.Firstname,
                    user.Middlename,
                    user.Lastname,
                    user.Phone,
                    user.Address,
                    Message = "UPDATE SUCCESS"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

