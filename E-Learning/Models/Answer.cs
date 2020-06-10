using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Answer
    {
        public long Id { get; set; }
        public string Text_EN { get; set; }
        public string Text_FR { get; set; }
        public string ImagePath { get; set; }
        public bool? IsCorrect { get; set; }

        [JsonIgnore]
        public Question Question { get; set; }
        public long QuestionId { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
