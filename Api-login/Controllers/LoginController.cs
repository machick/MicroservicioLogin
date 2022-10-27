using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Api_login.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DB;

namespace Api_login.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : Controller
    {
        public IConfiguration _configuration;
        private Grupo5Context _context;

        public LoginController(IConfiguration configuration, Grupo5Context context)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("mensaje")]
        public dynamic mensaje()
        {
            return new
            {
                code= 200,
                message = "hola mundo"
            };
        }

        [HttpPost]
        [Route("registerUser")]
        public dynamic registerUser([FromBody] Object optData)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());
            Usuario usuario = new Usuario
            {   
                User = data.user.ToString(),
                Password= data.password.ToString(),
                Fullname= data.fullname.ToString(),
            };
            
            _context.Usuarios.Add(usuario);
            

            _context.SaveChanges();
            return new
            {
                message= "guardado en teoria"
            };
        }

        [HttpPost]
        [Route("loginSesion")]
        public dynamic loginSesion([FromBody] Object optData)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());
            string user = data.user.ToString(); // datos del body 
            string password = data.password.ToString();
            // busco al usuario
                //metodo 1
                    //List<Usuario> listUsuario = _context.Usuarios.ToList();
                    //Usuario userObtenido = listUsuario.Find(x => x.User == user && x.Password == password);
                //metodo 2
                Usuario userObtenido = _context.Usuarios.Where(u => u.User == user && u.Password == password).FirstOrDefault<Usuario>();
            // si no hay tiro error
            if(userObtenido == null)
            {
                return new
                {
                    success = false,
                    message = "fracaso",
                    result = "Usuario o contraseña invalida"
                };

            }
            // si encuentra prosigo a crear token para enviarselo con sus datos

            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("id", userObtenido.IdUser.ToString()),
                new Claim("user", userObtenido.User.ToString()),
                new Claim("fullname", userObtenido.Fullname.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)); //clave secret del token

            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // firma del token creada con la clave secret

            var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(4),
                    signingCredentials: singIn
                    );

            return new
            {
                success = true,
                message = "exito",
                result = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        

    }
}
