using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class SavedSession
    {
        public long Id { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public Session Session { get; set; }
        public long? SessionId { get; set; }

        public DateTime? SaveDateTime { get; set; }
    }
}
