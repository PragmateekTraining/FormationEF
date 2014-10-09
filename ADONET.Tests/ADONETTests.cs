using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace ADONET.Tests
{
    [TestClass]
    public class ADONETTests
    {
        private const string DBName = "ADONET_Tests";

        private const string connectionString = @"Server=.\SQLExpress;Database=" + DBName + ";Integrated Security=True";

        [TestMethod]
        public void CanCreateANewDatabase()
        {
            string databaseCreationSQLScript = @"IF db_id('@DBName') IS NOT NULL
BEGIN
	ALTER DATABASE @DBName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE @DBName
END

CREATE DATABASE @DBName".Replace("@DBName", DBName);

            using (IDbConnection connection = new SqlConnection(@"Server=.\SQLExpress;Integrated Security=True"))
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = databaseCreationSQLScript;

                    command.ExecuteNonQuery();
                }

                using (IDbCommand getDatabaseIDCommand = connection.CreateCommand())
                {
                    getDatabaseIDCommand.CommandText = "SELECT db_id('@DBName')".Replace("@DBName", DBName);

                    int id = Convert.ToInt32(getDatabaseIDCommand.ExecuteScalar());

                    Assert.IsTrue(id > 0);
                }
            }
        }

        [TestMethod]
        public void CanCreateANewTable()
        {
        }

        [TestMethod]
        public void CanFeedANewTable()
        {
            Log[] inputLogs =
            {
                new Log { ID = 1, Level = Level.INFO, Message = "The sky is blue." },
                new Log { ID = 2, Level = Level.WARNING, Message = "Nuclear meltdown in progress!" },
                new Log { ID = 3, Level = Level.ERROR, Message = "Nuclear plant has exploded!!!"}
            };

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                foreach (Log log in inputLogs)
                {
                    using (IDbCommand insertCommand = connection.CreateCommand())
                    {
                        insertCommand.CommandText = "INSERT INTO Logs VALUES (@ID, @Level, @Message)";

                        IDbDataParameter idParameter = insertCommand.CreateParameter();
                        idParameter.ParameterName = "@ID";
                        idParameter.Value = log.ID;

                        IDbDataParameter levelParameter = insertCommand.CreateParameter();
                        levelParameter.ParameterName = "@Level";
                        levelParameter.Value = log.Level;

                        IDbDataParameter messageParameter = insertCommand.CreateParameter();
                        messageParameter.ParameterName = "@Message";
                        messageParameter.Value = log.Message;

                        insertCommand.Parameters.Add(idParameter);
                        insertCommand.Parameters.Add(levelParameter);
                        insertCommand.Parameters.Add(messageParameter);

                        int status = insertCommand.ExecuteNonQuery();

                        Assert.AreEqual(1, status);
                    }
                }
            }
        }
    }
}
