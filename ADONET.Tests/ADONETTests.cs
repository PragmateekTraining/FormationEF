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
            CanUpdateTheDataWithADirectUpdateSQLQuery();
            CanUpdateTheDataWithADataSet();
            CanUseATypedDataSet();
            CanUseATransaction();
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

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            IDbDataAdapter dataAdapter = providerFactory.CreateDataAdapter();

            IDbCommand getAllLogsCommand = providerFactory.CreateCommand();
            getAllLogsCommand.CommandText = getAllLogsQuery;
            dataAdapter.SelectCommand = getAllLogsCommand;

            DataSet dataSet = new DataSet();

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                getAllLogsCommand.Connection = connection;

                dataAdapter.Fill(dataSet);
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

        [TestMethod]
        public void CanUpdateTheDataWithADirectUpdateSQLQuery()
        {
            string countLogsIn2001Query = "SELECT COUNT(*) FROM Logs WHERE YEAR(Timestamp) = 2001";
            string updateLogsQuery = "UPDATE Logs SET Timestamp = DATEADD(year, 1, Timestamp)";

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (IDbCommand countLogsIn2001Command = connection.CreateCommand())
                {
                    countLogsIn2001Command.CommandText = countLogsIn2001Query;

                    object result = countLogsIn2001Command.ExecuteScalar();

                    int count = Convert.ToInt32(result);

                    Assert.AreEqual(3, count);
                }

                using (IDbCommand updateLogsCommand = connection.CreateCommand())
                {
                    updateLogsCommand.CommandText = updateLogsQuery;

                    int numberOfUpdatedRecords = updateLogsCommand.ExecuteNonQuery();

                    Assert.AreEqual(3, numberOfUpdatedRecords);
                }

                using (IDbCommand countLogsIn2001Command = connection.CreateCommand())
                {
                    countLogsIn2001Command.CommandText = countLogsIn2001Query;

                    object result = countLogsIn2001Command.ExecuteScalar();

                    int count = Convert.ToInt32(result);

                    Assert.AreEqual(0, count);
                }
            }
        }

        [TestMethod]
        public void CanUpdateTheDataWithADataSet()
        {
            string countLogsInFebruaryQuery = "SELECT COUNT(*) FROM Logs WHERE MONTH(Timestamp) = 2";

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            using (DbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                using (IDbCommand countLogsInFebruaryCommand = connection.CreateCommand())
                {
                    countLogsInFebruaryCommand.CommandText = countLogsInFebruaryQuery;

                    object result = countLogsInFebruaryCommand.ExecuteScalar();

                    int count = Convert.ToInt32(result);

                    Assert.AreEqual(3, count);
                }

                DbDataAdapter dataAdapter = providerFactory.CreateDataAdapter();

                DbCommand selectCommand = providerFactory.CreateCommand();
                selectCommand.CommandText = "SELECT * FROM Logs";
                selectCommand.Connection = connection;

                DbCommandBuilder commandBuilder = providerFactory.CreateCommandBuilder();
                commandBuilder.DataAdapter = dataAdapter;

                dataAdapter.SelectCommand = selectCommand;
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                #region Records update
                DataTable logsTable = dataSet.Tables[0];

                for (int i = 0; i < 3; ++i)
                {
                    DataRow record = logsTable.Rows[i];

                    DateTime timestamp = (DateTime)record["Timestamp"];

                    timestamp = timestamp.AddMonths(1);

                    record["Timestamp"] = timestamp;
                }

                int numberOfUpdatedRecords = dataAdapter.Update(dataSet);

                Assert.AreEqual(3, numberOfUpdatedRecords);
                #endregion

                using (IDbCommand countLogsInFebruaryCommand = connection.CreateCommand())
                {
                    countLogsInFebruaryCommand.CommandText = countLogsInFebruaryQuery;

                    object result = countLogsInFebruaryCommand.ExecuteScalar();

                    int count = Convert.ToInt32(result);

                    Assert.AreEqual(0, count);
                }
            }
        }

        [TestMethod]
        public void CanUseATypedDataSet()
        {
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            using (DbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                connection.Open();

                DbDataAdapter dataAdapter = providerFactory.CreateDataAdapter();

                dataAdapter.SelectCommand = providerFactory.CreateCommand();
                dataAdapter.SelectCommand.CommandText = "SELECT * FROM Logs";
                dataAdapter.SelectCommand.Connection = connection;

                DbCommandBuilder commandBuilder = providerFactory.CreateCommandBuilder();
                commandBuilder.DataAdapter = dataAdapter;

                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();

                LogsDataSet dataSet = new LogsDataSet();

                dataAdapter.Fill(dataSet, "Logs");

                for (int i = 0; i < 3; ++i)
                {
                    LogsDataSet.LogsRow record = dataSet.Logs[i];

                    record.Timestamp = record.Timestamp.AddDays(1);
                }

                int numberOfUpdatedRecords = dataAdapter.Update(dataSet, "Logs");

                Assert.AreEqual(3, numberOfUpdatedRecords);
            }
        }

        private int CountLogs(IDbConnection connection)
        {
            string countLogsQuery = "SELECT COUNT(*) FROM Logs";

            using (IDbCommand countLogsCommand = connection.CreateCommand())
            {
                countLogsCommand.CommandText = countLogsQuery;

                object result = countLogsCommand.ExecuteScalar();

                int count = Convert.ToInt32(result);

                return count;
            }
        }

        [TestMethod]
        public void CanUseATransaction()
        {
            string deleteLogQuery = "DELETE FROM Logs WHERE ID=@ID";

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int count = CountLogs(connection);

                Assert.AreEqual(3, count);

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (IDbCommand deleteLogCommand = connection.CreateCommand())
                    {
                        deleteLogCommand.CommandText = deleteLogQuery;
                        deleteLogCommand.Transaction = transaction;

                        IDbDataParameter idParameter = deleteLogCommand.CreateParameter();
                        idParameter.ParameterName = "@ID";
                        idParameter.Value = 1;

                        deleteLogCommand.Parameters.Add(idParameter);

                        int numberOfDeletedRecords = deleteLogCommand.ExecuteNonQuery();

                        Assert.AreEqual(1, numberOfDeletedRecords);
                    }
                }

                count = CountLogs(connection);

                Assert.AreEqual(3, count);

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (IDbCommand deleteLogCommand = connection.CreateCommand())
                    {
                        deleteLogCommand.CommandText = deleteLogQuery;
                        deleteLogCommand.Transaction = transaction;

                        IDbDataParameter idParameter = deleteLogCommand.CreateParameter();
                        idParameter.ParameterName = "@ID";
                        idParameter.Value = 1;

                        deleteLogCommand.Parameters.Add(idParameter);

                        int numberOfDeletedRecords = deleteLogCommand.ExecuteNonQuery();

                        Assert.AreEqual(1, numberOfDeletedRecords);
                    }

                    transaction.Commit();
                }

                count = CountLogs(connection);

                Assert.AreEqual(2, count);
            }
        }
    }
}
