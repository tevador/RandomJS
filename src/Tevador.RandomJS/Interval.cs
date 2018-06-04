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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tevador.RandomJS
{
    public class Interval : IXmlSerializable
    {
        public int Min { get; set; }
        public int Max { get; private set; }

        private int _span;
        public int Span
        {
            get { return _span; }
            set
            {
                _span = value;
                Max = Min + _span;
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            reader.MoveToContent();
            var value = reader.ReadContentAsString();
            FromString(value, reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Min + "-" + Max;
        }

        public void FromString(string s, XmlReader reader)
        {
            var minmax = s.Split('-');
            int min, max;
            if(minmax.Length != 2 || !int.TryParse(minmax[0], out min) || !int.TryParse(minmax[1], out max))
            {
                throw Error("Invalid interval", reader);
            }
            if (min < 0) throw Error("Min value of interval cannot be negative", reader);
            if (min < 0) throw Error("Max value of interval cannot be negative", reader);
            if(min > max) throw Error("Min value of interval cannot be higher than max value", reader);
            Min = min;
            Max = max;
        }

        private Exception Error(string message, XmlReader reader)
        {
            var li = reader as IXmlLineInfo;
            if (li != null)
                message += " on line " + li.LineNumber;
            return new ProgramOptionsException(message);
        }

        internal int RandomValue(IRandom rand)
        {
            if (Min == Max) return Max;
            return rand.GenInt(Min, Max + 1);
        }
    }
}
