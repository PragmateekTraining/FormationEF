using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternSample.Tests
{
    [TestClass]
    public class EFQuestionsRepositoryTests : QuestionsRepositoryTests
    {
        public EFQuestionsRepositoryTests()
            : base(new EFQuestionsRepository())
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
            base.CanClearQuestionsRepository();
        }
    }
}
