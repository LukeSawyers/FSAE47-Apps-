using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccumulatorMonitorM017.Backend;

namespace AccumulatorMonitorM017.Tests
{
    [TestClass]
    public class SerialInterfaceTests
    {
        SerialInterface s;

        /// <summary>
        /// Tests that there are ports available
        /// </summary>
        [TestMethod]
        public void GetAvailablePorts()
        {
            // set up 
            string[] ports = SerialInterface.GetAvailablePorts();

            //assert
            if(ports.Length == 0) { Assert.Fail(); }
        }

        /// <summary>
        /// Tests that a port can be connected to
        /// </summary>
        [TestMethod]
        public void OpenSerialPort()
        {
            // set up 
            string[] ports = SerialInterface.GetAvailablePorts();

            //assert
            if (ports.Length != 0)
            {
                s = new SerialInterface(ports[0]);
                Assert.IsTrue(s.IsOpen);
                s.Close();
            }
        }

        [TestMethod]
        public void GoodBufferAccepted()
        {
            byte[] buff = MockDataFrameFactory.createRandomGoodBuffer();
            Assert.IsTrue(SerialInterface.BufferIsGood(buff));
        }
    }
}
