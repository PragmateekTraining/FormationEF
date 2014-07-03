using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace LazyLoading.Tests
{
    class Team
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<Person> Members { get; set; }
    }

    class Person
    {
        public long ID { get; set; }

        public string Name { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Team> Teams { get; set; }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Team expandables = new Team
            {
                Name = "Expandables",
                Members = new[]
                {
                    new Person { Name = "Chuck Norris" },
                    new Person { Name = "Sylvester Stallone" },
                    new Person { Name = "Jet Li" }
                }
            };

            using (Context context = new Context())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM People");
                context.Database.ExecuteSqlCommand("DELETE FROM Teams");

                context.Teams.Add(expandables);

                context.SaveChanges();
            }

            using (Context context = new Context())
            {
                Team team = context.Teams.Single();

                Assert.IsNull(team.Members);
            }

            using (Context context = new Context())
            {
                Team team = context.Teams.Include(t => t.Members).Single();

                Assert.IsNotNull(team.Members);

                Assert.AreEqual(3, team.Members.Count);
            }
        }
    }
}
