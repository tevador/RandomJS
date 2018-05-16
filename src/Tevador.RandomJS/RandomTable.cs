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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Globalization;

namespace Tevador.RandomJS
{
    public abstract class RandomTable<T> : ICollection<TableEntry<T>>, IXmlSerializable
    {
        private List<TableEntry<T>> _items = new List<TableEntry<T>>();
        private double _total;

        public void Add(TableEntry<T> item)
        {
            _items.Add(item);
            _total += item.Weight;
        }

        public void Add(T item, double weight)
        {
            _items.Add(new TableEntry<T>(item, weight));
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

        internal T ChooseRandom(IRandom rand)
        {
            if (Count == 0)
            {
                throw new ProgramOptionsException("RandomTable is empty");
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

        protected abstract void AddValue(string valueStr, double weight, XmlReader reader);

        private void AddDeserializedItem(string valueStr, string weightStr, XmlReader reader)
        {
            double weight;
            if (!double.TryParse(weightStr, NumberStyles.Any, CultureInfo.InvariantCulture, out weight))
                Error("Invalid value of attribute 'weight' = " + weightStr, reader);

            AddValue(valueStr, weight, reader);
        }

        protected void Error(string message, XmlReader reader)
        {
            var li = reader as IXmlLineInfo;
            if (li != null)
                message += " on line " + li.LineNumber;
            throw new ProgramOptionsException(message);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool isEmpty = reader.IsEmptyElement;
            reader.Read();

            if (isEmpty)
                return;

            reader.MoveToContent();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                string valueStr = reader["type"];
                string weightStr = reader["weight"];

                if (valueStr == null) Error("Missing attribute 'type'", reader);
                if (weightStr == null) Error("Missing attribute 'weight'", reader);

                AddDeserializedItem(valueStr, weightStr, reader);

                reader.Read();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var item in _items)
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("type", item.Value.ToString());
                writer.WriteAttributeString("weight", item.Weight.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
        }
    }
}
