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

    ------------------------------------------------------------------------

    Based on C code from http://xoshiro.di.unimi.it/xoshiro256plus.c

    Original copyright statement:

    Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)

    To the extent possible under law, the author has dedicated all copyright
    and related and neighboring rights to this software to the public domain
    worldwide. This software is distributed without any warranty.

    See <http://creativecommons.org/publicdomain/zero/1.0/>.
*/

using System;


namespace Tevador.RandomJS
{
    class Xoshiro256Plus : IRandom
    {
        readonly ulong[] _s = new ulong[4];
        ulong _counter = 0;

        public static ulong SplitMix64(ref ulong state)
        {
            ulong z = (state += 0x9e3779b97f4a7c15);
            z = (z ^ (z >> 30)) * 0xbf58476d1ce4e5b9;
            z = (z ^ (z >> 27)) * 0x94d049bb133111eb;
            return z ^ (z >> 31);
        }

        public ulong Counter
        {
            get { return _counter; }
        }

        private ulong Next64()
        {
            ulong result_plus = _s[0] + _s[3];

            ulong t = _s[1] << 17;

            _s[2] ^= _s[0];
            _s[3] ^= _s[1];
            _s[1] ^= _s[2];
            _s[0] ^= _s[3];

            _s[2] ^= t;

            _s[3] = BinaryUtils.ROTL(_s[3], 45);

            _counter++;
            return result_plus;
        }

        public void Seed(byte[] seed)
        {
            if (seed.Length != 4 * sizeof(ulong)) throw new ArgumentException("Invalid seed size");
            _s[0] = BitConverter.ToUInt64(seed, 0 * sizeof(ulong));
            _s[1] = BitConverter.ToUInt64(seed, 1 * sizeof(ulong));
            _s[2] = BitConverter.ToUInt64(seed, 2 * sizeof(ulong));
            _s[3] = BitConverter.ToUInt64(seed, 3 * sizeof(ulong));
        }

        public double Gen()
        {
            return BitConverter.Int64BitsToDouble((long)(0x3FFUL << 52 | Next64() >> 12)) - 1.0;
        }
    }
}
