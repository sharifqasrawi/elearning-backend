using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class DoneSession
    {
        public long Id { get; set; }

        [JsonIgnore]
        public Session Session { get; set; }
        public long? SessionId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserID { get; set; }
        public DateTime? DoneDateTime { get; set; }

    }
}
