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
    class AssignmentExpression : Expression
    {
        public VariableExpression Variable { get; private set; }
        public AssignmentOperator Operator { get; private set; }
        public NumericLiteral DefaultValue { get; private set; }
        public Expression Rhs { get; private set; }

        public override void WriteTo(System.IO.TextWriter w)
        {
            if (Operator.Has(OperatorRequirement.NumericOnly))
            {
                w.Write(GlobalFunction.CALC);
                w.Write("(");
                Variable.WriteTo(w);
                w.Write(", function() { ");
            }
            w.Write("(");
            if (Operator.Has(OperatorRequirement.Prefix))
            {
                w.Write(Operator);
                Variable.WriteTo(w);
            }
            else
            {
                Variable.WriteTo(w);
                w.Write(Operator); 
            }
            Rhs?.WriteTo(w);
            w.Write(")");
            if (Operator.Has(OperatorRequirement.NumericOnly))
            {
                w.Write("; return ");
                Variable.WriteTo(w);
                w.Write("; }, ");
                DefaultValue.WriteTo(w);
                w.Write(")");
            }
        }

        public static AssignmentExpression Generate(IRandom rand, IScope scope, Variable v, Expression parent, bool isReturn = false)
        {
            AssignmentOperator op = scope.Options.AssignmentOperators.ChooseRandom(rand);
            AssignmentExpression ae = new AssignmentExpression();
            ae.ParentExpression = parent;
            ae.Operator = op;
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                scope.Require(GlobalFunction.CALC);
                ae.DefaultValue = NumericLiteral.Generate(rand, scope);
            }
            if (!op.Has(OperatorRequirement.WithoutRhs))
            {
                Expression expr = Expression.Generate(rand, scope, ae, isReturn);
                if (op.Has(OperatorRequirement.RhsNonzero))
                {
                    expr = new NonZeroExpression(scope, expr);
                }
                ae.Rhs = expr;
            }
            ae.Variable = new VariableExpression(v);
            return ae;
        }
    }
}
