using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Diagnostics;

namespace Enumeration.Tests
{
    class Number
    {
        public long ID { get; set; }

        public string Name { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Number> Numbers { get; set; }
    }

    [TestClass]
    public class EnumerationTests
    {
        [TestMethod]
        public void CanEnumerateEntities()
        {
            using (Context context = new Context())
            {
                foreach (string name in new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Height", "Nine", "Ten" })
                {
                    context.Numbers.Add(new Number { Name = name });

                    context.SaveChanges();
                }
            }

            using (Context context = new Context())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                foreach (Number number in context.Numbers)
                {
                    Debug.WriteLine(number.Name);
                }
            }
        }
    }
}
