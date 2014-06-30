using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class QuestionMetadata
    {
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public string Comment { get; set; }
    }

    public class Nomination
    {
        public long ID { get; set; }
        public string Title { get; set; }
    }

    public class QuestionWithMetadata
    {
        public long ID { get; set; }

        public QuestionMetadata Metadata { get; set; }
        public Nomination Nomination { get; set; }

        public string Text { get; set; }
        public string Answer { get; set; }
    }
}
