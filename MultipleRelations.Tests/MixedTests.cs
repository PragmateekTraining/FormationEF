using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MultipleRelations.Tests
{
    [TestClass]
    public class MixedTests
    {
    public class Company
    {
        public long Id { get; set; }

        public string Name { get; set; }

        // [ForeignKey("CEO_Id")]
        public Employee CEO { get; set; }

        // public long? CEO_Id { get; set; }

        // [ForeignKey("Company_Id")]
        [InverseProperty("Company")]
        public ICollection<Employee> Employees { get; set; }

        public Company()
        {
            Employees = new HashSet<Employee>();
        }
    }

    public class Employee
    {
        public long Id { get; set; }

        public string Name { get; set; }

        /*[ForeignKey("Company_Id")]
        public Company Company { get; set; }
        public long? Company_Id { get; set; }*/
    }

    public class CorporateContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().HasMany(c => c.Directors).WithOptional(e => e.DirectorOf);
            modelBuilder.Entity<Company>().HasMany(c => c.Managers).WithOptional(e => e.ManagerOf);
            modelBuilder.Entity<Company>().HasMany(c => c.Employees).WithOptional(e => e.EmployeeOf);
        }*/
    }

    [ClassInitialize]
    public static void SetUp(TestContext _)
    {
        Database.SetInitializer(new DropCreateDatabaseAlways<CorporateContext>());
    }

    // [TestMethod]
    public void CanHandleMultipleMixedRelations()
    {
        Company Disney = new Company { Name = "Disney" };

        Employee Mickey = new Employee { Name = "Mickey Mouse" };
        Employee Donald = new Employee { Name = "Donald Duck" };
        Employee Goofy = new Employee { Name = "Goofy Goof" };

        Disney.CEO = Mickey;

        Disney.Employees.Add(Mickey);
        Disney.Employees.Add(Donald);
        Disney.Employees.Add(Goofy);

        using (CorporateContext context = new CorporateContext())
        {
            context.Database.Initialize(true);
        }

        using (CorporateContext context = new CorporateContext())
        {
            context.Companies.Add(Disney);

            context.SaveChanges();
        }

            using (CorporateContext context = new CorporateContext())
            {
                Company company = context.Companies.Include(c => c.CEO)
                                                   .Include(c => c.Employees)
                                                   .Single();

                Assert.AreEqual("Disney", company.Name);

                Assert.IsNotNull(company.CEO);
                Assert.AreEqual(Mickey.Name, company.CEO.Name);

                Assert.AreEqual(3, company.Employees.Count);
            }
        }
    }
}
