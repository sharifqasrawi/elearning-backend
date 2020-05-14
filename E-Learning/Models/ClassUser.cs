using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class ClassUser
    {
        [JsonIgnore]
        public Class Class { get; set; }
        public string ClassId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public DateTime? EnrollDateTime { get; set; }

        [JsonIgnore]
        public Session CurrentSession { get; set; }
        public long? CurrentSessionId { get; set; }
        public string CurrentSessionSlug { get; set; }

    }
}
