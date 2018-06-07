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

namespace Tevador.RandomJS
{
    class GlobalFunction : Global
    {
        public readonly static GlobalFunction NONZ = new GlobalFunction("__nonz", "(_) { return _ == 0 ? 1 : _; }");
        public readonly static GlobalFunction CALC = new GlobalFunction("__calc", "(_,_f,_d) { if(typeof _ === 'number') return _f(_); return _d; }");
        public readonly static GlobalFunction STRL = new GlobalFunction("__strl", "(_) {{ if(typeof _ === 'string' && _.length > {0}) return _.substring(0, {0}); return _; }}", new GlobalVariable("__maxStrlen", true));
        public readonly static GlobalFunction TSTR = new GlobalFunction("__tstr", "(_){{return _!=null?{0}(_.toString()):_;}}", STRL);
        public readonly static GlobalFunction INVK = new GlobalFunction("__invk", "(_,...__) {{ if(typeof _ === 'function') return {0}(_(...__)); else return {0}(_); }}", STRL);
        public readonly static GlobalFunction PRNT = new GlobalFunction("__prnt", "(_){{console.log({0}(_));}}", TSTR);
        public readonly static GlobalFunction NUMB = new GlobalFunction("__numb", "(_,__) { _=+_; if(!isNaN(_)) return _; else return __; }");
        public readonly static GlobalFunction PREC = new GlobalFunction("__prec", "(_) {{ return +_.toPrecision({0}); }}", new GlobalVariable("__fpMathPrec", true));
        public readonly static GlobalFunction NNEG = new GlobalFunction("__nneg", "(_) { return _ < 0 ? -_ : _; }");
        public readonly static GlobalFunction TRYC = new GlobalFunction("__tryc", "(_,__){try {return _();}catch(_e){return _e.name+__;}}");
        public readonly static GlobalFunction OBJC = new GlobalFunction("__objc", "(_,...__){ if(typeof _ === 'function') return new _(...__); if(typeof _ === 'object') return _; return { a: _ }; }");
        public readonly static GlobalFunction OBJS = new GlobalFunction("__objs", "(_,_k,_v){ if(Object.isExtensible(_)) _[_k]=_v; return _||_v; }");
        public readonly static GlobalFunction EVAL = new GlobalFunction("__eval", "(_f,_s){{return {0}(()=>_f(_s),_s);}}", TRYC);

        public string Declaration { get; protected set; }

        public GlobalFunction(string name, string declaration, Global references = null)
        {
            Name = name;
            Declaration = declaration;
            References = references;
        }

        public override void WriteTo(System.IO.TextWriter w)
        {
            w.Write("function ");
            w.Write(Name);
            if (References != null)
            {
                w.Write(Declaration, References.Name);
            }
            else
            {
                w.Write(Declaration);
            }
        }

        public override Global Clone()
        {
            return this;
        }
    }
}
