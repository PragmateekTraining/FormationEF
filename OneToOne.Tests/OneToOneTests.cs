using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Data.Entity;

namespace OneToOne.Tests
{
    class PhoneNumber
    {
        public long ID { get; set; }

        public DateTime CreationDate { get; set; }

        public string Value { get; set; }

        //[Required]
        public SIMCard SIM { get; set; }
    }

    class SIMCard
    {
        public long ID { get; set; }

        public string Code { get; set; }

        [Required]
        public PhoneNumber PhoneNumber { get; set; }
    }

    class TaxiPhoneContext : DbContext
    {
        public DbSet<SIMCard> Cards { get; set; }
    }

    [TestClass]
    public class OneToOneTests
    {
        [TestMethod]
        public void CanPersistAOneToOneRelation()
        {
            long id = 0;

            using (TaxiPhoneContext context = new TaxiPhoneContext())
            {
                SIMCard card = new SIMCard
                {
                    Code = "01234567891011121314",
                    PhoneNumber = new PhoneNumber
                    {
                        CreationDate = new DateTime(2000, 01, 01),
                        Value = "0102030405"
                    }
                };

                context.Cards.Add(card);

                context.SaveChanges();

                Assert.IsTrue(card.ID > 0);
                Assert.IsTrue(card.PhoneNumber.ID > 0);

                id = card.ID;
            }

            using (TaxiPhoneContext context = new TaxiPhoneContext())
            {
                SIMCard card = context.Cards.Include(c => c.PhoneNumber).Single(c => c.ID == id);

                Assert.AreEqual("01234567891011121314", card.Code);

                Assert.IsNotNull(card.PhoneNumber);

                Assert.AreEqual("0102030405", card.PhoneNumber.Value);

                Assert.AreSame(card, card.PhoneNumber.SIM);
            }
        }
    }
}
