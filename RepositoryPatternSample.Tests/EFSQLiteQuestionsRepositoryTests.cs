using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Tests
{
    [TestClass]
    public class EFSQLiteQuestionsRepositoryTests : QuestionsRepositoryTests
    {
        public EFSQLiteQuestionsRepositoryTests()
            : base(new EFSQLiteQuestionBasesRepository())
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
