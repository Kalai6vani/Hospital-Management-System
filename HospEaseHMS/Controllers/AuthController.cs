using HospEaseHMS.Data;
using HospEaseHMS.DTOs;
using HospEaseHMS.Models;
using HospEaseHMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospEaseHMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly AuthService _authService;

        public AuthController(HospitalDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        //register as patient or doctor only
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            //AnyAsync - checks if ANY elements in a sequence satisfy a specified condition
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("Email already exists.");

            //HashPassword - Hashes the password using SHA256
            var hashedPassword = _authService.HashPassword(model.Password);

            User user;
            if (model.Role == "Admin")
            {
                if (await _context.Admins.AnyAsync()) return BadRequest("Only one admin allowed.");
                user = new Admin { Name = model.Name, Email = model.Email, PasswordHash = hashedPassword, Role = "Admin" };
            }
            else if (model.Role == "Doctor")
            {
                user = new Doctor { Name = model.Name, Email = model.Email, PasswordHash = hashedPassword, Role = "Doctor", Specialization = "General" };
            }
            else if (model.Role == "Patient")
            {
                user = new Patient { Name = model.Name, Email = model.Email, PasswordHash = hashedPassword, Role = "Patient" };
            }
            else
            {
                return BadRequest("Invalid role. Choose Admin, Doctor, or Patient.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        //login using email and password 
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !_authService.VerifyPassword(model.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _authService.GenerateJWT(user);
            return Ok(new { Token = token, userId=user.Id, role=user.Role});
        }

        //get api for patients or doctors
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");
            return Ok(new { user.Id, user.Name, user.Email, user.Role });
        }

        //get api to get the users by roles
        [HttpGet]
        public IActionResult GetUsersByRole([FromQuery] string role)
        {
            var users = _context.Users.Where(u => u.Role == role).ToList();
            return Ok(users);
        }
    }
}
