using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Question
    {
        public long Id { get; set; }
        public string Text_EN { get; set; }
        public string Slug_EN { get; set; }
        public string ImagePath { get; set; }

        [JsonIgnore]
        public Quiz Quiz { get; set; }
        public long QuizId { get; set; }
        public int? Duration { get; set; }

        public IList<Answer> Answers { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
