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
    class ExpressionStatement<T> : Statement
        where T : Expression
    {
        private T _expr;

        public ExpressionStatement(T expr)
        {
            _expr = expr;
        }

        public override void WriteTo(TextWriter w)
        {
            _expr.WriteTo(w);
            w.Write(";");
        }
    }
}
