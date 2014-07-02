using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    public abstract class QuestionsRepositoryTests
    {
        protected IQuestionBasesRepository repository = null;

        protected QuestionsRepositoryTests(IQuestionBasesRepository repository)
        {
            this.repository = repository;
        }

        public virtual void CanAddAndGetQuestionsBack()
        {
            BasicQuestion inputQuestion = new BasicQuestion
            {
                CreationDate = DateTime.Now,
                Text = "1 + 1?",
                Answer = "2",
                IsOptional = true
            };

            /*BasicQuestion question = new BasicQuestion
            {
                CreationDate = DateTime.UtcNow,
                Text = "Why is the sky blue?",
                Answer = "Because."
            };*/

            repository.Add(inputQuestion);

            Assert.IsTrue(inputQuestion.ID > 0);

            BasicQuestion outputQuestion = repository.GetQuestionById(inputQuestion.ID);

            Assert.AreEqual(inputQuestion.ID, outputQuestion.ID);
            Assert.IsTrue((inputQuestion.CreationDate - outputQuestion.CreationDate).TotalSeconds < 1);
            Assert.AreEqual(inputQuestion.Text, outputQuestion.Text);
            Assert.AreEqual(inputQuestion.Answer, outputQuestion.Answer);
            Assert.AreEqual(inputQuestion.IsOptional, outputQuestion.IsOptional);
        }

        public virtual void CantGetAnUnexistingQuestion()
        {
            BasicQuestion question = repository.GetQuestionById(-1);

            Assert.IsNull(question);
        }

        public virtual void CanClearQuestionsRepository()
        {
            repository.Add(new BasicQuestion
                {
                    CreationDate = DateTime.UtcNow,
                    Text = "Knock knock!",
                    Answer = "Who's there?"
                });

            IEnumerable<BasicQuestion> all = repository.GetAllQuestions();

            Assert.IsTrue(all.Count() >= 1);

            repository.Clear();

            all = repository.GetAllQuestions();

            Assert.AreEqual(0, all.Count());
        }
    }
}
