using System;
using System.Collections.Generic;
using System.Linq;

using Model;
using Repositories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    [TestClass]
    public class QuestionnairePresentationsRepositoryTests
    {
        protected EFQuestionnairePresentationsRepository repository = new EFQuestionnairePresentationsRepository();

        [TestMethod]
        public void CanAddAndGetPresentationsBack()
        {
            repository.Clear();

            QuestionnairePresentation inputPresentation = new QuestionnairePresentation
            {
                Title = "Prez",
                Pages = new List<QuestionnairePresentationPage>()
            };

            inputPresentation.Pages.Add(new QuestionnairePresentationPage
                    {
                        Title = "Welcome",
                        Content = "Fill it or you're fired!",
                        //Presentation = inputPresentation
                    });

            inputPresentation.Pages.Add(new QuestionnairePresentationPage
            {
                Title = "Details",
                Content = "Choose only the right answers.",
                //Presentation = inputPresentation
            });

            inputPresentation.Pages.Add(new QuestionnairePresentationPage
            {
                Title = "Credits",
                Content = "Your dear boss.",
                //Presentation = inputPresentation
            });

            var o = repository.Add(inputPresentation);

            Assert.IsTrue(inputPresentation.ID > 0);

            QuestionnairePresentation outputPresentation = repository.GetPresentationById(inputPresentation.ID);

            Assert.AreSame(inputPresentation, outputPresentation);

            repository.RemovePresentationPage(inputPresentation, inputPresentation.Pages[2]);

            repository.SaveAll();

            /*Assert.AreEqual(inputQuestion.ID, outputQuestion.ID);
            Assert.IsTrue((inputQuestion.CreationDate - outputQuestion.CreationDate).TotalSeconds < 1);
            Assert.AreEqual(inputQuestion.Text, outputQuestion.Text);
            Assert.AreEqual(inputQuestion.Answer, outputQuestion.Answer);
            Assert.AreEqual(inputQuestion.IsOptional, outputQuestion.IsOptional);*/
        }
    }
}
