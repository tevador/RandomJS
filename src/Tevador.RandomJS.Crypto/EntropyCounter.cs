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
using System.Linq;

namespace Tevador.RandomJS.Crypto
{
    class EntropyCounter
    {
        static readonly char _max = '\x7F';
        int[] _counts = new int[_max + 1];
        int strings = 0;
        
        public void Add(string s)
        {
            foreach(char c in s)
            {
                _counts[c]++;
            }
            strings++;
        }

        public double GetEntropy()
        {
            double count = _counts.Sum();
            double entropy = 0;

            for (int i = 0; i < _counts.Length; ++i)
            {
                var p = _counts[i] / count;
                if (p > 0 && p < 1)
                {
                    entropy += - p * Math.Log(p, 2);
                }
            }

            return entropy * count / strings;
        }
    }
}
