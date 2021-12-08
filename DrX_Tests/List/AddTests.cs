using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;

namespace DrX_Tests.List
{
    [TestClass]
    public class AddTests
    {
        [TestMethod]
        public void Add_SingleValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);
        }

        [TestMethod]
        public void Add_TwoValue()
        {
            List<int> list = new();

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
            List<int> list = new();

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

        [TestMethod]
        public void AddAt_MultipleValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.AddAt(0, 0);
            list.AddAt(1, 1);
            list.AddAt(3, 2);
            list.AddAt(2, 2);
            list.AddAt(4, 4);

            Assert.AreEqual(5, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
        }
    }
}
