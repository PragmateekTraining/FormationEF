using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSample
{
    public class QuestionsRepository
    {
        class QuestionsContext : DbContext
        {
            public DbSet<Question> Questions { get; set; }
        }

        private QuestionsContext context = new QuestionsContext();

        public void AddQuestion(Question question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public Question GetQuestionById(int id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<Question> GetAllQuestions()
        {
            return context.Questions.ToList();
        }
    }
}
