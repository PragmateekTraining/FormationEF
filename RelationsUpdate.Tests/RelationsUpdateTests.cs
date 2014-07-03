using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RelationsUpdate.Tests
{
    class Person
    {
        public long ID { get; set; }
        public string Name { get; set; }

        public Person Father { get; set; }
        //public long Father_ID { get; set; }
    }

    class PersonFK
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Name { get; set; }

        //[ForeignKey("FatherID")]
        public PersonFK Father { get; set; }

        public long? FatherID { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Person> People { get; set; }
    }

    class ContextFK : DbContext
    {
        public DbSet<PersonFK> People { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    /*modelBuilder.Entity<PersonFK>()
        //                .HasOptional(x => x.Father)*/
        //                /*.WithOptionalDependent(x => x.ChildMessage)*/;
        //}
    }

    [TestClass]
    public class RelationsUpdateTests
    {
        [TestMethod]
        public void CanUpdateARelation()
        {
            long AnakinID = 0;
            long LukeID = 0;
            long ChuckID = 0;

            using (Context context = new Context())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE People");

                Person Anakin = new Person { Name = "Dark Vador" };
                Person Luke = new Person { Name = "Luke", Father = Anakin };

                context.People.Add(Luke);

                context.SaveChanges();

                AnakinID = Anakin.ID;
                LukeID = Luke.ID;
            }

            using (Context context = new Context())
            {
                Person Luke = context.People.Single(p => p.Name == "Luke");

                Person Chuck = new Person { Name = "Chuck Norris" };

                Luke.Father = Chuck;

                context.SaveChanges();

                ChuckID = Chuck.ID;
            }

            // With stubs
            using (Context context = new Context())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                Person Chuck = new Person { ID = ChuckID };
                Person Anakin = new Person { ID = AnakinID };
                Person Luke = new Person { ID = LukeID, Father = Chuck };

                // context.People.Attach(Chuck);
                context.People.Attach(Anakin);
                context.People.Attach(Luke);

                Luke.Father = Anakin;

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanUpdateARelationFK()
        {
            long AnakinID = 0;
            long LukeID = 0;
            long ChuckID = 0;

            using (ContextFK context = new ContextFK())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE PersonFKs");

                PersonFK Anakin = new PersonFK { Name = "Dark Vador" };
                PersonFK Luke = new PersonFK { Name = "Luke", Father = Anakin };

                context.People.Add(Luke);

                context.SaveChanges();

                Assert.AreEqual(Anakin.ID, Luke.FatherID);

                AnakinID = Anakin.ID;
                LukeID = Luke.ID;
            }

            using (ContextFK context = new ContextFK())
            {
                PersonFK Luke = context.People.Single(p => p.Name == "Luke");

                PersonFK Chuck = new PersonFK { Name = "Chuck Norris" };

                Luke.Father = Chuck;

                context.SaveChanges();

                ChuckID = Chuck.ID;
            }

            // With stubs
            using (ContextFK context = new ContextFK())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                PersonFK Chuck = new PersonFK { ID = ChuckID };
                PersonFK Anakin = new PersonFK { ID = AnakinID };
                PersonFK Luke = new PersonFK { ID = LukeID };

                // context.People.Attach(Chuck);
                context.People.Attach(Anakin);
                context.People.Attach(Luke);

                Luke.Father = Anakin;

                context.SaveChanges();
            }
        }
    }
}
