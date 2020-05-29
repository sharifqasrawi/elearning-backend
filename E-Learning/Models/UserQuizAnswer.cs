using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning.Models
{
    public class UserQuizAnswer
    {
        public long Id { get; set; }

        [JsonIgnore]
        public UserQuiz UserQuiz { get; set; }
        public long UserQuizId { get; set; }

        [JsonIgnore]
        public Question Question { get; set; }
        public long QuestionId { get; set; }

        [JsonIgnore]
        public Answer Answer { get; set; }
        public long AnswerId { get; set; }
        public DateTime? ChooseDateTime { get; set; }

    }
}
