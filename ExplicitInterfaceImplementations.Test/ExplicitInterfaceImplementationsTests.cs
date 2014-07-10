using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;

namespace ExplicitInterfaceImplementations.Test
{
    interface IPerson
    {
        string Name { get; set; }
        DateTime DateOfBirth { get; set; }
        bool KicksAss { get; set; }
        string Address { get; set; }
    }

    class Person : IPerson
    {
        public long Id { get; set; }

        public string Name { get; set; }

        DateTime IPerson.DateOfBirth { get; set; }

        internal bool KicksAss { get; set; }
        bool IPerson.KicksAss
        {
            get { return KicksAss; }
            set { KicksAss = value; }
        }

        internal string Address { get; set; }

        string IPerson.Address
        {
            get { return Address; }
            set { Address = value; }
        }
    }

    class CRMContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().Property(p => p.Address).HasColumnName("Address");
        }
    }

    [TestClass]
    public class ExplicitInterfaceImplementationsTests
    {
        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<CRMContext>());
        }

        [TestMethod]
        public void CanPersistEntities()
        {
            using (CRMContext context = new CRMContext())
            {
                Person Chuck = new Person
                {
                    Name = "Chuck Norris",
                    Address = "Everywhere",
                    KicksAss = true
                };

                (Chuck as IPerson).DateOfBirth = new DateTime(1940, 03, 10);

                context.People.Add(Chuck);

                context.SaveChanges();
            }

            using (CRMContext context = new CRMContext())
            {
                Person fake = context.People.Single();

                Assert.AreEqual("Chuck Norris", fake.Name); // OK : mapped and persisted
                Assert.AreEqual(default(DateTime), (fake as IPerson).DateOfBirth); // KO : not mapped
                Assert.IsFalse(fake.KicksAss); // KO : not mapped
                Assert.AreEqual("Everywhere", fake.Address); // OK : mapped and persisted
            }
        }
    }
}
