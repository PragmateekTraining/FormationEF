using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class QuestionnairePresentation
    {
        public long ID { get; set; }

        public string Title { get; set; }

        public IList<QuestionnairePresentationPage> Pages { get; set; }
    }

    public class QuestionnairePresentationPage
    {
        public long ID { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
    }
}
