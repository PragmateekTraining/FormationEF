using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics;

namespace Queries.Tests
{
    class Employee
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public DateTime HireDate { get; set; }
    }

    class Department
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public IList<Employee> Employees { get; set; }
    }

    class HRContext : DbContext
    {
        static HRContext()
        {
            // /!\ Only for testing, not production code /!\
            Database.SetInitializer(new DropCreateDatabaseAlways<HRContext>());
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
    }

    [TestClass]
    public class QueriesTests
    {
        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            Department marketing = new Department
            {
                Name = "Marketing",
                Employees = new[]
                {
                    new Employee
                    {
                        Name = "Jane Doe",
                        HireDate = new DateTime(2010, 01, 01)
                    },
                    new Employee
                    {
                        Name = "John Doe",
                        HireDate = new DateTime(2001, 01, 01)
                    }
                }
            };

            Department rd = new Department
            {
                Name = "R&D"
            };

            using (HRContext context = new HRContext())
            {
                context.Departments.Add(marketing);
                context.Departments.Add(rd);

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void IsCaseInsensitive()
        {
            using (HRContext context = new HRContext())
            {
                Department marketing = context.Departments.Single(dpt => dpt.Name == "mArKeTiNg");

                Assert.IsNotNull(marketing);

                Assert.AreEqual("Marketing", marketing.Name);

                Employee John = context.Employees.Single(e => e.Name == "john doe");

                Assert.IsNotNull(John);

                Assert.AreEqual("John Doe", John.Name);
            }
        }

        [TestMethod]
        public void IsImplicitlyBuildingRelationships()
        {
            using (HRContext context = new HRContext())
            {
                Department mrktng = context.Departments.Single(dpt => dpt.Name == "Marketing");

                Assert.IsNotNull(mrktng);

                Assert.IsNull(mrktng.Employees);

                Employee John = context.Employees.Single(e => e.Name == "John Doe");

                Assert.IsNotNull(mrktng.Employees);

                Assert.AreEqual(1, mrktng.Employees.Count);

                Assert.AreEqual("John Doe", mrktng.Employees[0].Name);
            }
        }

        [TestMethod]
        public void CanQueryWithLINQToEntities()
        {
            using (HRContext context = new HRContext())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                Debug.WriteLine("===== Where StartsWith =====");

                context.Employees.Where(e => e.Name.StartsWith("T")).ToList();

                Debug.WriteLine("===== Where Substring =====");

                context.Employees.Where(e => e.Name.Substring(2, 2) == "ab").ToList();

                /*Debug.WriteLine("===== Where Split =====");

                context.Employees.Where(e => e.Name.Split().Length == 2).ToList();*/

                /*Debug.WriteLine("===== Where HireDate =====");

                context.Employees.Where(e => e.HireDate.DayOfWeek == DayOfWeek.Monday).ToList();*/

                Debug.WriteLine("===== Any =====");

                context.Departments.Where(dpt => dpt.Employees.Any()).ToList();

                Debug.WriteLine("===== Count() =====");

                context.Departments.Where(dpt => dpt.Employees.Count() != 0).ToList();

                Debug.WriteLine("===== Count =====");

                context.Departments.Where(dpt => dpt.Employees.Count != 0).ToList();

                Debug.WriteLine("===== Contains =====");

                context.Employees.Where(e => new[] { "Paris", "London", null }.Contains(e.City)).ToList();
            }

            using (HRContext context = new HRContext())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                var results = context.Departments.Select(dpt => new { Department = dpt, RecentEmployees = dpt.Employees.Where(e => e.HireDate > new DateTime(2009, 01, 01)) }).ToList();

                Assert.AreEqual(2, results.Count);

                Assert.AreEqual("Marketing", results[0].Department.Name);

                Assert.AreEqual(1, results[0].Department.Employees.Count);

                Assert.AreEqual("Jane Doe", results[0].Department.Employees[0].Name);
            }
        }
    }
}
