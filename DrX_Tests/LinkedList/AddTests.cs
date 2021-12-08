using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;

namespace DrX_Tests.LinkedList
{
    [TestClass]
    public class AddTests
    {
        [TestMethod]
        public void Add_SingleValue()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);
        }

        [TestMethod]
        public void Add_TwoValue()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
        }

        [TestMethod]
        public void Add_MultipleValue()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            Assert.AreEqual(5, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
        }
    }
}
