﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Course
    {
        public long Id { get; set; }
        public string Title_EN { get; set; }
        public string Description_EN { get; set; }
        public string Prerequisites_EN { get; set; }
        public int Duration { get; set; }
        public string ImagePath { get; set; }
        public ApplicationUser Author { get; set; }
        public float? Price { get; set; }
        public bool IsFree { get; set; }
        public bool IsPublished { get; set; }
        public Category Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public virtual IEnumerable<Section> Sections { get; set; }

    }
}
