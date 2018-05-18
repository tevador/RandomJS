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

using Tevador.RandomJS.Expressions;
using Tevador.RandomJS.Statements;

namespace Tevador.RandomJS
{
    class Variable : IVariable
    {
        public string Name { get; set; }
        public bool IsParameter { get; set; }
        public bool IsConstant { get; set; }
        public bool IsLoopCounter { get; set; }
        public IScope Parent { get; set; }
        public Statement Declaration { get; set; }
        public Expression Initializer { get; set; }

        public Variable()
        {
            Declaration = new VariableDeclaration(this);
        }

        public static string GetVariableName(int index)
        {
            string str = string.Empty;
            while (index >= 0)
            {
                int mod = index % 26;
                str = (char)('a' + mod) + str;
                index = index / 26 - 1;
            }
            return str;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
