using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RelationsUpdate.Tests
{
    class A
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<B> Bs { get; set; }

        public A()
        {
            Bs = new List<B>();
        }
    }

    class B
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<A> As { get; set; }

        public B()
        {
            As = new List<A>();
        }
    }

    class ABContext : DbContext
    {
        public DbSet<A> As { get; set; }
        public DbSet<B> Bs { get; set; }

        static ABContext()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<ABContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    [TestClass]
    public class ManyToManyUpdateTests
    {
        [TestMethod]
        public void CanUpdateAManyToManyRelationship()
        {
            A firstA = new A { Name = "First A" };

            B firstB = new B { Name = "First B" };
            B secondB = new B { Name = "Second B" };

            firstA.Bs.Add(firstB);
            firstA.Bs.Add(secondB);

            using (ABContext context = new ABContext())
            {
                context.As.Add(firstA);

                context.SaveChanges();
            }

            using (ABContext context = new ABContext())
            {
                A clone = context.As.AsNoTracking().Include(a => a.Bs).First();

                clone.Name = "Second A";

                // myA.ID = 0;

                foreach (B b in clone.Bs)
                {
                    // context.Entry(b).State = EntityState.Unchanged;

                    context.Bs.Attach(b);
                }

                context.As.Add(clone);

                Assert.AreEqual(EntityState.Unchanged, context.Entry(clone.Bs[0]).State);

                context.SaveChanges();
            }
        }
    }
}
