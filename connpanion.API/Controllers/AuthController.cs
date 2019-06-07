using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using connpanion.API.Data;
using connpanion.API.DTOs;
using connpanion.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace connpanion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration, IMapper mapper)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTOForRegister userDTOForRegister)
        {
            // validate request

            userDTOForRegister.UserName = userDTOForRegister.UserName.ToLower();
            if (await _authRepository.isUserExists(userDTOForRegister.UserName))
                return BadRequest("Username already exists.");
            
            var userToCreate = new User { UserName = userDTOForRegister.UserName };
            var createdUser = await _authRepository.Register(userToCreate, userDTOForRegister.Password);

            return StatusCode(201); //TODO: CreatedAtRoute()
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTOForLogin userDTOForLogin) 
        {
            // try
            // {
            //    throw new Exception("Stop! Hammer time.");
            // }
            // catch (System.Exception)
            // {
            //     return StatusCode(500, "MC Hammer says you can not touch this!");
            // }

            var userFromRepository = await _authRepository.Login(userDTOForLogin.UserName.ToLower(), userDTOForLogin.Password);

            if (userFromRepository == null)
                return Unauthorized();

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepository.ID.ToString()),
                new Claim(ClaimTypes.Name, userFromRepository.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserDTOForList>(userFromRepository);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}