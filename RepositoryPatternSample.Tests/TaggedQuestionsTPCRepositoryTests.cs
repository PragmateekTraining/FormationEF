using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repositories.Tests
{
    [TestClass]
    public class TaggedQuestionsTPCRepositoryTests : TaggedQuestionsRepositoryTests
    {
        public TaggedQuestionsTPCRepositoryTests()
            : base(new EFTaggedQuestionsTPCRepository())
        {
        }

        [TestMethod]
        public override void CanAddAndGetQuestionsBack()
        {
            base.CanAddAndGetQuestionsBack();
        }
    }
}
