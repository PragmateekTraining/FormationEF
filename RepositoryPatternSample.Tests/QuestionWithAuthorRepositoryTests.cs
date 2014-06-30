using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    [TestClass]
    public class QuestionWithAuthorRepositoryTests
    {
        EFQuestionWithAuthorRepository repository = new EFQuestionWithAuthorRepository();

        [TestMethod]
        public void CanAddAndGetQuestionsBack()
        {
            QuestionWithAuthor inputQuestion = new QuestionWithAuthor
            {
                Author = new Author
                {
                    FirstName = "John",
                    LastName = "Doe"
                },
                Text = "0 / 0?",
                Answer = "NaN"
            };

            repository.Add(inputQuestion);

            Assert.IsTrue(inputQuestion.ID > 0);

            QuestionWithAuthor outputQuestion = repository.GetQuestionById(inputQuestion.ID);

            Assert.AreEqual(inputQuestion.ID, outputQuestion.ID);
            Assert.AreEqual(inputQuestion.Text, outputQuestion.Text);
            Assert.AreEqual(inputQuestion.Answer, outputQuestion.Answer);

            Assert.IsNotNull(outputQuestion.Author);

            Assert.AreEqual(inputQuestion.Author.FirstName, outputQuestion.Author.FirstName);
            Assert.AreEqual(inputQuestion.Author.LastName, outputQuestion.Author.LastName);
        }

        [TestMethod]
        public void CantGetAnUnexistingQuestion()
        {
            QuestionWithAuthor question = repository.GetQuestionById(-1);

            Assert.IsNull(question);
        }

        [TestMethod]
        public void CanClearQuestionsRepository()
        {
            repository.Add(new QuestionWithAuthor
                {
                    Text = "Knock knock!",
                    Answer = "Who's there?"
                });

            IEnumerable<QuestionWithAuthor> all = repository.GetAllQuestions();

            Assert.IsTrue(all.Count() >= 1);

            repository.Clear();

            all = repository.GetAllQuestions();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void CanGetAllAuthors()
        {
            repository.Clear();

            repository.Add(new QuestionWithAuthor { Author = new Author { FirstName = "Mickey", LastName = "Mouse" } });
            repository.Add(new QuestionWithAuthor { Author = new Author { FirstName = "Donald", LastName = "Duck" } });

            IEnumerable<Author> authors = repository.GetAllAuthors();

            Assert.AreEqual(2, authors.Count());

            Author Mickey = authors.FirstOrDefault(author => author.FirstName == "Mickey");
            Author Donald = authors.FirstOrDefault(author => author.FirstName == "Donald");            

            Assert.IsNotNull(Mickey);
            Assert.IsNotNull(Donald);            

            Assert.AreEqual("Mouse", Mickey.LastName);
            Assert.AreEqual("Duck", Donald.LastName);
        }
    }
}
