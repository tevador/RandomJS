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

#pragma once

#include "Global.h"

class GlobalFunction : public Global {
public:
	static GlobalFunction NONZ;
	static GlobalFunction CALC;
	static GlobalFunction STRL;
	static GlobalFunction TSTR;
	static GlobalFunction INVK;
	static GlobalFunction TRYC;
	static GlobalFunction INVC;
	static GlobalFunction NUMB;
	static GlobalFunction NNEG;
	static GlobalFunction OBJC;
	static GlobalFunction OBJS;
	static GlobalFunction EVAL;
	static GlobalFunction OBJD;
	static GlobalFunction OBJL;
	static GlobalFunction PRNT;

	virtual Global* clone() {
		return this;
	}

protected:
	GlobalFunction(uint32_t index, const char* name, const char* declaration, Global* reference = nullptr)
		: Global(index, name, reference), declaration(declaration) {}

	void writeTo(std::ostream& os) const override;

	const char* getDeclaration() const {
		return declaration;
	}

private:
	const char* declaration;
};