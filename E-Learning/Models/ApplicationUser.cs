using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string  Country { get; set; }
        public string Gender { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsAuthor { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public IList<ClassUser> ClassUsers { get; set; }

    }
}
