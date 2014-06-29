using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSample
{
    class Program
    {
        static void Main(string[] args)
        {
            QuestionsRepository repository = new QuestionsRepository();

            Question question = new Question
            {
                CreationDate = DateTime.UtcNow,
                Text = "Why is the sky blue?",
                Answer = "Because."
            };

            repository.AddQuestion(question);

            IEnumerable<Question> questions = repository.GetAllQuestions();
        }
    }
}
