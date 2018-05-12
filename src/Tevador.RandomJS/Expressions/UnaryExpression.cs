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
    class UnaryExpression : Expression
    {
        public UnaryOperator Operator { get; private set; }
        public Expression Value { get; private set; }
        public NumericLiteral DefaultValue { get; private set; }

        public static new UnaryExpression Generate(IRandom rand, IScope scope, Expression parent, bool isReturn)
        {
            UnaryExpression ue = new UnaryExpression();
            ue.ParentExpression = parent;
            UnaryOperator op = scope.Options.UnaryOperators.ChooseRandom(rand);
            ue.Operator = op;
            Expression expr = Expression.Generate(rand, scope, ue, isReturn);
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                expr = new NumericExpression(scope, expr, NumericLiteral.Generate(rand, scope));
            }
            if (op.Has(OperatorRequirement.RhsNonnegative))
            {
                expr = new NonNegativeExpression(scope, expr);
            }
            if (op.Has(OperatorRequirement.RhsNonzero))
            {
                expr = new NonZeroExpression(scope, expr);
            }
            if (op.Has(OperatorRequirement.LimitedPrecision))
            {
                scope.Require(GlobalFunction.PREC);
            }
            ue.Value = expr;
            return ue;
        }

        public override void WriteTo(System.IO.TextWriter w)
        {
            if (Operator.Has(OperatorRequirement.LimitedPrecision))
            {
                w.Write(GlobalFunction.PREC);
                w.Write("(");
            }
            if (Operator.Has(OperatorRequirement.FunctionCall))
            {
                w.Write(Operator);
                w.Write("(");
                Value.WriteTo(w);
                w.Write(")");
            }
            else
            {
                w.Write("(");
                w.Write(Operator);
                Value.WriteTo(w);
                w.Write(")");
            }
            if (Operator.Has(OperatorRequirement.LimitedPrecision))
            {
                w.Write(")");
            }
        }
    }
}
