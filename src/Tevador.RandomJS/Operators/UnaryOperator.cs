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

namespace Tevador.RandomJS.Operators
{
    sealed class UnaryOperator : Operator
    {
        public readonly static UnaryOperator Not = new UnaryOperator("!");
        public readonly static UnaryOperator Plus = new UnaryOperator("+");
        public readonly static UnaryOperator Typeof = new UnaryOperator("typeof ");
        public readonly static UnaryOperator Minus = new UnaryOperator("-", OperatorRequirement.NumericOnly);
        public readonly static UnaryOperator Sqrt = new UnaryOperator("Math.sqrt", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall | OperatorRequirement.LimitedPrecision | OperatorRequirement.RhsNonnegative);
        public readonly static UnaryOperator Abs = new UnaryOperator("Math.abs", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall);
        public readonly static UnaryOperator Ceil = new UnaryOperator("Math.ceil", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall);
        public readonly static UnaryOperator Floor = new UnaryOperator("Math.floor", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall);
        public readonly static UnaryOperator Sin = new UnaryOperator("Math.sin", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall | OperatorRequirement.LimitedPrecision);
        public readonly static UnaryOperator Cos = new UnaryOperator("Math.cos", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall | OperatorRequirement.LimitedPrecision);
        public readonly static UnaryOperator Exp = new UnaryOperator("Math.exp", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall | OperatorRequirement.LimitedPrecision);
        public readonly static UnaryOperator Log = new UnaryOperator("Math.log", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall | OperatorRequirement.LimitedPrecision | OperatorRequirement.RhsNonnegative);
        public readonly static UnaryOperator Atan = new UnaryOperator("Math.atan", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall | OperatorRequirement.LimitedPrecision);

        private UnaryOperator(string symbol, OperatorRequirement flag = OperatorRequirement.None)
            : base(symbol, flag)
        {

        }
    }
}
