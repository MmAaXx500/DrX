using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;

namespace DrX_Tests.LinkedList
{
    [TestClass]
    public class RemoveTests
    {
        [TestMethod]
        public void Remove_SingleValue()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);

            list.Remove(0);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void Remove_TwoValue()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);

            list.Remove(1);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);

            list.Remove(0);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void Remove_TwoValue_Shift()
        {
            LinkedList<int> list = new LinkedList<int>();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);

            list.Remove(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(1, list[0]);

            list.Remove(1);

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void Remove_MultipleValue()
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

            list.Remove(2);

            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);

            list.Remove(3);

            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(4, list[2]);

            list.Remove(0);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(4, list[1]);

            list.Remove(4);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(1, list[0]);

            list.Remove(1);

            Assert.AreEqual(0, list.Length);
        }
    }
}
