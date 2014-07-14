using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MultipleRelations.Tests
{
    [TestClass]
    public class OneToManyTests
    {
        public class Company
        {
            public long Id { get; set; }

            public string Name { get; set; }

            [ForeignKey("DirectorRole_Company_Id")]
            public ICollection<Employee> Directors { get; set; }
            [ForeignKey("ManagerRole_Company_Id")]
            public ICollection<Employee> Managers { get; set; }
            [ForeignKey("EmployeeRole_Company_Id")]
            public ICollection<Employee> Employees { get; set; }

            public Company()
            {
                Directors = new HashSet<Employee>();
                Managers = new HashSet<Employee>();
                Employees = new HashSet<Employee>();
            }
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

            Disney.Directors.Add(Mickey);
            Disney.Directors.Add(Donald);

            Disney.Managers.Add(Goofy);

            Disney.Employees.Add(Mickey);
            Disney.Employees.Add(Donald);
            Disney.Employees.Add(Goofy);

            using (CorporateContext context = new CorporateContext())
            {
                context.Companies.Add(Disney);

                context.SaveChanges();
            }

            using (CorporateContext context = new CorporateContext())
            {
                Company company = context.Companies.Include(c => c.Directors)
                                                   .Include(c => c.Managers)
                                                   .Include(c => c.Employees)
                                                   .Single();

                Assert.AreEqual("Disney", company.Name);

                Assert.AreEqual(2, company.Directors.Count);
                Assert.AreEqual(1, company.Managers.Count);
                Assert.AreEqual(3, company.Employees.Count);

                Assert.AreEqual(Mickey.Name, company.Directors.ElementAt(0).Name);
                Assert.AreEqual(Donald.Name, company.Directors.ElementAt(1).Name);

                Assert.AreEqual(Goofy.Name, company.Managers.Single().Name);
            }
        }
    }
}
