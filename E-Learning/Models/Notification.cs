using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Info { get; set; }
        public DateTime? DateTime { get; set; }
        public bool? IsSeen { get; set; }

    }
}
