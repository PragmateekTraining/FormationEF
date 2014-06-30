using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFQuestionWithMetadataRepository : IQuestionWithMetadataRepository
    {
        class QuestionWithMetadataContext : DbContext
        {
            public DbSet<QuestionWithMetadata> Questions { get; set; }
        }

        private QuestionWithMetadataContext context = new QuestionWithMetadataContext();

        public void Add(QuestionWithMetadata question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public QuestionWithMetadata GetQuestionById(long id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<QuestionWithMetadata> GetAllQuestions()
        {
            return context.Questions.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM QuestionWithMetadatas");
        }
    }
}
