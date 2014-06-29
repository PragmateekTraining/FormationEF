using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternSample
{
    public interface IQuestionsRepository
    {
        void Add(Question question);
        Question GetQuestionById(long id);
        IEnumerable<Question> GetAllQuestions();
        void Clear();
    }
}
