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
        public readonly static GlobalOverride OTST = new GlobalOverride("Object.prototype.toString", "{{return {0}(()=>JSON.stringify(this),'[Object]');}}", TRYC);
        public readonly static GlobalOverride OVOF = new GlobalOverride("Object.prototype.valueOf", "{for(const _ in this)if(typeof this[_]==='number') return this[_];return this;}");
        public readonly static GlobalOverride FTST = new GlobalOverride("Function.prototype.toString", "{return '[Function]'+this.name;}");
        public readonly static GlobalOverride FVOF = new GlobalOverride("Function.prototype.valueOf", "{{if(!this.name){{const _='_fvof';(_ in this)||(this[_]={0}(this));if(typeof this[_]!=='function')return this[_];}}return this.toString();}}", INVC);
        public readonly static GlobalOverride RTST = new GlobalOverride(GlobalClass.RERR + ".prototype.toString", "{{return this.constructor.name+this.name;}}", GlobalClass.RERR);
        public readonly static GlobalOverride RVOF = new GlobalOverride(GlobalClass.RERR + ".prototype.valueOf", "{{return this.name;}}", GlobalClass.RERR);

        public GlobalOverride(string name, string declaration, Global references = null)
            : base(name, declaration, references)
        {
        }

        public override void WriteTo(TextWriter w)
        {
            w.Write(Name);
            w.Write(AssignmentOperator.Basic);
            w.Write("function()");
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
