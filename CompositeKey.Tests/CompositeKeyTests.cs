using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace CompositeKey.Tests
{
    class Document
    {
        // [Key, Column(Order = 0)]
        public string Title { get; set; }
        //[Key, Column(Order = 1)]
        public int Version { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return string.Format("'{0}' (v{1}) :\n{2}", Title, Version, Text);
        }
    }

    class CMSContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>().HasKey(doc => new { doc.Title, doc.Version });
        }
    }

    [TestClass]
    public class CompositeKeyTests
    {
        [TestMethod]
        public void CanDefineACompositeKey()
        {
            using (CMSContext context = new CMSContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Documents");

                Document devisV1 = new Document
                {
                    Title = "Devis Bygmalion",
                    Version = 1,
                    Text = "Hebergement Site : 3588€ TTC\n" +
                           "Mini Sites : 232598 € TTC"
                };

                context.Documents.Add(devisV1);

                Document devisV2 = new Document
                {
                    Title = "Devis Bygmalion",
                    Version = 2,
                    Text = "Hebergement Site : 3588€ TTC\n" + 
                           "Mini Sites : 232598 € TTC\n" +
                           "e-reputation : 197340 € TTC"
                };

                context.Documents.Add(devisV2);

                context.SaveChanges();
            }

            using (CMSContext context = new CMSContext())
            {
                foreach (Document document in context.Documents)
                {
                    Debug.WriteLine(document);
                }
            }

            Debug.WriteLine(new string('=', 20));

            using (CMSContext context = new CMSContext())
            {
                Document devisV2 = context.Documents.Find("Devis Bygmalion", 2);

                Debug.WriteLine(devisV2);
            }
        }
    }
}
