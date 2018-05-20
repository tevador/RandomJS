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
    public sealed class BinaryOperator : Operator
    {
        public readonly static BinaryOperator Add = new BinaryOperator("+", OperatorRequirement.StringLengthLimit);
        public readonly static BinaryOperator Comma = new BinaryOperator(",");
        public readonly static BinaryOperator Sub = new BinaryOperator("-", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator Mul = new BinaryOperator("*", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator Div = new BinaryOperator("/", OperatorRequirement.NumericOnly | OperatorRequirement.RhsNonzero);
        public readonly static BinaryOperator Mod = new BinaryOperator("%", OperatorRequirement.NumericOnly | OperatorRequirement.RhsNonzero);
        public readonly static BinaryOperator Less = new BinaryOperator("<");
        public readonly static BinaryOperator Greater = new BinaryOperator(">");
        public readonly static BinaryOperator Equal = new BinaryOperator("==");
        public readonly static BinaryOperator NotEqual = new BinaryOperator("!=");
        public readonly static BinaryOperator And = new BinaryOperator("&&");
        public readonly static BinaryOperator Or = new BinaryOperator("||");
        public readonly static BinaryOperator BitAnd = new BinaryOperator("&", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator BitOr = new BinaryOperator("|", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator Xor = new BinaryOperator("^", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator ShLeft = new BinaryOperator("<<", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator ShRight = new BinaryOperator(">>", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator UnShRight = new BinaryOperator(">>>", OperatorRequirement.NumericOnly);
        public readonly static BinaryOperator Min = new BinaryOperator("Math.min", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall);
        public readonly static BinaryOperator Max = new BinaryOperator("Math.max", OperatorRequirement.NumericOnly | OperatorRequirement.FunctionCall);


        private BinaryOperator(string symbol, OperatorRequirement flag = OperatorRequirement.None)
            : base(symbol, flag)
        {

        }
    }
}
