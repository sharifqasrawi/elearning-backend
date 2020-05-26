using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Quiz
    {
        public long Id { get; set; }
        public string title_EN { get; set; }
        public string Slug_EN { get; set; }
        public string ImagePath { get; set; }
        public int? Duration { get; set; }

        public string Description_EN { get; set; }
        public string Languages { get; set; }

        public IList<Question> Questions { get; set; }

        public bool? IsPublished { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
