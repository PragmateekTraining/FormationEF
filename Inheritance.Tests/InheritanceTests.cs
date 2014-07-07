using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace Inheritance.Tests
{
    class Number
    {
        public long ID { get; set; }
    }

    class Real : Number
    {
    }

    class Single : Real
    {
        public float Value { get; set; }
    }

    class Double : Real
    {
        public double Value { get; set; }
    }

    class Integer : Number
    {
    }

    class Short : Integer
    {
        public short Value { get; set; }
    }

    class Long : Integer
    {
        public long Value { get; set; }
    }

    class TPHNumbersContext : DbContext
    {
        public DbSet<Number> Numbers { get; set; }

        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Single>().ToTable("singles");
            modelBuilder.Entity<Double>().ToTable("doubles");
        }*/
    }

    [TestClass]
    public class InheritanceTests
    {
        [TestMethod]
        public void CanPersistAnInheritanceHierarchy()
        {
            using (TPHNumbersContext context = new TPHNumbersContext())
            {
                context.Numbers.Add(new Single { Value = 123.456f });
                context.Numbers.Add(new Double { Value = 456.789 });
                context.Numbers.Add(new Short { Value = 123 });
                context.Numbers.Add(new Long { Value = 456 });

                context.SaveChanges();
            }

            using (TPHNumbersContext context = new TPHNumbersContext())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                Debug.WriteLine("===== Reals =====");

                foreach (dynamic real in context.Numbers.OfType<Real>())
                {
                    double v = real.Value;

                    Debug.WriteLine(v);
                }

                Debug.WriteLine("===== Integers =====");

                foreach (dynamic integer in context.Numbers.OfType<Integer>())
                {
                    double v = integer.Value;

                    Debug.WriteLine(v);
                }

                Debug.WriteLine("===== Shorts =====");

                foreach (dynamic @short in context.Numbers.OfType<Short>())
                {
                    double v = @short.Value;

                    Debug.WriteLine(v);
                }
            }
        }
    }
}
