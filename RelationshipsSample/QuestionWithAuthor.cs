using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Author
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class QuestionWithAuthor
    {
        public long ID { get; set; }

        public Author Author { get; set; }

        public string Text { get; set; }
        public string Answer { get; set; }
    }
}
