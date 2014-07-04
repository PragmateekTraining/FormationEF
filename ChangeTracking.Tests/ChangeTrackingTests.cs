using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ChangeTracking.Tests
{
    class AssKicker
    {
        public long ID { get; set; }

        public string Name { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<AssKicker> People { get; set; }
    }

    [TestClass]
    public class ChangeTrackingTests
    {
        [TestMethod]
        public void CanTrackChanges()
        {
            using (Context context = new Context())
            {
                AssKicker Chuck = new AssKicker { Name = "Chuck Noris" };

                DbEntityEntry<AssKicker> entry = context.Entry<AssKicker>(Chuck);

                Assert.AreEqual(EntityState.Detached, entry.State);

                context.People.Add(Chuck);

                Assert.AreEqual(EntityState.Added, entry.State);

                context.SaveChanges();

                Assert.AreEqual(EntityState.Unchanged, entry.State);

                DbPropertyEntry<AssKicker, string> nameEntry = entry.Property(p => p.Name);

                Assert.IsFalse(nameEntry.IsModified);
                Assert.AreEqual(nameEntry.OriginalValue, nameEntry.CurrentValue);

                Chuck.Name = "Chuck Norris";

                context.ChangeTracker.DetectChanges();

                Assert.IsTrue(nameEntry.IsModified);
                Assert.AreNotEqual(nameEntry.OriginalValue, nameEntry.CurrentValue);
            }
        }
    }
}
