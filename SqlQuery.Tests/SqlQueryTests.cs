using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;

using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQuery.Tests
{
    class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] '{1}'", ID, Name);
        }
    }

    [TestClass]
    public class SqlQueryTests
    {
        const string connectionString = @"Server=.\SQLExpress;Database=SqlQuery;Trusted_Connection=True;";

        [TestMethod]
        public void CanMaterializeRandomEntitiesWithSqlQuery()
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "IF OBJECT_ID('Players') IS NULL CREATE TABLE Players(id INT PRIMARY KEY IDENTITY, name TEXT);" +
                                          "TRUNCATE TABLE Players;" +
                                          "INSERT INTO Players(name) VALUES ('John');" +
                                          "INSERT INTO Players(name) VALUES ('Joe');" +
                                          "INSERT INTO Players(name) VALUES ('Bob');";

                    command.ExecuteNonQuery();
                }
            }

            using (DbContext context = new DbContext(connectionString))
            {
                foreach (Player player in context.Database.SqlQuery<Player>("SELECT * FROM Players"))
                {
                    Debug.WriteLine(player);
                }
            }
        }

        [TestMethod]
        public void CanInvokeStoredProcedures()
        {
            using (DbContext context = new DbContext(connectionString))
            {
                int sum = context.Database.SqlQuery<int>("EXEC EF_Sum 123, 456").Single();

                Assert.AreEqual(579, sum);
            }

            using (DbContext context = new DbContext(connectionString))
            {
                IList<Player> all = context.Database.SqlQuery<Player>("EXEC EF_All").ToList();

                Assert.AreEqual(3, all.Count);
            }
        }
    }
}
