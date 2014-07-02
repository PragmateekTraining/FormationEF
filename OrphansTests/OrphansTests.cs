using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data;

namespace OrphansTests
{
    class Parent
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<Child> Children{get;set;}

        public Parent()
        {
            Children = new List<Child>();
        }
    }

    class Child
    {
        public long ID { get; set; }

        public string Name { get; set; }

        // [Required]
        public Parent Parent { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }
    }

    [TestClass]
    public class OrphansTests
    {
        [TestMethod]
        public void CanDeleteOrphans()
        {
            Parent parent = new Parent { Name = "P1" };

            parent.Children.Add(new Child { Name = "C1", /*Parent = parent*/ });
            parent.Children.Add(new Child { Name = "C2", Parent = parent });
            parent.Children.Add(new Child { Name = "C3", Parent = parent });

            using (Context context = new Context())
            {
                context.Parents.Add(parent);

                context.SaveChanges();

                parent.Children.RemoveAt(1);

                context.SaveChanges();

                Child children = parent.Children[1];
                parent.Children.RemoveAt(1);

                context.Children.Remove(children);

                context.SaveChanges();

                using (IDbCommand command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Children";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }
    }
}
