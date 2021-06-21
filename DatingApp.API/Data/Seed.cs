using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public static class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userdata = await System.IO.File.ReadAllTextAsync("Data/DataSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userdata);
            foreach(var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHashed = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}
