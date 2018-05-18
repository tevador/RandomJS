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

using Tevador.RandomJS.Operators;

namespace Tevador.RandomJS.Statements
{
    class VariableDeclaration : Statement
    {
        IVariable _value;

        public VariableDeclaration(IVariable v)
        {
            _value = v;
        }

        public override void WriteTo(System.IO.TextWriter w)
        {
            w.Write(_value.IsConstant ? "const " : "let ");
            w.Write(_value.Name);
            if (_value.Initializer != null)
            {
                w.Write(AssignmentOperator.Basic);
                _value.Initializer.WriteTo(w);
            }
            w.Write(";");
        }
    }
}
