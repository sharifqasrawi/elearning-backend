using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Dtos.Classes
{
    public class Member
    {
        public string Id { get; set; }
        public string  FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public DateTime? EnrollDateTime { get; set; }
        public long? CurrentSessionId { get; set; }
        public string CurrentSessionSlug { get; set; }
    }

    public class ClassDto
    {
        public string Id { get; set; }
        public string Name_EN { get; set; }
        public long CourseId { get; set; }
        public IList<Member> Members { get; set; }
    }
}
