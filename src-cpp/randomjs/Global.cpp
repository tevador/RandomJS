/*
(c) 2018 tevador <tevador@gmail.com>

This file is part of RandomJS.

RandomJS is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RandomJS is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RandomJS.  If not, see<http://www.gnu.org/licenses/>.
*/

#include "Global.h"
#include "GlobalFunction.h"
#include "GlobalOverride.h"
#include "GlobalClass.h"
#include "GlobalVariable.h"
#include "Literal.h"
#include "NumericLiteral.h"
#include "AssignmentOperator.h"

Literal Literal::Zero = NumericLiteral("0");

GlobalVariable GlobalVariable::CSUM = GlobalVariable(__COUNTER__, "__callSum", false, &Literal::Zero);
GlobalVariable GlobalVariable::STRL = GlobalVariable(__COUNTER__, "__maxStrlen", true);
GlobalVariable GlobalVariable::ESUM = GlobalVariable(__COUNTER__, "__errorSum", false, &Literal::Zero);
GlobalVariable GlobalVariable::MCYC = GlobalVariable(__COUNTER__, "__maxCycles", true);
GlobalVariable GlobalVariable::CYCL = GlobalVariable(__COUNTER__, "__cycles", false, &Literal::Zero);
GlobalVariable GlobalVariable::MDPT = GlobalVariable(__COUNTER__, "__maxDepth", true);
GlobalVariable GlobalVariable::DPTH = GlobalVariable(__COUNTER__, "__depth", false, &Literal::Zero);

GlobalFunction GlobalFunction::NONZ = GlobalFunction(__COUNTER__, "__nonz", "(_) {return _==0?1:_;}");
GlobalFunction GlobalFunction::CALC = GlobalFunction(__COUNTER__, "__calc", "(_,_f,_d){if(typeof _==='number')return _f(_);return _d;}");
GlobalFunction GlobalFunction::STRL = GlobalFunction(__COUNTER__, "__strl", "(_) { if(typeof _==='string'&&_.length>__maxStrlen)return _.substring(0, __maxStrlen);return _;}", &GlobalVariable::STRL);
GlobalFunction GlobalFunction::TSTR = GlobalFunction(__COUNTER__, "__tstr", "(_){return _!=null?__strl(_.toString()):''+_;}", &GlobalFunction::STRL);
GlobalFunction GlobalFunction::INVK = GlobalFunction(__COUNTER__, "__invk", "(_,...__) { if(typeof _==='function')return _(...__);return _; }");
GlobalFunction GlobalFunction::TRYC = GlobalFunction(__COUNTER__, "__tryc", "(_,__){try {return _();}catch(_e){if(!(_e instanceof SyntaxError))__errorSum++;return _e.name+__;}}", &GlobalVariable::ESUM);
GlobalFunction GlobalFunction::INVC = GlobalFunction(__COUNTER__, "__invc", "(_,...__) { if(typeof _ === 'function')return __tryc(()=>_(...__),_.toString());return _; }", &GlobalFunction::TRYC);
GlobalFunction GlobalFunction::NUMB = GlobalFunction(__COUNTER__, "__numb", "(_,__){ _=+_; if(!isNaN(_)) return _; return __; }");
GlobalFunction GlobalFunction::NNEG = GlobalFunction(__COUNTER__, "__nneg", "(_) { return _ < 0 ? -_ : _; }");
GlobalFunction GlobalFunction::OBJC = GlobalFunction(__COUNTER__, "__objc", "(_,...__){if(typeof _ === 'function') return __tryc(()=>new _(...__),_.toString()); if(typeof _ === 'object') return _; return { a: _ }; }", &GlobalFunction::TRYC);
GlobalFunction GlobalFunction::OBJS = GlobalFunction(__COUNTER__, "__objs", "(_,_k,_v) { if(Object.isExtensible(_)) _[_k]=_v; return _||_v; }");
GlobalFunction GlobalFunction::EVAL = GlobalFunction(__COUNTER__, "__eval", "(_f,_s){return __tryc(()=>_f(_s),_s);}", &GlobalFunction::TRYC);
GlobalFunction GlobalFunction::OBJD = GlobalFunction(__COUNTER__, "__objd", "(_) {for(const __ in _)if(typeof _[__]==='object')return false;return true;}");
GlobalFunction GlobalFunction::OBJL = GlobalFunction(__COUNTER__, "__objl", "(_){if(typeof _!== 'object'||__objd(_))return _;}", &GlobalFunction::OBJD);
GlobalFunction GlobalFunction::PRNT = GlobalFunction(__COUNTER__, "__prnt", "(_) {print(__tstr(_));}", &GlobalFunction::TSTR);

GlobalOverride GlobalOverride::OTST = GlobalOverride(__COUNTER__, "Object.prototype.toString", "{return __tryc(()=>JSON.stringify(this),'[Object]');}", &GlobalFunction::TRYC);
GlobalOverride GlobalOverride::OVOF = GlobalOverride(__COUNTER__, "Object.prototype.valueOf", "{for(const _ in this)if(typeof this[_]==='number') return this[_];return this;}");
GlobalOverride GlobalOverride::FTST = GlobalOverride(__COUNTER__, "Function.prototype.toString", "{return '[Function]'+this.name;}");
GlobalOverride GlobalOverride::FVOF = GlobalOverride(__COUNTER__, "Function.prototype.valueOf", "{if(!this.name){const _='_fvof';(_ in this)||(this[_]=__invc(this));if(typeof this[_]!=='function')return this[_];}return this.toString();}", &GlobalFunction::INVC);

GlobalClass GlobalClass::RERR = GlobalClass(__COUNTER__, "RandomError", " extends Error{constructor(_){super(_);this.name=_;};toString(){return this.constructor.name+this.name;};valueOf(){return this.name;}}");

constexpr uint32_t finalCount = __COUNTER__;

static_assert(Global::count == finalCount, "The value of Global::count doesn't match the number of defined globals");

void GlobalClass::writeTo(std::ostream& os) const {
	os << "class ";
	os << getName();
	os << getDeclaration();
}

void GlobalFunction::writeTo(std::ostream& os) const {
	os << "function ";
	os << getName();
	os << declaration;
}

void GlobalOverride::writeTo(std::ostream& os) const {
	os << getName();
	os << AssignmentOperator::Basic;
	os << "function()";
	os << getDeclaration();
	os << ";";
}