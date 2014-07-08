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
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
    }

    [TestClass]
    public class QueriesTests
    {
        [TestMethod]
        public void CanQueryWithLINQToEntities()
        {
            Department marketing = new Department
            {
                Name = "Marketing",
                Employees = new[]
                {
                    new Employee
                    {
                        Name = "Tracy Boobs",
                        HireDate = new DateTime(2010, 01, 01)
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
            }
        }
    }
}
