using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccumulatorMonitorM017.Backend;

namespace AccumulatorMonitorM017.Tests
{
    public static class MockDataFrameFactory
    {
        public enum TypesOfBadFrame { BadSegmentNumber, BadCRC, BadEnd, PartiallyDropped, }
        private static Random random = new Random();

        /// <summary>
        /// Creates a mock good data frame that should pass the serial interface buffer is good test, an example of a frame we might expect
        /// </summary>
        /// <returns></returns>
        public static DataFrame createRandomGoodFrame()
        {
            byte[] MockArr = new byte[104];
            MockArr[0] = (byte)(random.NextDouble() * 5);
            MockArr[102] = (byte)(random.NextDouble() * 5);

            ushort crc16 = 0;

            for (int i = 2; i < 50; i = i + 2)
            {
                ushort vol = (ushort)RandomInRange(300, 420);
                ushort temp = (ushort)RandomInRange(500, 4000);
                MockArr[i] = (byte)(vol & 0xff);
                MockArr[i + 1] = (byte) (vol >> 8);
                MockArr[i + 48] = (byte)(temp & 0xff);
                MockArr[i + 49] = (byte)(temp >> 8);
                crc16 += vol;
                crc16 += temp;
            }
            MockArr[98] = (byte)(crc16 & 0xff);
            MockArr[99] = (byte)(crc16 >> 8);
            MockArr[100] = 0xFF;
            MockArr[101] = 0xFE;

            return new DataFrame(MockArr);
        }

        /// <summary>
        /// Creates a good mock buffer array, one we might expect to recieve
        /// </summary>
        /// <returns></returns>
        public static byte[] createRandomGoodBuffer()
        {
            byte[] MockArr = new byte[104];
            MockArr[0] = (byte)(random.NextDouble() * 5);
            MockArr[102] = (byte)(random.NextDouble() * 5);

            ushort crc16 = 0;

            for (int i = 2; i < 50; i = i + 2)
            {
                ushort vol = (ushort)RandomInRange(300, 420);
                ushort temp = (ushort)RandomInRange(500, 4000);
                MockArr[i] = (byte)(vol & 0xff);
                MockArr[i + 1] = (byte)(vol >> 8);
                MockArr[i + 48] = (byte)(temp & 0xff);
                MockArr[i + 49] = (byte)(temp >> 8);
                crc16 += vol;
                crc16 += temp;
            }
            MockArr[98] = (byte)(crc16 & 0xff);
            MockArr[99] = (byte)(crc16 >> 8);
            MockArr[100] = 0xFF;
            MockArr[101] = 0xFE;

            return MockArr;
        }

        /*public static DataFrame createBad(TypesOfBadFrame t)
        {
            
        }*/

        private static double RandomInRange(double min, double max)
        {
            if(max > min) { return -1; }

            double range = max - min;
            return ((random.NextDouble() * range) + min);
        }
    }
}
