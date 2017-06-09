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

        /// <summary>
        /// Tests that a good string can be accepted
        /// </summary>
        [TestMethod]
        public void AcceptGoodString()
        {
            string mockstring = ConstructMockString();
            Assert.IsTrue(SerialInterface.StringIsGood(mockstring));
            
        }

        /// <summary>
        /// Tests that a string that is too long can be rejected
        /// </summary>
        [TestMethod]
        public void RejectLongString()
        {
            // rejects a string that is too long
            string mockstring = ConstructMockString() + "Lol";
            Assert.IsFalse(SerialInterface.StringIsGood(mockstring));
        }

        /// <summary>
        /// Tests that a string with a bad frame can be rejected
        /// </summary>
        [TestMethod]
        public void RejectBadFrameString()
        {
            // rejects a string that has a broken end frame
            string mockstring = ConstructMockString();
            char[] chars = mockstring.ToCharArray();
            chars[101] = (char)0x03;
            mockstring = new string(chars);
            Assert.IsFalse(SerialInterface.StringIsGood(mockstring));
        }

        [TestMethod]
        public void FrameTranslateString()
        {

        }

        private string ConstructMockString()
        {
            uint arrLen = 102;
            uint randVolMin = 300;
            uint randVolMax = 420;
            uint randTempMin = 500;
            uint randTempMax = 4000;

            byte[] sendArr = new byte[arrLen];

            for (int i = 0; i < arrLen; i++)
            {
                sendArr[i] = 0;
            }

            // length of remaining message does not change
            sendArr[1] = 100;

            for (int i = 2; i < 50; i = i + 2)
            {
                Random r = new Random();
                // generate a random number for each
                uint vol = randVolMin + (uint)r.NextDouble() * (randVolMax - randVolMin);
                uint temp = randTempMin + (uint)r.NextDouble() * (randTempMax - randTempMin);

                byte[] _vol = BitConverter.GetBytes(vol);
                byte[] _temp = BitConverter.GetBytes(temp);

                // set array elemnts
                sendArr[i] = _vol[0];
                sendArr[i + 1] = _vol[1];

                sendArr[i + 48] = _temp[0];
                sendArr[i + 49] = _temp[1];

            }

            // add the frameend
            sendArr[100] = 0xFF;
            sendArr[101] = 0xFE;

            char[] charArr = new char[arrLen];

            for (int i = 0; i < arrLen; i++)
            {
                charArr[i] = (char)sendArr[i];
            }

            return new string(charArr);

        }
    }
}
