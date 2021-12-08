using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;

namespace DrX_Tests.List
{
    [TestClass]
    public class ArrayToListTests
    {
        [TestMethod]
        public void ArrayToList()
        {
            int[] arr = { 0, 1, 2, 3, 4, 5 };
            List<int> list = new(arr);

            Assert.AreEqual(6, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
            Assert.AreEqual(5, list[5]);
        }
    }
}
