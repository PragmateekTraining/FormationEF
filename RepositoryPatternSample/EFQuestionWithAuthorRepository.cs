using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFQuestionWithAuthorRepository
    {
        class QuestionWithAuthorContext : DbContext
        {
            public DbSet<QuestionWithAuthor> Questions { get; set; }
            public DbSet<Author> Authors { get; set; }
        }

        private QuestionWithAuthorContext context = new QuestionWithAuthorContext();

        public void Add(QuestionWithAuthor question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public QuestionWithAuthor GetQuestionById(long id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<QuestionWithAuthor> GetAllQuestions()
        {
            return context.Questions.ToList();
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            return context.Authors.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM QuestionWithAuthors");
            context.Database.ExecuteSqlCommand("DELETE FROM Authors");
        }
    }
}
