using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Tests
{
    [TestClass()]
    public class BarCodeTests
    {
        [TestMethod()]
        public void GenerateQrCodeTest()
        {
            BarCode.GenerateQrCode("1235468446168451480", DateTime.Now, "120.13", 21416185);
            Assert.IsTrue(true);
        }
    }
}