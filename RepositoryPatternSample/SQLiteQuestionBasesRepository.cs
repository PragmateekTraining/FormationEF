using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using Model;

namespace Repositories
{
    public class SQLiteQuestionBasesRepository : IQuestionBasesRepository
    {
        const string connectionString = "Data Source=questions.db3";

        public SQLiteQuestionBasesRepository()
        {
            using (IDbConnection connection = OpenConnection())
            {
                using (IDbCommand createTableCommand = connection.CreateCommand())
                {
                    createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS questions(id INTEGER PRIMARY KEY, creation_date TEXT, text TEXT, answer TEXT, is_optional INTEGER)";

                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            return connection;
        }

        public void Add(QuestionBase question)
        {
            using (IDbConnection connection = OpenConnection())
            {
                using (IDbCommand insertCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = "INSERT INTO questions(creation_date, text, answer, is_optional) VALUES (@CreationDate, @Text, @Answer, @IsOptional); SELECT last_insert_rowid() FROM questions";
                    insertCommand.Parameters.Add(new SQLiteParameter("@CreationDate", question.CreationDate.ToString("s")));
                    insertCommand.Parameters.Add(new SQLiteParameter("@Text", question.Text));
                    insertCommand.Parameters.Add(new SQLiteParameter("@Answer", question.Answer));
                    insertCommand.Parameters.Add(new SQLiteParameter("@IsOptional", question.IsOptional));

                    question.ID = (long)insertCommand.ExecuteScalar();
                }
            }
        }

        private IList<QuestionBase> ExecuteAndRead(IDbCommand command)
        {
            IList<QuestionBase> questions = new List<QuestionBase>();

            IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                QuestionBase question = new QuestionBase
                {
                    ID = (long)reader["id"],
                    CreationDate = DateTime.ParseExact((string)reader["creation_date"], "s", null),
                    Text = (string)reader["text"],
                    Answer = (string)reader["answer"],
                    IsOptional = (long)reader["is_optional"] == 1
                };

                questions.Add(question);
            }

            return questions;
        }

        public QuestionBase GetQuestionById(long id)
        {
            QuestionBase question = null;

            using (IDbConnection connection = OpenConnection())
            {
                using (IDbCommand selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT * FROM questions WHERE id=@ID";
                    selectCommand.Parameters.Add(new SQLiteParameter("@ID", id));

                    question = ExecuteAndRead(selectCommand).FirstOrDefault();
                }
            }

            return question;
        }

        public IEnumerable<QuestionBase> GetAllQuestions()
        {
            using (IDbConnection connection = OpenConnection())
            {
                using (IDbCommand selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT * FROM questions";

                    return ExecuteAndRead(selectCommand);
                }
            }
        }

        public void Clear()
        {
            using (IDbConnection connection = OpenConnection())
            {
                using (IDbCommand deleteCommand = connection.CreateCommand())
                {
                    deleteCommand.CommandText = "DELETE FROM questions";
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
