using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Tests
{
    [TestClass]
    public class SQLiteQuestionsRepositoryTests : QuestionsRepositoryTests
    {
        public SQLiteQuestionsRepositoryTests()
            : base(new SQLiteQuestionBasesRepository())
        {
        }

        [TestMethod]
        public override void CanAddAndGetQuestionsBack()
        {
            base.CanAddAndGetQuestionsBack();
        }

        [TestMethod]
        public override void CantGetAnUnexistingQuestion()
        {
            base.CantGetAnUnexistingQuestion();
        }

        [TestMethod]
        public override void CanClearQuestionsRepository()
        {
            base.CantGetAnUnexistingQuestion();
        }
    }
}
