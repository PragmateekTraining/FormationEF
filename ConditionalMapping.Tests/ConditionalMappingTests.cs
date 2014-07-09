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
        public void WontLoadDeletedDocuments()
        {
            using (Context context = new Context())
            {
                int count = context.Database.ExecuteSqlCommand("INSERT INTO Documents(Title, IsDeleted) VALUES (@p0, @p1);" +
                                                   "INSERT INTO Documents(Title, IsDeleted) VALUES (@p2, @p3);",
                                                   "Martine à la mer", 0, "Martine au cirque", 1);

                Assert.AreEqual(2, count);

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
