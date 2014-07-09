using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data.Entity;

namespace ManyToManyWithData.Tests
{
    [TestClass]
    public class ManyToManyWithDataTests
    {
        [TestMethod]
        public void CanRetrieveData()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<MoviesContext>());

            using (MoviesContext context = new MoviesContext())
            {
                foreach (movie movie in context.movies)
                {
                    Debug.WriteLine("{0} :", (object)movie.Title);

                    foreach (performance performance in movie.performances)
                    {
                        Debug.WriteLine("\t{0}: {1}", performance.actor.Name, performance.fee);
                    }
                }

                Debug.WriteLine(new string('=', 20));

                foreach (actor actor in context.actors)
                {
                    Debug.WriteLine("{0} :", (object)actor.Name);

                    foreach (performance performance in actor.performances)
                    {
                        Debug.WriteLine("\t{0}: {1}", performance.movie.Title, performance.fee);
                    }
                }
            }
        }
    }
}
