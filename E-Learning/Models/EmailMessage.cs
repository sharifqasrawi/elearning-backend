using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class EmailMessage
    {
        public int Id { get; set; }
        public string Emails { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SendDateTime { get; set; }


    }
}
