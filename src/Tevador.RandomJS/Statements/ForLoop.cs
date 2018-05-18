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
using System.IO;

namespace Tevador.RandomJS.Statements
{
    class ForLoop : Loop
    {
        public ForLoop(IScope scope)
            : base(scope)
        { }

        public Variable IteratorVariable { get; set; }
        public Expressions.Expression IteratorExpression { get; set; }
        public Statement Body { get; set; }

        public override IEnumerable<Variable> Variables
        {
            get
            {
                if(Parent != null)
                {
                    foreach (var v in Parent.Variables)
                    {
                        yield return v;
                    }
                }
                //yield return IteratorVariable;
            }
        }

        public override void WriteTo(TextWriter w)
        {
            w.Write("for(");
            IteratorVariable.Declaration.WriteTo(w);
            Control.WriteTo(w);
            w.Write(";");
            IteratorExpression.WriteTo(w);
            w.Write(")");
            Body.WriteTo(w);
        }
    }
}
