using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VerifyEmailForgotPassword.Data;
using VerifyEmailForgotPassword.Models;

namespace VerifyEmailForgotPassword.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext context;
        public UserController(DataContext context)
        {
            this.context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register
            (UserRegisterRequest request)
        {
            if (context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User Already Exists !");
            }
            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);
            var user = new User
            {
                Email = request.Email,
                PasswordHash=passwordHash,
                PasswordSalt=passwordSalt,
                VerificationToken=CreateRandomToken()
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok("User Successfully Created !");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login
         (UserLoginRequest request)
        {
            var user = await context.Users.
                FirstOrDefaultAsync(u => u.Email == 
                request.Email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            if (!VerifyPasswordHash(request.Password
              , user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong Password !");
            }
            if (user.VerifiedAt ==null)
            {
                return BadRequest("Not Verified !");
            }
          
            return Ok($"Welcome Back :{user.Email} ! :)");
        }
        [HttpPost("verify")]
        public async Task<IActionResult> Verify
      (string token)
        {
            var user = await context.Users.
                FirstOrDefaultAsync(u =>
                u.VerificationToken ==token);
            if (user == null)
            {
                return BadRequest("Invalid Token");
            }
            user.VerifiedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return Ok($"User Verified");
        }
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword
        (string email)
        {
            var user = await context.Users.
                FirstOrDefaultAsync(u =>
                u.Email == email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await context.SaveChangesAsync();
            return Ok($"You May Now Reset Your Password");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword
            (ResetPasswordRequest request)
        {
            var user = await context.Users.
                FirstOrDefaultAsync(u =>
                u.PasswordResetToken == request.Token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid Token");
            }
            CreatePasswordHash(request.Password,
                out byte[] passwordHash,out byte[] 
                passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await context.SaveChangesAsync();
            return Ok($"Password Successfully Reset");
        }
        private void CreatePasswordHash(string password
            ,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using(var hmac=new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash
                    (System.Text.Encoding.UTF8.GetBytes
                    (password));

            }
        }
        private bool VerifyPasswordHash(string password
          , byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash
                    (System.Text.Encoding.UTF8.GetBytes
                    (password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateRandomToken()
        {
            return Convert.ToString(RandomNumberGenerator.GetInt32(1,1000));
            //return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}

