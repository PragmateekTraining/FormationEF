using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RepositoryPatternSample
{
    public class EFQuestionsRepository : IQuestionsRepository
    {
        class QuestionsContext : DbContext
        {
            public QuestionsContext()
                //: base(@"Data Source=.\SQLEXPRESS; Initial Catalog=tests; User Instance=False; Integrated Security=True;")
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Question>().ToTable("questions");
            }

            public DbSet<Question> Questions { get; set; }
        }

        private QuestionsContext context = new QuestionsContext();

        public void Add(Question question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public Question GetQuestionById(long id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<Question> GetAllQuestions()
        {
            return context.Questions.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM questions");
        }
    }
}
