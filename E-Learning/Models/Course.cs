using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Course
    {
        public long Id { get; set; }
        public string Title_EN { get; set; }
        public string Slug_EN { get; set; }
        public string Description_EN { get; set; }
        public string Prerequisites_EN { get; set; }
        public string Languages { get; set; }
        public string Level { get; set; }
        public int Duration { get; set; }
        public string ImagePath { get; set; }
        public float? Price { get; set; }
        public bool? IsFree { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public Category Category { get; set; }

       //[JsonIgnore]
       // public ApplicationUser Author { get; set; }
        public IList<Section> Sections { get; set; }
        public IList<CourseTag> CourseTags { get; set; }
        public IList<Like> Likes { get; set; }
        public IList<Comment> Comments { get; set; }

    }
}
