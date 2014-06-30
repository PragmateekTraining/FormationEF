using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repositories
{
    public interface IQuestionBasesRepository
    {
        void Add(QuestionBase question);
        QuestionBase GetQuestionById(long id);
        IEnumerable<QuestionBase> GetAllQuestions();
        void Clear();
    }
}
