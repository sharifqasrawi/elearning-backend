using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Dtos.Users
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsAuthor { get; set; }
        public bool? EmailConfirmed { get; set; }
    }
}
