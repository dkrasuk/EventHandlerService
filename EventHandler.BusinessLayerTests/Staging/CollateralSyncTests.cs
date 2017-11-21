using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventHandler.BusinessLayer.Staging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandler.BusinessLayer.Staging.Tests
{
    [TestClass()]
    public class CollateralSyncTests
    {
        [TestMethod()]
        public void StartSyncCollateralTest()
        {
            CollateralSync.StartSyncCollateral();
        }
    }
}