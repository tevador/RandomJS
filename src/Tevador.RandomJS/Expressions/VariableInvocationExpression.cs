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

using System;
using System.Collections.Generic;

namespace Tevador.RandomJS.Expressions
{
    class VariableInvocationExpression : Expression
    {
        public List<Expression> Parameters = new List<Expression>();
        protected IVariable _variable;

        protected VariableInvocationExpression(IVariable variable, Expression parent)
            : base(parent)
        {
            if(variable == null) throw new ArgumentNullException();
            _variable = variable;
        }

        public VariableInvocationExpression(IScope scope, IVariable variable, Expression parent)
            : this(variable, parent)
        {
            scope.Require(GlobalFunction.INVK);
        }

        public override void WriteTo(System.IO.TextWriter w)
        {
            w.Write(GlobalFunction.INVK);
            w.Write("(");
            w.Write(_variable.Name);
            foreach (var expr in Parameters)
            {
                w.Write(", ");
                expr.WriteTo(w);
            }
            w.Write(")");
        }
    }
}
