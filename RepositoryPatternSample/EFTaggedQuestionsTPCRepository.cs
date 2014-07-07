using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories
{
    public class EFTaggedQuestionsTPCRepository : EFTaggedQuestionsRepository
    {
        protected class TPCQuestionsContext : QuestionsContext
        {
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<QuestionBase>().ToTable("QuestionBases");
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
