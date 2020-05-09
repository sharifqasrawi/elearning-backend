using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Like
    {
        public long Id { get; set; }
        public long? CourseId { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }

        public long? CommentId { get; set; }

        [JsonIgnore]
        public Comment Comment { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public DateTime? LikeDateTime { get; set; }
    }
}
