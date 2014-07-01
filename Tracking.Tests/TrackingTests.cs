using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ComponentModel;

namespace Tracking.Tests
{
    class Customer
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class AutoTrackedCustomer
    {
        public virtual long ID { get; set; }
        private string name;
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    class CustomersContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
    }

    class AutoTrackedCustomersContext : DbContext
    {
        public DbSet<AutoTrackedCustomer> Customers { get; set; }
    }

    [TestClass]
    public class TrackingTests
    {
        [TestMethod]
        public void CanTrackAnEntity()
        {
            CustomersContext context = new CustomersContext();

            Customer customer = new Customer { Name = "John Doe" };

            DbEntityEntry<Customer> entry = context.Entry(customer);

            Assert.AreEqual(EntityState.Detached, entry.State);

            Customer output = context.Customers.Add(customer);

            Assert.AreSame(customer, output);

            Assert.AreEqual(EntityState.Added, entry.State);

            context.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, entry.State);

            customer.Name = "Chuck Norris";

            context.ChangeTracker.DetectChanges();

            Assert.AreEqual(EntityState.Modified, entry.State);
        }

        [TestMethod]
        public void CanAutoTrackAnEntity()
        {
            AutoTrackedCustomersContext context = new AutoTrackedCustomersContext();

            AutoTrackedCustomer customer = context.Customers.Create();

            DbEntityEntry<AutoTrackedCustomer> entry = context.Entry(customer);

            Assert.AreEqual(EntityState.Detached, entry.State);

            AutoTrackedCustomer output = context.Customers.Add(customer);

            Assert.AreSame(customer, output);

            Assert.AreEqual(EntityState.Added, entry.State);

            context.SaveChanges();

            Assert.AreEqual(EntityState.Unchanged, entry.State);

            customer.Name = "Chuck Norris";

            Assert.AreEqual(EntityState.Modified, entry.State);
        }
    }
}
