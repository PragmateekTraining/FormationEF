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
        void Add(BasicQuestion question);
        BasicQuestion GetQuestionById(long id);
        IEnumerable<BasicQuestion> GetAllQuestions();
        void Clear();
    }
}
