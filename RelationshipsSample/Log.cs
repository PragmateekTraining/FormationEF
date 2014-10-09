using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum Level
    {
        _ = 0,
        INFO,
        WARNING,
        ERROR
    }

    public class Log
    {
        public int ID { get; set; }
        public Level Level { get; set; }
        public string Message { get; set; }
    }
}
