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
using System.Collections.Generic;
using System.Linq;

namespace Tevador.RandomJS.Crypto
{
    class ListStats<T>
    {
        double[] _sorted;

        public ListStats(List<T> items, Func<T, double> selector)
        {
            _sorted = items.Select(selector).OrderBy(d => d).ToArray();
            var avg = Average = _sorted.Average();
            Min = _sorted[0];
            Max = _sorted[_sorted.Length - 1];
            var sqsum = _sorted.Sum(d => (d - avg) * (d - avg));
            StdDev = Math.Sqrt(sqsum / _sorted.Length);
        }

        public double Average { get; private set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double StdDev { get; set; }

        public double GetPercentile(double p)
        {
            var index = Math.Min(_sorted.Length - 1, (int)(p * _sorted.Length));
            return _sorted[index];
        }
    }
}
