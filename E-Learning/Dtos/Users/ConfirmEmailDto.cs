using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Dtos.Users
{
    public class ConfirmEmailDto
    {
        public string userId { get; set; }
        public string token { get; set; }

    }
}
