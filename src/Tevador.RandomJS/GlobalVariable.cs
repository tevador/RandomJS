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
using Tevador.RandomJS.Statements;

namespace Tevador.RandomJS
{
    class GlobalVariable : Global, IVariable
    {
        public readonly static GlobalVariable CSUM = new GlobalVariable("__callSum", false, new Literal("0"));
        public readonly static GlobalVariable STRL = new GlobalVariable("__maxStrlen", true);
        public readonly static GlobalVariable ESUM = new GlobalVariable("__errorSum", false, new Literal("0"));
        public readonly static GlobalVariable PREC = new GlobalVariable("__fpMathPrec", true);

        private Statement _declaration;

        public bool IsConstant { get; private set; }
        public Expression Initializer { get; set; }

        public GlobalVariable(string name, bool isConstant = false, Expression initializer = null)
        {
            Name = name;
            IsConstant = isConstant;
            Initializer = initializer;
            _declaration = new VariableDeclaration(this);
        }

        public override void WriteTo(TextWriter w)
        {
            _declaration.WriteTo(w);
        }

        public override Global Clone()
        {
            return new GlobalVariable(Name, IsConstant, Initializer);
        }
    }
}
