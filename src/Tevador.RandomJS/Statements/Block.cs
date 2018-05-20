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
using Tevador.RandomJS.Expressions;

namespace Tevador.RandomJS.Statements
{
    class Block : Statement, IScope
    {
        public Block(IScope parent)
        {
            Parent = parent;
            if (Parent != null)
            {
                VariableCounter = Parent.VariableCounter;
                StatementDepth = Parent.StatementDepth + 1;
                InFunc = parent.InFunc;
                HasBreak = parent.HasBreak;
            }
        }

        public readonly List<Statement> Statements = new List<Statement>();
        public readonly List<Variable> DeclaredVariables = new List<Variable>();

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
                foreach (var v in DeclaredVariables)
                {
                    yield return v;
                }
            }
        }

        public IScope Parent
        {
            get;
            private set;
        }

        public virtual bool InFunc { get; private set; }

        public override void WriteTo(System.IO.TextWriter w)
        {
            bool hasBrackets = false;
            if (Parent != null && (Parent is FunctionExpression || Statements.Count > 1 || DeclaredVariables.Count > 0))
            {
                w.Write("{");
                hasBrackets = true;
            }
            if (!hasBrackets && Statements.Count == 0)
            {
                w.Write(";");
            }
            else
            {
                foreach (var s in Statements)
                {
                    s.WriteTo(w);
                }
            }
            if (hasBrackets)
                w.Write("}");
        }


        public int VariableCounter
        {
            get;
            set;
        }

        public virtual void Require(Global gf)
        {
            if (Parent != null)
            {
                Parent.Require(gf);
            }
        }

        public virtual bool HasBreak
        {
            get;
            private set;
        }
    }
}
