using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repositories
{
    public interface IQuestionWithMetadataRepository
    {
        void Add(QuestionWithMetadata question);
        QuestionWithMetadata GetQuestionById(long id);
        IEnumerable<QuestionWithMetadata> GetAllQuestions();
        void Clear();
    }
}
