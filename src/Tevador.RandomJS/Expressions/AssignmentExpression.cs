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
        public AssignmentExpression(Expression parent)
            :base(parent)
        { }

        public Variable Variable { get; set; }
        public AssignmentOperator Operator { get; set; }
        public NumericLiteral DefaultValue { get; set; }
        public Expression Rhs { get; set; }

        public override void WriteTo(System.IO.TextWriter w)
        {
            if (!Variable.IsLoopCounter && Operator.Has(OperatorRequirement.StringLengthLimit))
            {
                // ((var += expr), var = __strl(var))
                w.Write("((");
                w.Write(Variable);
                w.Write(Operator);
                Rhs.WriteTo(w);
                w.Write("),");
                w.Write(Variable);
                w.Write(AssignmentOperator.Basic);
                w.Write(GlobalFunction.STRL);
                w.Write("(");
                w.Write(Variable);
                w.Write("))");
            }
            else if (Variable.IsLoopCounter || !Operator.Has(OperatorRequirement.NumericOnly))
            {
                w.Write("(");
                if (Operator.Has(OperatorRequirement.Prefix))
                {
                    w.Write(Operator);
                    w.Write(Variable);
                }
                else
                {
                    w.Write(Variable);
                    w.Write(Operator);
                }
                Rhs?.WriteTo(w);
                w.Write(")");
            }
            else
            {
                w.Write(GlobalFunction.CALC);
                w.Write("(");
                w.Write(Variable);
                w.Write(", function() { return (");
                if (Operator.Has(OperatorRequirement.WithoutRhs))
                {
                    if (Operator.Has(OperatorRequirement.Prefix))
                    {
                        w.Write(Operator);
                        w.Write(Variable);
                    }
                    else
                    {
                        w.Write(Variable);
                        w.Write(Operator);
                    }
                }
                else
                {
                    w.Write(Variable);
                    w.Write(Operator);
                    Rhs.WriteTo(w);
                }
                w.Write("); }, ");
                DefaultValue.WriteTo(w);
                w.Write(")");
            }
        }

        public override bool IsNumeric
        {
            get { return Operator.Has(OperatorRequirement.NumericOnly); }
        }
    }
}
