using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

namespace Repositories
{
    public class EFTaggedQuestionsTPCRepository : EFTaggedQuestionsRepository
    {
        protected class TPCQuestionsContext : QuestionsContext
        {
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<QuestionBase>().Map<QuestionBase>(m => m.Re.ToTable("QuestionBases");
                modelBuilder.Entity<CategorizedQuestion>().ToTable("CategorizedQuestions");
                modelBuilder.Entity<TaggedQuestion>().ToTable("TaggedQuestions");
            }
        }

        public EFTaggedQuestionsTPCRepository()
            : base(new TPCQuestionsContext())
        {
        }
    }
}
