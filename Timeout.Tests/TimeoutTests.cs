using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace Timeout.Tests
{
    [TestClass]
    public class TimeoutTests
    {
        [TestMethod]
        public void CanChangeTheDefaultTimeout()
        {
            string connectionString = @"Server=.\SqlExpress;Database=TimeoutTests;Integrated Security=true";

            using (DbContext context = new DbContext(connectionString))
            {
                context.Database.ExecuteSqlCommand("WAITFOR DELAY '00:00:02'");
            }

            using (DbContext context = new DbContext(connectionString))
            {
                context.Database.CommandTimeout = 1;

                bool hasThrownException = false;

                try
                {
                    context.Database.ExecuteSqlCommand("WAITFOR DELAY '00:00:02'");
                }
                catch (Exception)
                {
                    hasThrownException = true;
                }

                Assert.IsTrue(hasThrownException);
            }
        }
    }
}
