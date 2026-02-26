using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstateAPI.Data;
using RealEstateAPI.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        #region GetAllUsers
        [HttpGet]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.SelectAllUser();
            return Ok(users.Select(user => new
            {
                user.UserID,
                user.UserName,
                user.Email,
                user.PhoneNo,
                user.Role
            }));
        }
        #endregion

        #region GetUserByID
        [HttpGet("{userID}")]
        [Authorize]
        public IActionResult GetUserByID(int userID)
        {
            var user = _userRepository.SelectUserByID(userID);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new
            {
                user.UserID,
                user.UserName,
                user.Email,
                user.PhoneNo,
                user.Role
            });
        }
        #endregion

        #region Register (Insert User)
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
                return BadRequest(new { message = "Password is required." });

            var existingUser = _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already registered." });

            var success = _userRepository.InsertUser(user);
            if (!success)
                return StatusCode(500, new { message = "Failed to register the user." });

            return Ok(new { message = "User registered successfully." });
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginUser)
        {
            if (string.IsNullOrWhiteSpace(loginUser.Email) || string.IsNullOrWhiteSpace(loginUser.Password))
                return BadRequest(new { message = "Email and Password are required." });

            var user = _userRepository.LoginUser(loginUser.Email, loginUser.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("Role", user.Role),
                    new Claim("Password", user.Password)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Ok(new
            {
                message = "Login successful.",
                token = jwtToken,
                user = new
                {
                    user.UserID,
                    user.UserName,
                    user.Email,
                    user.PhoneNo,
                    user.Role,
                    user.Password
                }
            });
        }
        #endregion

        #region Update User
        [HttpPut("{userID}")]
        [Authorize]
        public IActionResult UpdateUser(int userID, [FromBody] UserModel user)
        {
            if (userID != user.UserID)
                return BadRequest(new { message = "UserID mismatch." });

            var existingUser = _userRepository.SelectUserByID(userID);
            if (existingUser == null)
                return NotFound(new { message = "User not found." });

            var success = _userRepository.UpdateUser(user);
            if (!success)
                return StatusCode(500, new { message = "Failed to update user." });

            return Ok(new { message = "User updated successfully." });
        }
        #endregion

        #region DeleteUser
        [HttpDelete("{userID}")]
        [Authorize]
        public IActionResult DeleteUser(int userID)
        {
            var existingUser = _userRepository.SelectUserByID(userID);
            if (existingUser == null)
                return NotFound(new { message = "User not found." });

            var success = _userRepository.DeleteUser(userID);
            if (!success)
                return StatusCode(500, new { message = "Failed to delete user." });

            return Ok(new { message = "User deleted successfully." });
        }
        #endregion
    }
}
