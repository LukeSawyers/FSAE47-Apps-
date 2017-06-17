using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccumulatorMonitorM017.Backend;

namespace AccumulatorMonitorM017.Tests
{
    [TestClass]
    public class DataLoggerTests
    {
        DataLogger logger;

        /// <summary>
        /// Tests that the datalogger can log a frame
        /// </summary>
        [TestMethod]
        public void CanLogFrame()
        {
            // setup
            string Dir = AppDomain.CurrentDomain.BaseDirectory;

            logger = new DataLogger("\\Test Logs");
            DataFrame f = MockDataFrameFactory.createRandomGoodFrame();

            // act and assert
            Assert.IsTrue(logger.Log(f));

        }
    }
}
