﻿using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions opt) : base(opt)
        {

        }

        public DbSet<AppUser> Users { get; set; }
    }
}
