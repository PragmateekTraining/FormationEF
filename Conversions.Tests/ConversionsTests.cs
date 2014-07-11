using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Conversions.Tests
{
    [Flags]
    enum Ideology
    {
        _ = 0,
        Communism = 1 << 0,
        Socialism = 1 << 1,
        Liberalism = 1 << 2,
        Conservatism = 1 << 3,
        Nationalism = 1 << 4,
        Gaullism = 1 << 5
    }

    class Politician
    {
        public long Id { get; set; }

        public string Name { get; set; }

        private string ideologyAsString;
        [Column("Ideology")]
        internal string IdeologyAsString
        {
            get { return ideologyAsString; }
            set
            {
                ideologyAsString = value;
                ideology = (Ideology)Enum.Parse(typeof(Ideology), value);
            }
        }

        private Ideology ideology;
        [NotMapped]
        public Ideology Ideology
        {
            get { return ideology; }
            set
            {
                ideology = value;
                ideologyAsString = ideology.ToString();
            }
        }

        private string likeActressesAsString;
        [Column("LikeActresses")]
        internal string LikeActressesAsString
        {
            get { return likeActressesAsString; }
            set
            {
                likeActressesAsString = value;
                likeActresses = value == "Yes";
            }
        }

        private bool likeActresses;
        [NotMapped]
        public bool LikeActresses
        {
            get { return likeActresses; }
            set
            {
                likeActresses = value;
                likeActressesAsString = value ? "Yes" : "No";
            }
        }
    }

    class FranceContext : DbContext
    {
        public DbSet<Politician> Politicians { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Politician>().Property(p => p.IdeologyAsString);
            modelBuilder.Entity<Politician>().Property(p => p.LikeActressesAsString);

            // modelBuilder.Entity<Politician>().Ignore(p => p.Ideology);
        }
    }

    [TestClass]
    public class ConversionsTests
    {
        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<FranceContext>());
        }

        [TestMethod]
        public void CanStoreEnumsAndBooleansAsStrings()
        {
            Politician Jacques = new Politician
            {
                Name = "Jacques",
                Ideology = Ideology.Gaullism | Ideology.Liberalism,
                LikeActresses = false
            };

            Politician Nicolas = new Politician
            {
                Name = "Nicolas",
                Ideology = Ideology.Conservatism | Ideology.Liberalism,
                LikeActresses = true
            };

            Politician Francois = new Politician
            {
                Name = "François",
                Ideology = Ideology.Socialism | Ideology.Liberalism,
                LikeActresses = true
            };

            using (FranceContext context = new FranceContext())
            {
                context.Politicians.Add(Jacques);
                context.Politicians.Add(Nicolas);
                context.Politicians.Add(Francois);

                context.SaveChanges();
            }

            using (FranceContext context = new FranceContext())
            {
                Politician outJacques = context.Politicians.Single(p => p.Name == Jacques.Name);
                Politician outNicolas = context.Politicians.Single(p => p.Name == Nicolas.Name);
                Politician outFrancois = context.Politicians.Single(p => p.Name == Francois.Name);

                Assert.AreEqual(Jacques.Ideology, outJacques.Ideology);
                Assert.AreEqual(Jacques.LikeActresses, outJacques.LikeActresses);

                Assert.AreEqual(Nicolas.Ideology, outNicolas.Ideology);
                Assert.AreEqual(Nicolas.LikeActresses, outNicolas.LikeActresses);

                Assert.AreEqual(Francois.Ideology, outFrancois.Ideology);
                Assert.AreEqual(Francois.LikeActresses, outFrancois.LikeActresses);
            }
        }
    }
}
