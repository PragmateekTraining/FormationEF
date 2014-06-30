using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class QuestionBase
    {
        public long ID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public bool IsOptional { get; set; }
    }
}
