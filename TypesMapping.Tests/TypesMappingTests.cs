using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TypesMapping.Tests
{
    class Entity
    {
        [Key]
        public int Int { get; set; }
        public uint UInt { get; set; }
        public long Long { get; set; }
        public double Double { get; set; }
        public decimal Decimal { get; set; }
        public bool Bool { get; set; }
        public char Char { get; set; }
        public string String { get; set; }
        public DateTime DateTime { get; set; }
        // public DateTimeOffset DateTimeOffset { get; set; }
        public Guid Guid { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime DateTime2 { get; set; }
        public byte[] ArrayOfBytes { get; set; }
    }

    class Context : DbContext
    {
        public DbSet<Entity> Entities { get; set; }
    }

    [TestClass]
    public class TypesMappingTests
    {
        [TestMethod]
        public void CheckTypesMapping()
        {
            using (Context context = new Context())
            {
                context.Database.Initialize(true);
            }
        }
    }
}
