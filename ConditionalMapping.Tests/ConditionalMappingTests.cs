using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Diagnostics;

namespace ConditionalMapping.Tests
{
    class Document
    {
        public long ID { get; set; }

        public string Title { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>().Map(m => m.Requires("IsDeleted").HasValue(false));
        }
    }

    [TestClass]
    public class ConditionalMappingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (Context context = new Context())
            {
                context.Database.ExecuteSqlCommand("INSERT INTO Documents(Title, IsDeleted) VALUES ('Martine à la mer', 0);" +
                                                   "INSERT INTO Documents(Title, IsDeleted) VALUES ('Martine au cirque', 1);");

                Document doc = new Document
                {
                    Title = "Martine à la ferme"
                };

                context.Documents.Add(doc);

                context.SaveChanges();
            }

            using (Context context = new Context())
            {
                foreach (Document doc in context.Documents)
                {
                    Debug.WriteLine(doc.Title);
                }
            }
        }
    }
}
