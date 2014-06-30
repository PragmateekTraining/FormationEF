using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    [TestClass]
    public class QuestionnairesRepositoryTests
    {
        EFQuestionnairesRepository repository = new EFQuestionnairesRepository();

        [TestMethod]
        public void CanAddAndGetQuestionnairesBack()
        {
            Questionnaire inputQuestionnaire = new Questionnaire
            {
                Title = "Mathematics",
                Questions = new List<QuestionBase>
                {
                    new QuestionBase
                    {
                        CreationDate = DateTime.UtcNow,
                        Text = "1 / 1 ?",
                        Answer = "1"
                    },
                    new QuestionBase
                    {
                        CreationDate = DateTime.UtcNow,
                        Text = "1 / 0 ?",
                        Answer = "+Infinity"
                    },
                    new QuestionBase
                    {
                        CreationDate = DateTime.UtcNow,
                        Text = "0 / 0 ?",
                        Answer = "NaN"
                    }
                }
            };

            repository.Add(inputQuestionnaire);

            Assert.IsTrue(inputQuestionnaire.ID > 0);

            Questionnaire outputQuestionnaire = repository.GetQuestionnaireById(inputQuestionnaire.ID);

            Assert.AreEqual(inputQuestionnaire.ID, outputQuestionnaire.ID);
            Assert.AreEqual(inputQuestionnaire.Title, outputQuestionnaire.Title);
        }

        /*[TestMethod]
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
        }*/
    }
}
