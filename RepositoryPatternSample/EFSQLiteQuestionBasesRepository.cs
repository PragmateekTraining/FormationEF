using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Repositories
{
    public class EFSQLiteQuestionBasesRepository : IQuestionBasesRepository
    {
        class QuestionsContext : DbContext
        {
            public QuestionsContext()
                : base("name=dbConnection")
            {
                //System.Data.Entity.Database.SetInitializer<QuestionsContext>(null);
            }

            public DbSet<BasicQuestion> Questions { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                /*modelBuilder.Conventions
                    .Remove<PluralizingTableNameConvention>();*/
            }
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
            context.Database.ExecuteSqlCommand("DELETE FROM QuestionBases");
        }
    }
}
