using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
        [TestMethod]
        public void CanMaterializeRandomEntitiesWithSqlQuery()
        {
            string connectionString = @"Server=.\SQLExpress;Database=SqlQuery;Trusted_Connection=True;";

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE Players(id INT PRIMARY KEY IDENTITY, name TEXT);" +
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
    }
}
