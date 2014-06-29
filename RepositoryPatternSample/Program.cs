using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternSample
{
    class Program
    {
        static void Main(string[] args)
        {
            new EFQuestionsRepository().Add(new Question { CreationDate = DateTime.UtcNow });
        }
    }
}
