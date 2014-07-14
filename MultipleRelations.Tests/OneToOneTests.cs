using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;

namespace MultipleRelations.Tests
{
    [TestClass]
    public class OneToOneTests
    {
        public class Company
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public Employee CEO { get; set; }
            public Employee CFO { get; set; }
            public Employee CTO { get; set; }
        }

        public class Employee
        {
            public long Id { get; set; }

            public string Name { get; set; }
        }

        public class CorporateContext : DbContext
        {
            public DbSet<Company> Companies { get; set; }
            public DbSet<Employee> Employees { get; set; }
        }

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<CorporateContext>());
        }

        [TestMethod]
        public void CanHandleMultipleRelations()
        {
            Company Disney = new Company { Name = "Disney" };

            Employee Mickey = new Employee { Name = "Mickey Mouse" };
            Employee Donald = new Employee { Name = "Donald Duck" };
            Employee Goofy = new Employee { Name = "Goofy Goof" };

            Disney.CEO = Mickey;
            Disney.CFO = Donald;
            Disney.CTO = Goofy;

            using (CorporateContext context = new CorporateContext())
            {
                context.Companies.Add(Disney);

                context.SaveChanges();
            }

            using (CorporateContext context = new CorporateContext())
            {
                Company company = context.Companies.Include(c => c.CEO)
                                                   .Include(c => c.CFO)
                                                   .Include(c => c.CTO)
                                                   .Single();

                Assert.AreEqual("Disney", company.Name);

                Assert.IsNotNull(company.CEO);
                Assert.AreEqual(Mickey.Name, company.CEO.Name);

                Assert.IsNotNull(company.CFO);
                Assert.AreEqual(Donald.Name, company.CFO.Name);

                Assert.IsNotNull(company.CTO);
                Assert.AreEqual(Goofy.Name, company.CTO.Name);
            }
        }
    }
}
