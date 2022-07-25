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
            long date = AppLink.LongFromDate(DateTime.Now.AddDays(-3));
            long date1 = AppLink.LongFromDate(DateTime.Now.AddDays(-2));
            long date2 = AppLink.LongFromDate(DateTime.Now.AddDays(-1));
            long date3 = AppLink.LongFromDate(DateTime.Now);
            Assert.AreEqual(1126900000, date);
        }


    }
}