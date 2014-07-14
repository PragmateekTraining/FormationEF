using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.ComponentModel;

namespace Audit.Tests
{
    public interface IAuditableEntity
    {
        DateTime CreationTimestamp { get; set; }
        string CreationUser { get; set; }

        DateTime UpdateTimestamp { get; set; }
        string UpdateUser { get; set; }
    }

    // "Cleanest" but most complex
    //public class AuditableEntity : IAuditableEntity
    //{
    //    #region IAuditableEntity implementation
    //    DateTime IAuditableEntity.CreationTimestamp
    //    {
    //        get { return CreationTimestamp; }
    //        set { CreationTimestamp = value; }
    //    }
    //    internal DateTime CreationTimestamp { get; set; }

    //    string IAuditableEntity.CreationUser
    //    {
    //        get { return CreationUser; }
    //        set { CreationUser = value; }
    //    }
    //    internal string CreationUser { get; set; }

    //    DateTime IAuditableEntity.UpdateTimestamp
    //    {
    //        get { return UpdateTimestamp; }
    //        set { UpdateTimestamp = value; }
    //    }
    //    internal DateTime UpdateTimestamp { get; set; }

    //    string IAuditableEntity.UpdateUser
    //    {
    //        get { return UpdateUser; }
    //        set { UpdateUser = value; }
    //    }
    //    internal string UpdateUser { get; set; }
    //    #endregion
    //}

    public class NuclearCode : IAuditableEntity // AuditableEntity
    {
        public long Id { get; set; }

        public string Code { get; set; }

        // Simplest
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime CreationTimestamp { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string CreationUser { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime UpdateTimestamp { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string UpdateUser { get; set; }
    }

    class NuclearDeterrenceContext : DbContext
    {
        public DbSet<NuclearCode> NuclearCodes { get; set; }

        public override int SaveChanges()
        {
            IEnumerable<DbEntityEntry<IAuditableEntity>> auditableEntities = this.ChangeTracker.Entries<IAuditableEntity>();

            foreach (IAuditableEntity auditableEntity in auditableEntities.Where(e => e.State == EntityState.Added)
                                                                          .Select(e => e.Entity))
            {
                auditableEntity.CreationTimestamp = DateTime.UtcNow;
                auditableEntity.CreationUser = Environment.UserName;

                auditableEntity.UpdateTimestamp = auditableEntity.CreationTimestamp;
                auditableEntity.UpdateUser = auditableEntity.CreationUser;
            }

            foreach (IAuditableEntity auditableEntity in auditableEntities.Where(e => e.State == EntityState.Modified)
                                                                          .Select(e => e.Entity))
            {
                auditableEntity.UpdateTimestamp = DateTime.UtcNow;
                auditableEntity.UpdateUser = Environment.UserName;
            }

            return base.SaveChanges();
        }
    }

    [TestClass]
    public class AuditabilityTests
    {
        [ClassInitialize]
        public static void SetUp(TestContext _)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<NuclearDeterrenceContext>());
        }

        [TestMethod]
        public void CanAuditAnEntity()
        {
            DateTime creationDate = default(DateTime);

            using (NuclearDeterrenceContext context = new NuclearDeterrenceContext())
            {
                NuclearCode code = new NuclearCode { Code = "Talonnette" };

                context.NuclearCodes.Add(code);

                context.SaveChanges();

                creationDate = (code as IAuditableEntity).CreationTimestamp;

                Assert.AreNotEqual(default(DateTime), creationDate);

                Assert.IsTrue((DateTime.UtcNow - creationDate) < TimeSpan.FromSeconds(1));
                Assert.AreEqual((code as IAuditableEntity).UpdateTimestamp, creationDate);
            }

            Thread.Sleep(5000);

            using (NuclearDeterrenceContext context = new NuclearDeterrenceContext())
            {
                NuclearCode code = context.NuclearCodes.Single();

                code.Code = "Pepito";

                context.SaveChanges();

                Assert.IsTrue(Math.Abs(((code as IAuditableEntity).CreationTimestamp - creationDate).TotalMilliseconds) < 5);
                Assert.IsTrue((DateTime.UtcNow - (code as IAuditableEntity).UpdateTimestamp) < TimeSpan.FromSeconds(1));
            }
        }
    }
}
