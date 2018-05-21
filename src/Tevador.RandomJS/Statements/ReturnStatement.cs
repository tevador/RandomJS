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
using Tevador.RandomJS.Expressions;

namespace Tevador.RandomJS.Statements
{
    class ReturnStatement : Block
    {
        public ReturnStatement(IScope scope, Expression value, CallDepthProtection depthProtection)
            : base(scope)
        {
            if (scope.FunctionDepth == 0) throw new InvalidOperationException("Return statement must be inside a function");
            if (depthProtection != null)
                Statements.Add(depthProtection.Cleanup);
            Statements.Add(new _Return(value));
        }

        class _Return : Statement
        {
            Expression _value;

            public _Return(Expression value)
            {
                _value = value;
            }

            public override void WriteTo(System.IO.TextWriter w)
            {
                w.Write("return");
                if (_value != null)
                {
                    w.Write(" ");
                    _value.WriteTo(w);
                }
                w.Write(";");
            }
        }

        public override bool IsTerminating
        {
            get { return true; }
        }
    }
}
