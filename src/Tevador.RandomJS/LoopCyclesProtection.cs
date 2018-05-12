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

using System.IO;
using Tevador.RandomJS.Expressions;

namespace Tevador.RandomJS
{
    class LoopCyclesProtection : Expression
    {
        static readonly string _cyclesVaribleName = "_cycles";
        static readonly string _maxCyclesConstantName = "__maxCycles";

        readonly static GlobalVariable _cycles = new GlobalVariable(_cyclesVaribleName) { Initializer = new Literal("0") };
        readonly static GlobalVariable _maxCycles = new GlobalVariable(_maxCyclesConstantName, true);

        public LoopCyclesProtection(int maxCycles)
        {
            _maxCycles.Initializer = new Literal(maxCycles.ToString());
        }

        public void AttachTo(IScope scope)
        {
            scope.Require(_cycles);
            scope.Require(_maxCycles);
        }

        public override void WriteTo(TextWriter w)
        {
            w.Write("{0}++<{1}", _cyclesVaribleName, _maxCyclesConstantName);
        }
    }
}
