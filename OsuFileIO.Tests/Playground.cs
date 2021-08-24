using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.OsuFileReader;

namespace OsuFileIO.Tests
{
    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void TestMethod1()
        {
            using var factory = new OsuFileReaderFactory(@"M:\osu!\Songs\1069112 Celldweller - Good L_ck (Yo_'re F_cked)\Celldweller - Good L_ck (Yo_'re F_cked) (Daycore) [piroshki's Normal].osu");

            if (true)
            {
                var reader = factory.Build();
                var map = reader.ReadAll();
                ;
            }
            
        }
    }
}
