using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFQuestionBasesRepository : IQuestionBasesRepository
    {
        class QuestionsContext : DbContext
        {
            public QuestionsContext()
                //: base(@"Data Source=.\SQLEXPRESS; Initial Catalog=tests; User Instance=False; Integrated Security=True;")
            {
            }

            /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Question>().ToTable("questions");
            }*/

            public DbSet<BasicQuestion> Questions { get; set; }
        }

        private QuestionsContext context = new QuestionsContext();

        public void Add(BasicQuestion question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public BasicQuestion GetQuestionById(long id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<BasicQuestion> GetAllQuestions()
        {
            return context.Questions.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM BasicQuestions");
        }
    }
}
