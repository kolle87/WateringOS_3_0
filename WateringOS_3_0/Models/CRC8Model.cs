/// CRC8 Class for RaspberryPi 3
/// to be used with WateringOS >2.0
/// 
/// (C) Michael Kollmeyer, 27-Sept-2019
/// 

namespace WateringOS_3_0
{
    public sealed class CRC8
    {
        static byte[] table = new byte[256];
        const byte poly = 0x07;

        public static byte ComputeChecksum(params byte[] bytes)
        {
            byte crc = 0xFF;
            if (bytes != null && bytes.Length > 0)
            {
                foreach (byte b in bytes)
                {
                    crc = table[crc ^ b];
                }
            }
            return crc;
        }

        static CRC8()
        {
            for (int i = 0; i < 256; i++)
            {
                int temp = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((temp & 0x80) != 0)
                    {
                        temp <<= 1;
                        temp ^= poly;
                    }
                    else
                    {
                        temp <<= 1;
                    }
                }
                table[i] = (byte)temp;
            }
        }
    }
}
