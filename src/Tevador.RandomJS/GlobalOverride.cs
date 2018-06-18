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

using System.IO;
using Tevador.RandomJS.Operators;

namespace Tevador.RandomJS
{
    class GlobalOverride : GlobalFunction
    {
        public readonly static GlobalOverride OTST = new GlobalOverride("Object.prototype.toString", "function() {{return {0}(()=>JSON.stringify(this),'[Object]');}}", TRYC);
        public readonly static GlobalOverride OVOF = new GlobalOverride("Object.prototype.valueOf", "function() { for(let _ in this) if (typeof this[_] === 'number') return this[_]; return this; }");
        public readonly static GlobalOverride FTST = new GlobalOverride("Function.prototype.toString", "function(){return '[Function]'+this.name;}");
        public readonly static GlobalOverride FVOF = new GlobalOverride("Function.prototype.valueOf", "function() {{if(!this.name){{({0} in this)||(this[{0}]=this());if(typeof this[{0}]!=='function')return this[{0}];}}return this.toString();}}", new GlobalVariable("__fvof", true, new Expressions.Literal("'__fvof'")));

        public GlobalOverride(string name, string declaration, Global references = null)
            : base(name, declaration, references)
        {
        }

        public override void WriteTo(TextWriter w)
        {
            w.Write(Name);
            w.Write(AssignmentOperator.Basic);
            if (References != null)
            {
                w.Write(Declaration, References);
            }
            else
            {
                w.Write(Declaration);
            }
            w.Write(";");
        }

        public override Global Clone()
        {
            return this;
        }
    }
}
