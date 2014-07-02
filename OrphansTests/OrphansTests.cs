using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace OrphansTests
{
    class Parent
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<Child> Children { get; set; }

        public Parent()
        {
            Children = new List<Child>();
        }
    }

    class Child
    {
        public long ID { get; set; }

        public string Name { get; set; }

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

            parent.Children.Add(new Child { Name = "C1" });
            parent.Children.Add(new Child { Name = "C2" });
            parent.Children.Add(new Child { Name = "C3"/*, Parent = parent*/ });

            using (Context context = new Context())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Children");
                context.Database.ExecuteSqlCommand("DELETE FROM Parents");

                context.Parents.Add(parent);

                context.SaveChanges();

                parent.Children.RemoveAt(1);

                context.SaveChanges();

                Child children = parent.Children[1];
                parent.Children.RemoveAt(1);

                children.Parent = null;

                context.Children.Remove(children);

                context.SaveChanges();

                using (IDbConnection connection = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Children";

                        var all = new[] { new { Name = "", Parent_ID = (long?)null } }.ToList();
                        all.Clear();

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var o = new
                                {
                                    Name = reader["Name"] as string,
                                    Parent_ID = reader["Parent_ID"] != DBNull.Value ? (long)reader["Parent_ID"] : (long?)null
                                };

                                all.Add(o);
                            }
                        }

                        Assert.AreEqual(2, all.Count);
                        Assert.AreEqual("C2", all[1].Name);
                        Assert.IsNull(all[1].Parent_ID);
                    }
                }
            }
        }
    }
}
