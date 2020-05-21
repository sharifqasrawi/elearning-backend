using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Report
    {
        public long Id { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string Type { get; set; }
        public string Severity { get; set; }
        public int? SeverityLevel { get; set; }
        public string Description { get; set; }
        public DateTime? ReportDateTime { get; set; }
        public bool? IsSeen { get; set; }


        public string ReplyMessage { get; set; }
        public DateTime? ReplyDateTime { get; set; }
        public bool? IsReplySeen { get; set; }
    }
}
