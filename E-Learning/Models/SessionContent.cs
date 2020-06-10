using Newtonsoft.Json;
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
        public string Content_FR { get; set; }
        public int Order { get; set; }

        [JsonIgnore]
        public Session Session { get; set; }
        public long SessionId { get; set; }
        public string Note { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
