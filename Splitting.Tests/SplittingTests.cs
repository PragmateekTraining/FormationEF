using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace Splitting.Tests
{
    class ExtendedEntity
    {
        public Guid ID { get; set; }

        public string Legacy { get; set; }

        public string New { get; set; }
    }

    class EntityContext : DbContext
    {
        public DbSet<ExtendedEntity> Entities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExtendedEntity>()
                        .Map(m =>
                        {
                            m.Properties(e => new { e.ID, e.Legacy });
                            m.ToTable("LegacyTable");
                        })
                        .Map(m =>
                        {
                            m.Properties(e => new { e.ID, e.New });
                            m.ToTable("NewTable");
                        });
        }
    }

    [TestClass]
    public class SplittingTests
    {
        [TestMethod]
        public void CanSplitAnEntity()
        {
            Guid id = default(Guid);

            using (EntityContext context = new EntityContext())
            {
                ExtendedEntity entity = new ExtendedEntity
                {
                    ID = Guid.NewGuid(),
                    Legacy = "Old stuff",
                    New = "New stuff"
                };

                context.Entities.Add(entity);

                context.SaveChanges();

                id = entity.ID;
            }

            using (EntityContext context = new EntityContext())
            {
                ExtendedEntity entity = context.Entities.Find(id);

                Assert.AreEqual("Old stuff", entity.Legacy);
                Assert.AreEqual("New stuff", entity.New);
            }
        }
    }
}
