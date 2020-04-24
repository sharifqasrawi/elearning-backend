using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class CourseTag
    {
        public long CourseId { get; set; }
        public Course Course { get; set; }


        public long TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
