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

namespace Tevador.RandomJS.Statements
{
    abstract class BreakableStatement : Statement, IScope
    {
        protected BreakableStatement(IScope parent)
        {
            Parent = parent;
            if (Parent != null)
            {
                VariableCounter = Parent.VariableCounter;
                FunctionDepth = parent.FunctionDepth;
            }
        }

        public virtual IEnumerable<Variable> Variables
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
            }
        }

        public IScope Parent
        {
            get;
            private set;
        }

        public virtual int FunctionDepth { get; private set; }

        public bool HasBreak
        {
            get { return true; }
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
    }
}
