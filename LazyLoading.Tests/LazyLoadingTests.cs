using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace LazyLoading.Tests
{
    public class Team
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<Person> Members { get; set; }
    }

    public class LazyTeam
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public virtual IList<Person> Members { get; set; }
    }

    public class Person
    {
        public long ID { get; set; }

        public string Name { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<LazyTeam> LazyTeams { get; set; }
    }

    [TestClass]
    public class LazyLoadingTests
    {
        [TestMethod]
        public void CanLoadEntitiesEagerly()
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

        [TestMethod]
        public void CanLoadEntitiesLazily()
        {
            LazyTeam expandables = new LazyTeam
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
                Assert.AreSame(context.Set<Team>(), context.Teams);

                context.Database.ExecuteSqlCommand("DELETE FROM People");
                context.Database.ExecuteSqlCommand("DELETE FROM LazyTeams");

                context.LazyTeams.Add(expandables);

                context.SaveChanges();
            }

            using (Context context = new Context())
            {
                bool hasLoadedPeople = false;

                (context as IObjectContextAdapter).ObjectContext.ObjectMaterialized += (s, a) =>
                {
                    Debug.WriteLine(string.Format("Materializing '{0}'.", a.Entity));

                    if (a.Entity is Person)
                        hasLoadedPeople = true;
                };

                context.Database.Log += sql => Debug.WriteLine(sql);

                LazyTeam team = context.LazyTeams.Single();

                Assert.AreEqual(1, context.ChangeTracker.Entries().Count());

                Assert.IsFalse(hasLoadedPeople);

                Debug.WriteLine("========== Before ==========");

                Assert.IsNotNull(team.Members);

                Debug.WriteLine("========== After ==========");

                Assert.IsTrue(hasLoadedPeople);

                Assert.AreEqual(4, context.ChangeTracker.Entries().Count());

                Assert.AreEqual(3, team.Members.Count);
            }
        }
    }
}
