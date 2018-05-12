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

using System.Collections.Generic;

namespace Tevador.RandomJS.Expressions
{
    class FunctionInvocationExpression : Expression
    {
        FunctionExpression _func;
        List<Expression> _parameters = new List<Expression>();

        public override void WriteTo(System.IO.TextWriter w)
        {
            w.Write("(");
            _func.WriteTo(w);
            w.Write(")(");
            using (var enumerator = _parameters.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    bool isLast;
                    do
                    {
                        var param = enumerator.Current;
                        isLast = !enumerator.MoveNext();

                        param.WriteTo(w);
                        if (!isLast)
                            w.Write(", ");
                    }
                    while (!isLast);
                }
            }
            w.Write(")");
        }

        public static FunctionInvocationExpression Generate(IRandom rand, IScope scope, Expression parent)
        {
            FunctionInvocationExpression fi = new FunctionInvocationExpression();
            fi.ParentExpression = parent;
            fi._func = FunctionExpression.Generate(rand, scope, fi);
            int paramCount = rand.GenInt(1, scope.Options.MaxFunctionParameterCount);
            while (paramCount-- > 0)
            {
                fi._parameters.Add(Expression.Generate(rand, scope, fi, false));
            }
            return fi;
        }
    }
}
