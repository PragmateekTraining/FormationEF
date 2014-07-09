using System;

namespace Model
{
    public class BasicQuestion
    {
        public long ID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public bool IsOptional { get; set; }
    }
}
