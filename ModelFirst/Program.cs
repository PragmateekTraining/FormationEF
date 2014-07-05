using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            long id = 0;

            using (QuestionnaireModelContainer context = new QuestionnaireModelContainer())
            {
                ModelFirstQuestion question = new ModelFirstQuestion
                {
                    CreationDate = DateTime.UtcNow,
                    Text = "1 + 1?",
                    Answer = "2"
                };

                context.ModelFirstQuestions.Add(question);

                context.SaveChanges();

                id = question.Id;
            }

            using (QuestionnaireModelContainer context = new QuestionnaireModelContainer())
            {
                ModelFirstQuestion question = context.ModelFirstQuestions.Find(id);
            }
        }
    }
}
