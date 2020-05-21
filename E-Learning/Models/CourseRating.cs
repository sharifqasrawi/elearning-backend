using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class CourseRating
    {
        public long Id { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
        public long CourseId { get; set; }
        public float Value { get; set; }
        public float? OldValue{ get; set; }
        public DateTime? RateDateTime { get; set; }
        public DateTime? RateDateTimeUpdated { get; set; }
    }
}
