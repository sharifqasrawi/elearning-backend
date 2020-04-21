using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Message
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsSeen { get; set; }
        public DateTime SeenDateTime { get; set; }


    }
}
