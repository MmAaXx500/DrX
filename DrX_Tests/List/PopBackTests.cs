using Microsoft.VisualStudio.TestTools.UnitTesting;
using DrX.Utils.Generic;

namespace DrX_Tests.List
{
    [TestClass]
    public class PopBackTests
    {
        [TestMethod]
        public void PopBack_SingleValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);

            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(0, list[0]);

            Assert.AreEqual(0, list.PopBack());

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void PopBack_TwoValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);

            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);

            Assert.AreEqual(1, list.PopBack());

            Assert.AreEqual(1, list.Length);

            Assert.AreEqual(0, list.PopBack());

            Assert.AreEqual(0, list.Length);
        }

        [TestMethod]
        public void PopBack_MultipleValue()
        {
            List<int> list = new();

            Assert.AreEqual(0, list.Length);

            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);


            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);

            int startLen = list.Length;
            for (int i = startLen - 1; i >= 0; i--)
            {
                if (list.Length > 0)
                    Assert.AreEqual(i, list.PopBack());

                Assert.AreEqual(i, list.Length);
            }
        }
    }
}
