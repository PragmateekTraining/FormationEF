using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;
using Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace StubEntities.Tests
{
    [TestClass]
    public class StubEntitiesTests
    {
        class Context : DbContext
        {
            public DbSet<BasicQuestion> Questions { get; set; }
        }

        private Context NewContext()
        {
            Context context = new Context();

            context.Database.Log += sql => Debug.WriteLine(sql);

            return context;
        }

        private void SetUpDB()
        {
            using (Context context = NewContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE BasicQuestions");

                context.Questions.Add(new BasicQuestion
                {
                    CreationDate = DateTime.UtcNow,
                    Text = "1+1?",
                    Answer = "2"
                });

                context.Questions.Add(new BasicQuestion
                {
                    CreationDate = DateTime.UtcNow,
                    Text = "2+2?",
                    Answer = "5"
                });

                context.Questions.Add(new BasicQuestion
                {
                    CreationDate = DateTime.UtcNow,
                    Text = "3+3?",
                    Answer = "6"
                });

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanDeleteUsingAStub()
        {
            SetUpDB();

            using (Context context = NewContext())
            {
                BasicQuestion stub = new BasicQuestion { ID = 2 };

                context.Questions.Attach(stub);

                context.Questions.Remove(stub);

                context.SaveChanges();
            }

            using (Context context = new Context())
            {
                IList<BasicQuestion> questions = context.Questions.ToList();

                Assert.AreEqual(2, questions.Count);
            }
        }

        [TestMethod]
        public void CanUpdateUsingAStub()
        {
            SetUpDB();

            using (Context context = NewContext())
            {
                BasicQuestion stub = new BasicQuestion { ID = 2 };

                context.Questions.Attach(stub);

                stub.Answer = "4";

                context.SaveChanges();
            }

            using (Context context = new Context())
            {
                IList<BasicQuestion> questions = context.Questions.ToList();

                // First check that we have not changed other properties
                Assert.AreEqual("2+2?", questions[1].Text);

                Assert.AreEqual("4", questions[1].Answer);
            }
        }

        [TestMethod]
        public void CanReplaceAnEntireEntityUsingAStub()
        {
            SetUpDB();

            using (Context context = NewContext())
            {
                BasicQuestion stub = new BasicQuestion { ID = 2, CreationDate = DateTime.UtcNow, Answer = "4" };

                context.Entry(stub).State = EntityState.Modified;

                context.SaveChanges();
            }

            using (Context context = new Context())
            {
                IList<BasicQuestion> questions = context.Questions.ToList();

                Assert.AreEqual("4", questions[1].Answer);

                // Check that we have overridden other properties
                Assert.AreEqual(null, questions[1].Text);
            }
        }
    }
}
