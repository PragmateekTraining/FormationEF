using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFQuestionnairePresentationsRepository
    {
        class QuestionsContext : DbContext
        {
            public DbSet<QuestionnairePresentation> Presentations { get; set; }
            public DbSet<QuestionnairePresentationPage> Pages { get; set; }
        }

        private QuestionsContext context = new QuestionsContext();

        public QuestionnairePresentation Add(QuestionnairePresentation presentation)
        {
            var o = context.Presentations.Add(presentation);
            context.SaveChanges();

            return GetPresentationById(presentation.ID);
        }

        public QuestionnairePresentation GetPresentationById(long id)
        {
            return context.Presentations.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<QuestionnairePresentation> GetAllPresentations()
        {
            return context.Presentations.ToList();
        }

        public void RemovePresentationPage(QuestionnairePresentation presentation, QuestionnairePresentationPage page)
        {
            presentation.Pages.Remove(page);

            context.Pages.Remove(page);

            context.SaveChanges();
        }

        public void SaveAll()
        {
            // context.ChangeTracker.DetectChanges();

            context.SaveChanges();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM QuestionnairePresentationPages");
            context.Database.ExecuteSqlCommand("DELETE FROM QuestionnairePresentations");
        }
    }
}
