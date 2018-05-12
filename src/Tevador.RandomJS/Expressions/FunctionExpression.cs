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
    class FunctionExpression : Expression, IScope
    {
        public FunctionExpression(IScope parent)
        {
            Parent = parent;
            VariableCounter = Parent.VariableCounter;
            StatementDepth = Parent.StatementDepth; //increased in Body
        }

        /* public string Name { get; private set; } anonymous */
        private List<Variable> _parameters = new List<Variable>();
        private List<Variable> _unusedVariables = new List<Variable>(); // TODO
        public Expression DefaultReturnValue { get; private set; }
        public Block Body { get; private set; }

        public IEnumerable<Variable> Variables
        {
            get
            {
                if (Parent != null)
                {
                    foreach (var v in Parent.Variables)
                    {
                        yield return v;
                    }
                }
                foreach (var v in _parameters)
                {
                    yield return v;
                }
            }
        }

        public Expression MakeReturn(IRandom rand)
        {
            return new NonEmptyExpression(Expression.Generate(rand, this, null, true), rand, this);
        }

        public IScope Parent
        {
            get;
            private set;
        }

        public override void WriteTo(System.IO.TextWriter w)
        {
            w.Write("function (");
            using (var enumerator = _parameters.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    bool isLast;
                    do
                    {
                        var param = enumerator.Current;
                        isLast = !enumerator.MoveNext();

                        w.Write(param.Name);
                        if (!isLast)
                            w.Write(", ");
                    }
                    while (!isLast);
                }
            }
            w.Write(")");
            Body.WriteTo(w);
        }


        public int VariableCounter
        {
            get;
            set;
        }

        public int StatementDepth
        {
            get;
            private set;
        }


        public void Require(Global gf)
        {
            Parent.Require(gf);
        }


        public ProgramOptions Options
        {
            get { return Parent.Options; }
        }

        public static FunctionExpression Generate(IRandom rand, IScope scope, Expression parent)
        {
            var func = new FunctionExpression(scope);
            //func.ExpressionDepth = scope.StatementDepth + 1;
            int paramCount = rand.GenInt(1, scope.Options.MaxFunctionParameterCount);
            for (int i = 0; i < paramCount; ++i)
            {
                func._parameters.Add(Variable.Generate(rand, func, true, false));
            }
            func._unusedVariables.AddRange(func._parameters);
            func.DefaultReturnValue = func.MakeReturn(rand);
            func.Body = Block.Generate(rand, func);
            return func;
        }


        public bool InFunc
        {
            get { return true; }
        }
    }
}
