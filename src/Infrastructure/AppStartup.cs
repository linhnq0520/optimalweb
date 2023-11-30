using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using optimalweb.Data;
using optimalweb.Models;

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
			services.AddDefaultIdentity<AppUser>().AddEntityFrameworkStores<AppDbContext>()
					.AddDefaultTokenProviders();
			return services;
		}
	}
}