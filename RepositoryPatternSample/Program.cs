using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model;
using Repositories;

namespace RepositoryPatternSample
{
    class Program
    {
        static void Main(string[] args)
        {
            new EFQuestionBasesRepository().Add(new BasicQuestion { CreationDate = DateTime.UtcNow });
        }
    }
}
