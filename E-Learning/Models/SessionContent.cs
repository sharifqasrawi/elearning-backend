using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class SessionContent
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public Session Session { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
