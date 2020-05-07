﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Comment
    {
        public long Id { get; set; }

        [JsonIgnore]
        public Course Course { get; set; }
        public long CourseId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime? CommentDateTime { get; set; }

    }
}