using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;
using System.Data.SQLite;

namespace Repositories
{
    public class EFSQLiteQuestionBasesRepository : IQuestionBasesRepository
    {
        class QuestionsContext : DbContext
        {
            public QuestionsContext()
                : base("name=dbConnection")
            {
                //System.Data.Entity.Database.SetInitializer<QuestionsContext>(null);
            }

            public DbSet<BasicQuestion> Questions { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                /*modelBuilder.Conventions
                    .Remove<PluralizingTableNameConvention>();*/
            }
        }

        private QuestionsContext context = new QuestionsContext();

        public EFSQLiteQuestionBasesRepository()
        {
            using (IDbConnection connection = OpenConnection())
            {
                using (IDbCommand createTableCommand = connection.CreateCommand())
                {
                    createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS BasicQuestions(id INTEGER PRIMARY KEY, creation_date TEXT, text TEXT, answer TEXT, is_optional INTEGER)";

                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        const string connectionString = @"Data Source=.\data.sqlite";

        private IDbConnection OpenConnection()
        {
            IDbConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            return connection;
        }

        public void Add(BasicQuestion question)
        {
            context.Questions.Add(question);
            context.SaveChanges();
        }

        public BasicQuestion GetQuestionById(long id)
        {
            return context.Questions.SingleOrDefault(q => q.ID == id);
        }

        public IEnumerable<BasicQuestion> GetAllQuestions()
        {
            return context.Questions.ToList();
        }

        public void Clear()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM BasicQuestions");
        }
    }
}
