/*
    (c) 2018 tevador <tevador@gmail.com>

    This file is part of Tevador.RandomJS.

    Tevador.RandomJS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Tevador.RandomJS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Tevador.RandomJS.  If not, see<http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Runtime.CompilerServices;

namespace Tevador.RandomJS
{
    public static class BinaryUtils
    {
        public static string ByteArrayToString(byte[] arr)
        {
            StringBuilder sb = new StringBuilder(arr.Length * 2);
            foreach (byte b in arr)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static bool ArraysEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for(int i = 0; i < a.Length; ++i)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ROTL(ulong value, int count)
        {
            return (value << count) | (value >> ((sizeof(ulong) * 8) - count));
        }
    }
}
