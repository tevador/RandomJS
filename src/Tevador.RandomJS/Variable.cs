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

namespace Tevador.RandomJS
{
    class Variable : IVariable
    {
        public string Name { get; private set; }
        public bool IsParameter { get; private set; }
        public bool IsConstant { get; private set; }
        public bool IsLoopCounter { get; private set; }
        public IScope Parent { get; private set; }
        public Statement Declaration { get; private set; }
        public Expression Initializer { get; set; }

        private Variable()
        {
            Declaration = new VariableDeclaration(this);
        }

        private static string _getVariableName(int index)
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

        public static Variable Generate(IRandom rand, IScope scope, bool isParameter, bool isLoopCounter)
        {
            var v = new Variable();
            v.Name = _getVariableName(scope.VariableCounter++);
            v.Parent = scope;
            v.IsParameter = isParameter;
            v.IsLoopCounter = isLoopCounter;
            if (!v.IsParameter && !v.IsLoopCounter && rand.FlipCoin(scope.Options.ConstVariableChance))
            {
                v.IsConstant = true;
            }
            if (!v.IsParameter && !v.IsLoopCounter)
            {
                v.Initializer = Expression.Generate(rand, scope, null, false);
            }
            return v;
        }
    }
}
