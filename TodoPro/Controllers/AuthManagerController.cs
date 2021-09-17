using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoProCore.Configuration;
using TodoProCore.Dtos.Reponses;
using TodoProCore.Dtos.Requests;

namespace TodoPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagerController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthManagerController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> options)
        {
            _userManager = userManager;
            _jwtConfig = options.Value;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto user)
        {
            if (ModelState.IsValid)
            {
                var existUser = await _userManager.FindByEmailAsync(user.Email);
                var result = await _userManager.CheckPasswordAsync(existUser, user.PassWord);
                if (!result)
                {
                    return BadRequest(new RegistResponse()
                    {
                        Errors = new List<string>()
                        {
                             "Invalid payload!"
                        },
                        Success = false
                    });
                }
                var jwtToken = GenerrateJwtToken(existUser);
                return Ok(new RegistResponse()
                {
                    Success = true,
                    Token = jwtToken
                });
            }
            return BadRequest(new RegistResponse()
            {
                Errors = new List<string>()
                 {
                     "Invalid payload!"
                 },
                Success = false
            });
          
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistDto user)
        {
            if (ModelState.IsValid)
            {
                var existUser = await _userManager.FindByEmailAsync(user.Email);
                if (existUser != null)
                {
                    return BadRequest(new RegistResponse()
                    {
                        Errors = new List<string>()
                        {
                             "该邮箱已被注册"
                         },
                        Success = false
                    });
                }
                var newUser = new IdentityUser()
                {
                    Email = user.Email,
                    UserName = user.UserName
                };
                var result = await _userManager.CreateAsync(newUser, user.PassWord);
                if (result.Succeeded)
                {
                    var jwtToken = GenerrateJwtToken(newUser);
                    return Ok(new RegistResponse() { 
                        Success=true,
                        Token=jwtToken
                    });
                }
                else
                {
                    return BadRequest(new RegistResponse()
                    {
                        Errors = result.Errors.Select(x => x.Description).ToList()
                    }); ;
                }
            }
            return BadRequest(new RegistResponse()
            {
                Errors = new List<string>()
                 {
                     "Invalid payload!"
                 },
                Success = false
            });
        }
        private string GenerrateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Sercret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
