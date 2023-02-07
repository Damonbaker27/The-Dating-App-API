﻿using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {
        private readonly DataContext _context;

        //Inject datacontext class into 
        public AccountController(DataContext context)
        {
            _context = context;
        }


        [HttpPost("register")] //  POST: api/account/register
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");
                    
            using var hmac = new HMACSHA512(); 

            var user = new AppUser()
            {
                UserName = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;

        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=> 
                x.UserName == loginDto.UserName);

            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);


            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Incorrect Password");


            }



        }




        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }




    }
}
