using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DevIO.Api.Controllers;
using DevIO.Api.Data;
using DevIO.Api.DTO;
using DevIO.Api.Extensions;
using DevIO.Business.Interfaces;
using DevIO.Business.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace DevIO.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Account")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appsettings;
        private ApplicationDbContext _context { get; set; }
        public AuthController(INotificador notificador,
                                SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager,
                                IOptions<AppSettings> appsettings,
                                ApplicationDbContext context,
                                IUser appUser) : base(notificador, appUser)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appsettings = appsettings.Value;
            _context = context;
        }
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserViewModel userRegister)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            var user = new IdentityUser
            {
                Email = userRegister.Email,
                UserName = userRegister.Email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, userRegister.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,"Admin");

                await _signInManager.SignInAsync(user, false);
                return CustomResponse(await GerarJwt(userRegister.Email));
            }
            else
            {
                foreach (var erro in result.Errors)
                {
                    NotificarErro(erro.Description);
                }
            }
            return CustomResponse(userRegister);
        }
        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
            if (result.Succeeded)
            {
                CustomResponse(login);
                return CustomResponse(await GerarJwt(login.Email));
            }
            if (result.IsLockedOut)
            {
                NotificarErro("Usuário bloqueado temporariamente por tentativas invalidas.");
                CustomResponse(login);
            }
            NotificarErro("Usuário ou Senha incorreto.");
            return CustomResponse(login);
        }
        private async Task<LoginResponseViewModel> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            //Pegando role do usuário
            var roles = await _userManager.GetRolesAsync(user);
            var roleId = _context.Roles.FirstOrDefault(x => roles.Contains(x.Name))?.Id;
            //Pegando todos os claims dessa role
            var roleClaims = _context.RoleClaims.Where(x => x.RoleId == roleId).ToList();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in roles)
            {
                claims.Add(new Claim("role", userRole));
            }
            foreach (var roleClaim in roleClaims)
            {
                claims.Add(new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
            }
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);
            //Para manipular o token
            var tokenHandle = new JwtSecurityTokenHandler();
            //Key
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
            //Gerar o token
            var token = tokenHandle.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appsettings.Emissor,
                Audience = _appsettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appsettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });
            //Escrever o token. Serializar no padrão da web
            var encodedToken = tokenHandle.WriteToken(token);
            var filtro = new List<string>(){
                new string("sub"),
                new string("jti"),
                new string("nbf"),
                new string("iat"),
                new string("iss"),
                new string("aud"),
                new string("email"),
            };
            return new LoginResponseViewModel
            {
                AccessToken = encodedToken,
                ExpiratioIn = TimeSpan.FromHours(_appsettings.ExpiracaoHoras).TotalSeconds,
                UserToken = new UserTokenViewModel
                {
                    Email = email,
                    UserId = user.Id,
                    Claims = claims.Select(x => new ClaimsViewModel { Type = x.Type, Value = x.Value }).Where(x => !filtro.Contains(x.Type))
                }
            };
        }
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}