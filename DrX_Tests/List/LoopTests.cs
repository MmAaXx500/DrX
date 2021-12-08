using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;
namespace DrX_Tests.List
{
    [TestClass]
    public class LoopTests
    {
        [TestMethod]
        public void Loop()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            int expected = 0;
            for (int i = 0; i < list.Length; i++)
            {
                Assert.AreEqual(expected, list[i]);
                expected++;
            }
        }
    }
}
