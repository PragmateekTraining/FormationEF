using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    [TestClass]
    public class TaggedQuestionsRepositoryTests
    {
        EFTaggedQuestionsRepository repository = new EFTaggedQuestionsRepository();

        public TaggedQuestionsRepositoryTests()
            : this(new EFTaggedQuestionsRepository())
        {
        }

        protected TaggedQuestionsRepositoryTests(EFTaggedQuestionsRepository repository)
        {
            this.repository = repository;
        }

        [TestMethod]
        public virtual void CanAddAndGetQuestionsBack()
        {
            QuestionBase baseQuestion = new QuestionBase
            {
                Text = "0 / 0?",
                Answer = "NaN",
                CreationDate = DateTime.UtcNow
            };

            repository.Add(baseQuestion);

            TaggedQuestion inputQuestion = new TaggedQuestion
            {
                Text = "0 / 0?",
                Answer = "NaN",
                CreationDate = DateTime.UtcNow,
                MainCategory = "Science",
                Section = "Mathematics",
                SubSection = "Calculus",
                Tags = "zero math division"
            };

            repository.Add(inputQuestion);

            Assert.IsTrue(inputQuestion.ID > 0);

            QuestionBase outputQuestionTmp = repository.GetQuestionById(inputQuestion.ID);

            Assert.IsInstanceOfType(outputQuestionTmp, typeof(TaggedQuestion));

            TaggedQuestion outputQuestion = outputQuestionTmp as TaggedQuestion;

            Assert.AreEqual(inputQuestion.ID, outputQuestion.ID);
            Assert.AreEqual(inputQuestion.Text, outputQuestion.Text);
            Assert.AreEqual(inputQuestion.Answer, outputQuestion.Answer);

            Assert.AreEqual(inputQuestion.MainCategory, outputQuestion.MainCategory);
            Assert.AreEqual(inputQuestion.Section, outputQuestion.Section);
            Assert.AreEqual(inputQuestion.SubSection, outputQuestion.SubSection);

            Assert.AreEqual(inputQuestion.Tags, outputQuestion.Tags);
        }

        [TestMethod]
        public void CanClearQuestionsRepository()
        {
            repository.Add(new TaggedQuestion
                {
                    Text = "Knock knock!",
                    Answer = "Who's there?",
                    CreationDate = DateTime.UtcNow
                });

            IEnumerable<QuestionBase> all = repository.GetAllQuestions();

            Assert.IsTrue(all.Count() >= 1);

            repository.Clear();

            all = repository.GetAllQuestions();

            Assert.AreEqual(0, all.Count());
        }
    }
}
