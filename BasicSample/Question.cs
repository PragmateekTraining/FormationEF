using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSample
{
    public enum Category
    {
        Unspecified = 0,
        General,
        Computing,
        Sport,
        Cooking
    }

    public class Question
    {
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public bool IsOptional { get; set; }
    }
}
