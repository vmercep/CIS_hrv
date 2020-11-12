using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class AppLinkTests
    {
        [TestMethod()]
        public void LongFromDateTest()
        {
            long date=AppLink.LongFromDate(DateTime.Now.AddDays(-3));
            Assert.AreEqual(1126900000, date);
        }
    }
}