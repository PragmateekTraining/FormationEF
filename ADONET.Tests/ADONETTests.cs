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

        private static readonly Log[] referenceLogs =
            {
                new Log { Timestamp = new DateTime(2001, 02, 03, 04, 05, 06),   Level = Level.INFO,       Message = "The sky is blue." },
                new Log { Timestamp = new DateTime(2001, 02, 03, 04, 05, 07),   Level = Level.WARNING,    Message = "Nuclear meltdown in progress!" },
                new Log { Timestamp = new DateTime(2001, 02, 03, 04, 05, 08),   Level = Level.ERROR,      Message = "Nuclear plant has exploded!!!"}
            };

        [TestMethod]
        public void CanExecuteTheWholeProcess()
        {
            CanCreateANewDatabase();
            CanCreateANewTable();
            CanFeedANewTable();
            CanGetTheDataFromTheDatabaseInConnectedMode();
        }

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
            string getTableIdQuery = "SELECT object_id('Logs', 'U')";
            string createTableQuery = "CREATE TABLE Logs(ID bigint IDENTITY PRIMARY KEY, Timestamp datetime2, Level varchar(10), Message varchar(max))";

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand getTableIdCommand = connection.CreateCommand())
                {
                    getTableIdCommand.CommandText = getTableIdQuery;

                    object result = getTableIdCommand.ExecuteScalar();

                    Assert.IsTrue(result is DBNull);
                }

                using (IDbCommand createTableCommand = connection.CreateCommand())
                {
                    createTableCommand.CommandText = createTableQuery;

                    createTableCommand.ExecuteNonQuery();
                }

                using (IDbCommand getTableIdCommand = connection.CreateCommand())
                {
                    getTableIdCommand.CommandText = getTableIdQuery;

                    object result = getTableIdCommand.ExecuteScalar();

                    int id = Convert.ToInt32(result);

                    Assert.IsTrue(id > 0);
                }
            }
        }

        [TestMethod]
        public void CanFeedANewTable()
        {
            string countLogsQuery = "SELECT COUNT(*) FROM Logs";
            string insertLogQuery = "INSERT INTO Logs VALUES (@Timestamp, @Level, @Message)";

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand countLogsCommand = connection.CreateCommand())
                {
                    countLogsCommand.CommandText = countLogsQuery;

                    object result = countLogsCommand.ExecuteScalar();

                    int count = Convert.ToInt32(result);

                    Assert.AreEqual(0, count);
                }

                foreach (Log log in referenceLogs)
                {
                    using (IDbCommand insertCommand = connection.CreateCommand())
                    {
                        insertCommand.CommandText = insertLogQuery;

                        //IDbDataParameter idParameter = insertCommand.CreateParameter();
                        //idParameter.ParameterName = "@ID";
                        //idParameter.Value = log.ID;

                        IDbDataParameter timestampParameter = insertCommand.CreateParameter();
                        timestampParameter.ParameterName = "@Timestamp";
                        timestampParameter.Value = log.Timestamp;

                        IDbDataParameter levelParameter = insertCommand.CreateParameter();
                        levelParameter.ParameterName = "@Level";
                        levelParameter.Value = log.Level;

                        IDbDataParameter messageParameter = insertCommand.CreateParameter();
                        messageParameter.ParameterName = "@Message";
                        messageParameter.Value = log.Message;

                        // insertCommand.Parameters.Add(idParameter);
                        insertCommand.Parameters.Add(timestampParameter);
                        insertCommand.Parameters.Add(levelParameter);
                        insertCommand.Parameters.Add(messageParameter);

                        int status = insertCommand.ExecuteNonQuery();

                        Assert.AreEqual(1, status);
                    }
                }

                using (IDbCommand countLogsCommand = connection.CreateCommand())
                {
                    countLogsCommand.CommandText = countLogsQuery;

                    object result = countLogsCommand.ExecuteScalar();

                    int count = Convert.ToInt32(result);

                    Assert.AreEqual(3, count);
                }
            }
        }

        [TestMethod]
        public void CanGetTheDataFromTheDatabaseInConnectedMode()
        {
            string getAllLogsQuery = "SELECT * FROM Logs";

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand getAllLogsCommand = connection.CreateCommand())
                {
                    getAllLogsCommand.CommandText = getAllLogsQuery;

                    using (IDataReader dataReader = getAllLogsCommand.ExecuteReader())
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            bool hasMoreRecords = dataReader.Read();

                            Assert.IsTrue(hasMoreRecords);

                            DateTime timestamp = (DateTime)dataReader["Timestamp"];

                            string levelAsString = (string)dataReader["Level"];
                            Level level = (Level)Enum.Parse(typeof(Level), levelAsString);

                            string message = (string)dataReader["Message"];

                            Assert.AreEqual(timestamp, referenceLogs[i].Timestamp);
                            Assert.AreEqual(level, referenceLogs[i].Level);
                            Assert.AreEqual(message, referenceLogs[i].Message);
                        }

                        bool hasMoreRecord = dataReader.Read();

                        Assert.IsFalse(hasMoreRecord);
                    }
                }
            }
        }

        [TestMethod]
        public void CanGetTheDataFromTheDatabaseInDisconnectedMode()
        {
            string getAllLogsQuery = "SELECT * FROM Logs";

            DataSet dataSet = new DataSet();

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            IDbDataAdapter dataAdapter = providerFactory.CreateDataAdapter();

            using (DbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand getAllLogsCommand = connection.CreateCommand())
                {
                    getAllLogsCommand.CommandText = getAllLogsQuery;

                    dataAdapter.SelectCommand = getAllLogsCommand;

                    dataAdapter.Fill(dataSet);
                }
            }

            Assert.AreEqual(1, dataSet.Tables.Count);

            DataTable dataTable = dataSet.Tables[0];

            Assert.AreEqual(4, dataTable.Columns.Count);

            Assert.AreEqual(3, dataTable.Rows.Count);

            for (int i = 0; i < 3; ++i)
            {
                DateTime timestamp = (DateTime)dataTable.Rows[i]["Timestamp"];

                string levelAsString = (string)dataTable.Rows[i]["Level"];
                Level level = (Level)Enum.Parse(typeof(Level), levelAsString);

                string message = (string)dataTable.Rows[i]["Message"];

                Assert.AreEqual(timestamp, referenceLogs[i].Timestamp);
                Assert.AreEqual(level, referenceLogs[i].Level);
                Assert.AreEqual(message, referenceLogs[i].Message);
            }
        }
    }
}
