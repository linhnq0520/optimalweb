using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace optimalweb.Models
{
    public class AppUser : IdentityUser
    {
        public string? Address { get; set; }
    }
}