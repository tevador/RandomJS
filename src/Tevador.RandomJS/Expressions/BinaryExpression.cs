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

namespace Tevador.RandomJS.Expressions
{
    class BinaryExpression : Expression
    {
        public BinaryOperator Operator { get; set; }
        public Expression Lhs { get; set; }
        public Expression Rhs { get; set; }

        public override void WriteTo(System.IO.TextWriter w)
        {
            if (Operator.Has(OperatorRequirement.FunctionCall))
            {
                w.Write(Operator);
                w.Write("(");
                Lhs.WriteTo(w);
                w.Write(", ");
                Rhs.WriteTo(w);
                w.Write(")");
            }
            else
            {
                w.Write("(");
                Lhs.WriteTo(w);
                w.Write(Operator);
                Rhs.WriteTo(w);
                w.Write(")");
            }
        }

        public static new BinaryExpression Generate(IRandom rand, IScope scope, Expression parent, bool isReturn)
        {
            BinaryExpression be = new BinaryExpression();
            be.ParentExpression = parent;
            var op = scope.Options.BinaryOperators.ChooseRandom(rand);
            be.Operator = op;
            var lhs = Expression.Generate(rand, scope, be, isReturn);
            var rhs = Expression.Generate(rand, scope, be, isReturn);
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                lhs = new NumericExpression(scope, lhs, NumericLiteral.Generate(rand, scope));
                rhs = new NumericExpression(scope, rhs, NumericLiteral.Generate(rand, scope));
                if (op.Has(OperatorRequirement.RhsNonzero))
                {
                    rhs = new NonZeroExpression(scope, rhs);
                }
            }
            be.Lhs = lhs;
            be.Rhs = rhs;
            return be;
        }
    }
}
