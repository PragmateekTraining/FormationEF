using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    [TestClass]
    public class QuestionWithMetadataRepositoryTests
    {
        IQuestionWithMetadataRepository repository = new EFQuestionWithMetadataRepository();

        [TestMethod]
        public void CanAddAndGetQuestionsBack()
        {
            QuestionWithMetadata inputQuestion = new QuestionWithMetadata
            {
                Metadata = new QuestionMetadata
                {
                    CreationDate = DateTime.UtcNow,
                    Author = "Chuck Norris",
                    Comment = "If you don't answer I'll kick your ass!"
                },
                Nomination = new Nomination
                {
                    Title = "Question of the year 2014"
                },
                Text = "0 / 0?",
                Answer = "NaN"
            };

            repository.Add(inputQuestion);

            Assert.IsTrue(inputQuestion.ID > 0);

            QuestionWithMetadata outputQuestion = repository.GetQuestionById(inputQuestion.ID);

            Assert.AreEqual(inputQuestion.ID, outputQuestion.ID);
            Assert.AreEqual(inputQuestion.Text, outputQuestion.Text);
            Assert.AreEqual(inputQuestion.Answer, outputQuestion.Answer);

            Assert.IsNotNull(outputQuestion.Metadata);

            Assert.IsTrue((inputQuestion.Metadata.CreationDate - outputQuestion.Metadata.CreationDate).TotalSeconds < 1);
            Assert.AreEqual(inputQuestion.Metadata.Author, outputQuestion.Metadata.Author);
            Assert.AreEqual(inputQuestion.Metadata.Comment, outputQuestion.Metadata.Comment);

            Assert.IsNotNull(outputQuestion.Nomination);

            Assert.AreEqual(inputQuestion.Nomination.Title, outputQuestion.Nomination.Title);
        }

        [TestMethod]
        public void CantGetAnUnexistingQuestion()
        {
            QuestionWithMetadata question = repository.GetQuestionById(-1);

            Assert.IsNull(question);
        }

        [TestMethod]
        public void CanClearQuestionsRepository()
        {
            repository.Add(new QuestionWithMetadata
                {
                    Metadata = new QuestionMetadata
                    {
                        CreationDate = DateTime.UtcNow
                    },
                    Text = "Knock knock!",
                    Answer = "Who's there?"
                });

            IEnumerable<QuestionWithMetadata> all = repository.GetAllQuestions();

            Assert.IsTrue(all.Count() >= 1);

            repository.Clear();

            all = repository.GetAllQuestions();

            Assert.AreEqual(0, all.Count());
        }
    }
}
