using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Validation.Tests
{
    [TestClass]
    public class ValidationTests
    {
        class Taxpayer
        {
            public long Id { get; set; }

            [Required]
            [StringLength(maximumLength: 200, MinimumLength = 5)]
            // [MinLength(5), MaxLength(200)]
            public string Name { get; set; }

            public string Needs { get; set; }

            [Range(18, int.MaxValue)]
            public int Age { get; set; }

            // [DataType(DataType.EmailAddress)]
            [EmailAddress]
            public string EMail { get; set; }

            [RegularExpression("^[1-3][0-9]{4}(2[AB]|[0-9]{2})[0-9]{6}([0-8][0-9]|9[0-7])$")]
            public string SocialSecurityNumber { get; set; }
        }

        class TaxContext : DbContext
        {
            public DbSet<Taxpayer> Taxpayers { get; set; }
        }

        private void AssertThrows<T>(Action action)
            where T : Exception
        {
            bool hasThrown = false;
            try
            {
                action();
            }
            catch (T)
            {
                hasThrown = true;
            }

            Assert.IsTrue(hasThrown);
        }

        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<TaxContext>());
        }

        [TestMethod]
        public void CanForceAValueToBeProvided()
        {
            using (TaxContext context = new TaxContext())
            {
                Taxpayer anonymousTaxpayer = new Taxpayer { Age = 50 };
                context.Taxpayers.Add(anonymousTaxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(1, errors.Length);
                Assert.AreEqual(1, errors[0].ValidationErrors.Count);
                Assert.AreEqual("Name", errors[0].ValidationErrors.Single().PropertyName);

                AssertThrows<DbEntityValidationException>(() => context.SaveChanges());
            }

            using (TaxContext context = new TaxContext())
            {
                Taxpayer goodTaxpayer = new Taxpayer { Name = "Pierre Dupont", Age = 50 };
                context.Taxpayers.Add(goodTaxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(0, errors.Length);

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanForceANumericalValueToBeRanged()
        {
            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Kevin Dupont", Age = 15 };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(1, errors.Length);
                Assert.AreEqual(1, errors[0].ValidationErrors.Count);
                Assert.AreEqual("Age", errors[0].ValidationErrors.Single().PropertyName);

                AssertThrows<DbEntityValidationException>(() => context.SaveChanges());
            }

            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Kevin Dupont", Age = 20 };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(0, errors.Length);

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanEnsureTheFormatOfAValue()
        {
            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Pierre Dupont", Age = 50, EMail = "kevin_arobase_gmail_dot_com" };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(1, errors.Length);
                Assert.AreEqual(1, errors[0].ValidationErrors.Count);
                Assert.AreEqual("EMail", errors[0].ValidationErrors.Single().PropertyName);

                AssertThrows<DbEntityValidationException>(() => context.SaveChanges());
            }

            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Pierre Dupont", Age = 50, EMail = "kevin@gmail.com" };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(0, errors.Length);

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanRangeTheLengthOfAString()
        {
            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Al O", Age = 50 };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(1, errors.Length);
                Assert.AreEqual(1, errors[0].ValidationErrors.Count);
                Assert.AreEqual("Name", errors[0].ValidationErrors.Single().PropertyName);

                AssertThrows<DbEntityValidationException>(() => context.SaveChanges());
            }

            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Alexandre O", Age = 50 };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(0, errors.Length);

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanForceAStringToMatchARegex()
        {
            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Pierre Dupont", Age = 50, SocialSecurityNumber = "012345678912345" };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(1, errors.Length);
                Assert.AreEqual(1, errors[0].ValidationErrors.Count);
                Assert.AreEqual("SocialSecurityNumber", errors[0].ValidationErrors.Single().PropertyName);

                AssertThrows<DbEntityValidationException>(() => context.SaveChanges());
            }

            using (TaxContext context = new TaxContext())
            {
                Taxpayer taxpayer = new Taxpayer { Name = "Pierre Dupont", Age = 50, SocialSecurityNumber = "177023523800522" };
                context.Taxpayers.Add(taxpayer);

                DbEntityValidationResult[] errors = context.GetValidationErrors().ToArray();
                Assert.AreEqual(0, errors.Length);

                context.SaveChanges();
            }
        }
    }
}
