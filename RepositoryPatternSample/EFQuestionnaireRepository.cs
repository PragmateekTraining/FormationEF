using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFQuestionnairesRepository
    {
        class QuestionsContext : DbContext
        {
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Questionnaire>().HasMany<QuestionBase>(questionnaire => questionnaire.Questions)
                                                    .WithMany();
            }

            public DbSet<Questionnaire> Questionnaires { get; set; }
        }

        private QuestionsContext context = new QuestionsContext();

        public void Add(Questionnaire questionnaire)
        {
            context.Questionnaires.Add(questionnaire);
            context.SaveChanges();
        }

        public Questionnaire GetQuestionnaireById(long id)
        {
            return context.Questionnaires.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<Questionnaire> GetAllQuestionnaires()
        {
            return context.Questionnaires.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM Questionnaires");
        }
    }
}
