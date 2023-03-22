using Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebAPIs.Models;
using WebAPIs.Token;

namespace WebAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/CreateTokenIdentity")]
        public async Task<IActionResult> CreateTokenIdentity([FromBody] Login login)
        {
            if (!ModelState.IsValid) return BadRequest("Request Invalid");

            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.password)) 
            {
                return Unauthorized();
            }

            var result = await
            _signInManager.PasswordSignInAsync(login.email, login.password, false, true);

            if (result.Succeeded)
            {
                // Recupera Usuario logado

                var userCurrent = await _userManager.FindByEmailAsync(login.email);

                var idUser = userCurrent?.Id;

                var token = new TokenJwtBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create("Secret_Key-123456789"))
                    .AddSubject("Empresa")
                    .AddIssuer("Teste.security")
                    .AddAudience("Teste.Security")
                    .AddClaim("idUser", idUser)
                    .AddExpiryInMinutes(5)
                    .Builder();

                return Ok(token.value);

            }
            else
            {
                return Unauthorized();
            }
            
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AddUserIdentity")]
        public async Task<IActionResult> AddUserIdentity([FromBody] Login login) 
        {
            if (!ModelState.IsValid) return BadRequest("Request Invalid");

            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.password))
            {
                return BadRequest("Faltam alguns dados");
            }

            var user = new ApplicationUser
            {
                UserName = login.email,
                Email = login.email,
                CPF = login.password,
                Type =  Entities.Enums.TypeUser.Commom
            };

            var result = await
                _userManager.CreateAsync(user, login.password);
            
            if (result.Errors.Any()) 
            {
                return Ok(result.Errors);
            }

            // Geracao de confirmacao caso precise

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // retorno email

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result2 = await _userManager.ConfirmEmailAsync(user, code);

            if (result2.Succeeded)
            {
                return Ok("Usuario adicionado com sucesso");
            }
            else
            {
                return Ok("Erro ao confirmar usuario");
            }
        }
    }
}
