using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Questionnaire
    {
        public long ID { get; set; }
        public string Title { get; set; }

        public IList<QuestionBase> Questions { get; set; }
    }
}
