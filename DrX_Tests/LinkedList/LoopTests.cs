using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;
namespace DrX_Tests.LinkedList
{
    [TestClass]
    public class LoopTests
    {
        [TestMethod]
        public void Loop()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            int expected = 0;
            for (LinkedListNode<int> item = list.First; item != null; item = item.Next)
            {
                Assert.AreEqual(expected, item.Data);
                expected++;
            }
        }
    }
}
