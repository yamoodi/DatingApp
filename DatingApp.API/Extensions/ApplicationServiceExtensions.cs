using DatingApp.API.Data;
using DatingApp.API.Interfaces;
using DatingApp.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Helpers;

namespace DatingApp.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(Configuration.GetConnectionString("con"));
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.AddScoped<IPhotoService, PhotoService>();
            return services;
        }
    }
}
