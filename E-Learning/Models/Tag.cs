﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class Tag
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Name_FR { get; set; }

        public IList<CourseTag> CourseTags { get; set; }
    }
}
