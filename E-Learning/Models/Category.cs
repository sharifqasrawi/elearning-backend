using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Title_EN { get; set; }
        public string Title_FR { get; set; }

        public string Slug { get; set; }
        public string Slug_FR { get; set; }
        public string ImagePath { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        [JsonIgnore]
        public IList<Course> Courses { get; set; }

    }
}
