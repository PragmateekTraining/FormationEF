using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace RecursiveAssociation.Tests
{
    public class Folder
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public virtual IList<Folder> SubFolders { get; set; }
    }

    public class FSContext : DbContext
    {
        public DbSet<Folder> Folders { get; set; }
    }

    [TestClass]
    public class FSHierarchyTests
    {
        [TestMethod]
        public void CanStoreAFoldersHierarchy()
        {
            long id = 0;

            using (FSContext context = new FSContext())
            {
                Folder root = new Folder
                {
                    Name = "/",
                    SubFolders = new[]
                    {
                        new Folder { Name = "tmp" },
                        new Folder
                        {
                            Name = "home",
                            SubFolders = new[]
                            {
                                new Folder{ Name = "joe" },
                                new Folder{ Name = "bob" }
                            }
                        }
                    }
                };

                context.Folders.Add(root);

                context.SaveChanges();

                id = root.ID;
            }

            using (FSContext context = new FSContext())
            {
                context.Database.Log += sql => Debug.WriteLine(sql);

                Folder root = context.Folders.Single(f => f.ID == id);

                Assert.AreEqual("/", root.Name);

                Assert.IsNotNull(root.SubFolders);
                Assert.AreEqual(2, root.SubFolders.Count);

                Assert.AreEqual("tmp", root.SubFolders[0].Name);
                Assert.AreEqual("home", root.SubFolders[1].Name);

                Assert.IsNotNull(root.SubFolders[1].SubFolders);
                Assert.AreEqual(2, root.SubFolders[1].SubFolders.Count);

                Assert.AreEqual("joe", root.SubFolders[1].SubFolders[0].Name);
                Assert.AreEqual("bob", root.SubFolders[1].SubFolders[1].Name);
            }
        }
    }
}
