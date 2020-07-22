using System;
using IPSender;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class IPResolverTest
    {
        [TestMethod]
        public void GetIPTest()
        {
            // replace with your IP
            Assert.AreEqual("10.6.0.118", IPResolver.GetIP());
        }
    }
}
