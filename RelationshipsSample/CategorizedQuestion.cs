using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CategorizedQuestion : QuestionBase
    {
        public string MainCategory { get; set; }
        public string Section { get; set; }
        public string SubSection { get; set; }
    }
}
