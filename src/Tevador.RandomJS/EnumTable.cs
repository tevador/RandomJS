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
using System.Xml;

namespace Tevador.RandomJS
{
    public class EnumTable<T> : RandomTable<T>
        where T : struct, IConvertible
    {
        public EnumTable()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Invalid generic argument type, expected Enum");
        }

        protected override void AddValue(string valueStr, double weight, XmlReader reader)
        {
            T type;
            if (!Enum.TryParse(valueStr, out type))
                Error("Invalid value of attribute 'type' = " + valueStr, reader);

            Add(type, weight);
        }

        private bool IsInList(int index, ulong mask)
        {
            return (_items[index].Value.ToUInt64(null) & mask) != 0;
        }

        internal T ChooseRandom(IRandom rand, T list)
        {
            double total = 0.0;
            ulong mask = list.ToUInt64(null);
            for(int j = 0; j < _items.Count; ++j)
            {
                if (IsInList(j, mask))
                    total += _items[j].Weight;
            }
            if (total == 0)
                return default(T);
            double pivot = rand.Gen() * total;
            int i = 0;
            double probe = IsInList(i, mask) ? _items[i].Weight : 0.0;
            while (probe < pivot)
            {
                ++i;
                if (IsInList(i, mask))
                    probe += _items[i].Weight;
            }
            return _items[i].Value;
        }
    }
}
