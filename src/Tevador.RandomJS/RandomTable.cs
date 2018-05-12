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

namespace Tevador.RandomJS
{
    class RandomTable<T> : ICollection<TableEntry<T>>
    {
        private List<TableEntry<T>> _items = new List<TableEntry<T>>();
        private double _total;

        public void Add(TableEntry<T> item)
        {
            _items.Add(item);
            _total += item.Weight;
        }

        public void Add(double weight, T item)
        {
            _items.Add(new TableEntry<T>(weight, item));
            _total += weight;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TableEntry<T> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TableEntry<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TableEntry<T> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TableEntry<T>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T ChooseRandom(IRandom rand)
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("RandomTable is empty");
            }
            double pivot = rand.Gen() * _total;
            int i = 0;
            double probe = _items[i].Weight;
            while (probe < pivot)
            {
                ++i;
                probe += _items[i].Weight;
            }
            return _items[i].Value;          
        }
    }
}
