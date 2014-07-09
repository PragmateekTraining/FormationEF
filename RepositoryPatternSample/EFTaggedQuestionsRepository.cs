using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFTaggedQuestionsRepository
    {
        protected class QuestionsContext : DbContext
        {
            static QuestionsContext()
            {
                // /!\ Only for testing, not production code /!\
                Database.SetInitializer(new DropCreateDatabaseAlways<QuestionsContext>());
            }

            public DbSet<QuestionBase> Questions { get; set; }
        }

        private QuestionsContext context;

        public EFTaggedQuestionsRepository()
            : this(new QuestionsContext())
        {
        }

        protected EFTaggedQuestionsRepository(QuestionsContext context)
        {
            this.context = context;
        }

        public void Add(QuestionBase question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public QuestionBase GetQuestionById(long id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<QuestionBase> GetAllQuestions()
        {
            return context.Questions.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("IF OBJECT_ID('TaggedQuestions') IS NOT NULL DELETE FROM TaggedQuestions");
            context.Database.ExecuteSqlCommand("IF OBJECT_ID('CategorizedQuestions') IS NOT NULL DELETE FROM CategorizedQuestions");
            context.Database.ExecuteSqlCommand("IF OBJECT_ID('QuestionBases') IS NOT NULL DELETE FROM QuestionBases");
        }
    }
}
