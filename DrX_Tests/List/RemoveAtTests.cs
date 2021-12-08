using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;

namespace DrX_Tests.List
{
    [TestClass]
    public class RemoveAtTests
    {
        [TestMethod]
        public void RemoveAt_SingleValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);

            list.RemoveAt(0);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void RemoveAt_TwoValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);

            list.RemoveAt(1);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);

            list.RemoveAt(0);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void RemoveAt_TwoValue_Shift()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);

            list.RemoveAt(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(1, list[0]);

            list.RemoveAt(0);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void RemoveAt_MultipleValue()
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

            list.RemoveAt(2);

            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);

            list.RemoveAt(2);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(4, list[2]);

            list.RemoveAt(0);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(4, list[1]);

            list.RemoveAt(1);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(1, list[0]);

            list.RemoveAt(0);

            Assert.AreEqual(0, list.Length);
        }
    }
}
