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

namespace Tevador.RandomJS
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
            }
        }

        protected List<Statement> _statements = new List<Statement>();
        protected List<Variable> _declaredVariables = new List<Variable>();

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
                foreach (var v in _declaredVariables)
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
            if (Parent != null && (Parent is FunctionExpression || _statements.Count > 1 || _declaredVariables.Count > 0))
            {
                w.Write("{");
                hasBrackets = true;
            }

            foreach (var v in _declaredVariables)
            {
                v.Declaration.WriteTo(w);
            }

            if (!hasBrackets && _statements.Count == 0)
            {
                w.Write(";");
            }
            foreach (var s in _statements)
            {
                s.WriteTo(w);
            }

            if (hasBrackets)
                w.Write("}");
        }


        public int VariableCounter
        {
            get;
            set;
        }

        public virtual int StatementDepth
        {
            get;
            private set;
        }


        public virtual void Require(Global gf)
        {
            if (Parent != null)
            {
                Parent.Require(gf);
            }
        }


        public virtual ProgramOptions Options
        {
            get { return Parent.Options; }
        }

        public static Block Generate(IRandom rand, IScope scope)
        {
            var block = new Block(scope);
            block.StatementDepth = scope.StatementDepth + 1;
            var func = scope as FunctionExpression;
            if (func != null)
            {
                if (func.Options.DepthProtection != null)
                {
                    block._statements.Add(func.Options.DepthProtection.Check);
                    block._statements.Add(new ReturnStatement(block, func.DefaultReturnValue));
                }
                block._statements.Add(new ReturnStatement(block, func.MakeReturn(rand)));
            }
            else
            {
                Variable v = rand.ChooseVariable(scope, true);
                if (v != null)
                {
                    block._statements.Add(new AssignmentStatement(AssignmentExpression.Generate(rand, block, v, null)));
                }
            }

            return block;
        }
    }
}
