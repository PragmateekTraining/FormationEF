using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TaggedQuestion : CategorizedQuestion
    {
        public string TagsSeparator { get; set; }
        public string Tags { get; set; }
    }
}
