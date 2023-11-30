using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using optimalweb.Data;
using optimalweb.Models;
using optimalweb.Services.Interfaces;
using optimalweb.Services.Services;

namespace optimalweb.Infrastructure
{
	public static class AppStartup
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<AppDbContext>(option =>
				{
					option.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
				});

			// Identity configuration
			services.AddIdentity<AppUser, IdentityRole>()
					.AddEntityFrameworkStores<AppDbContext>()
					.AddDefaultTokenProviders();
			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequiredUniqueChars = 1;

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				options.User.AllowedUserNameCharacters =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = true;

				options.SignIn.RequireConfirmedEmail = true;
				options.SignIn.RequireConfirmedPhoneNumber = false;
				options.SignIn.RequireConfirmedAccount = true;

			});

			services.AddScoped<ISendMailService, SendMailService>();
            services.AddOptions();
            var mailsetting = configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);

            return services;
		}
	}
}