using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.OsuFileReader;

namespace OsuFileIO.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var factory = new OsuFileReaderFactory(@"M:\osu!\Songs\1069112 Celldweller - Good L_ck (Yo_'re F_cked)\Celldweller - Good L_ck (Yo_'re F_cked) (Daycore) [piroshki's Normal].osu");
            var reader = factory.Build();
            var map = reader.ReadAll();
        }
    }
}
