using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepositoryPatternSample.Tests
{
    [TestClass]
    public class QuestionsRepositoryTests
    {
        [TestMethod]
        public void CanAddAndGetQuestionsBack()
        {
            QuestionsRepository repo = new QuestionsRepository();

            Question inputQuestion = new Question
            {
                CreationDate = DateTime.Now,
                Text = "1 + 1?",
                Answer = "2",
                IsOptional = true
            };

            repo.Add(inputQuestion);

            Assert.IsTrue(inputQuestion.ID > 0);

            Question outputQuestion = repo.GetQuestionById(inputQuestion.ID);

            Assert.AreEqual(inputQuestion.ID, outputQuestion.ID);
            Assert.IsTrue((inputQuestion.CreationDate - outputQuestion.CreationDate).TotalSeconds < 1);
            Assert.AreEqual(inputQuestion.Text, outputQuestion.Text);
            Assert.AreEqual(inputQuestion.Answer, outputQuestion.Answer);
            Assert.AreEqual(inputQuestion.IsOptional, outputQuestion.IsOptional);
        }

        [TestMethod]
        public void CantGetAnUnexistingQuestion()
        {
            QuestionsRepository repo = new QuestionsRepository();

            Question question = repo.GetQuestionById(-1);

            Assert.IsNull(question);
        }

        [TestMethod]
        public void CanClearQuestionsRepository()
        {
            QuestionsRepository repo = new QuestionsRepository();

            repo.Add(new Question
                {
                    CreationDate = DateTime.UtcNow,
                    Text = "Knock knock!",
                    Answer = "Who's there?"
                });

            IEnumerable<Question> all = repo.GetAllQuestions();

            Assert.IsTrue(all.Count() >= 1);

            repo.Clear();

            all = repo.GetAllQuestions();

            Assert.AreEqual(0, all.Count());
        }
    }
}
