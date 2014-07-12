using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Tracing.Tests
{
    public class Customer
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class CRMContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
    }

    [TestClass]
    public class TracingTests
    {
        [TestMethod]
        public void CanTraceSQLRequest()
        {
            using (CRMContext context = new CRMContext())
            {
                context.Database.Log += Debug.WriteLine;

                context.Customers.Add(new Customer { Name = "Chuck Norris" });

                context.SaveChanges();
            }
        }
    }
}
