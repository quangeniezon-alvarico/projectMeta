using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ProjectMetaAPI
{
    public class JWtmanager : IJWT
    {
        private readonly IConfiguration _configuration;
        public JWtmanager(IConfiguration configuration)
        { 
            _configuration = configuration;
        }

        //public ClaimsIdentity Subject { get; private set; }

        public string Authenticate(string username, string password)
        {
            if (!Token.Users.Any(x => x.Key.Equals(username) && x.Value.Equals(password)))
                return null;
                
            var key = _configuration.GetValue<string>("Jwt=: Key");
            var keybyte = Encoding.ASCII.GetBytes(key);

            var tokenhandler = new JwtSecurityTokenHandler();

            var tokendesc = new SecurityTokenDescriptor()
            {
               Subject = new ClaimsIdentity(new Claim[]
               {
                   new Claim(ClaimTypes.NameIdentifier, username)
               }),
               Expires= DateTime.UtcNow.AddMinutes(10),
               SigningCredentials = new SigningCredentials 
               (new SymmetricSecurityKey(keybyte), SecurityAlgorithms.HmacSha512Signature)
            };
            
            var token = tokenhandler.CreateToken(tokendesc);
            return tokenhandler.WriteToken(token);
        }
    }
}
