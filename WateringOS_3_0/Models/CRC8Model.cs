
/* DISCLAIMER

Watering OS - (C) Michael Kollmeyer 2020  
  
This file is part of WateringOS.

    WateringOS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    WateringOS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with WateringOS.  If not, see<https://www.gnu.org/licenses/>.

*/


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
