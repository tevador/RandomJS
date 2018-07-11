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

#include "Expression.h"
#include "Memory.h"

class IVariable : public AllocatorBase {
public:
	const char* getName() const {
		return name;
	};
	bool isConstant() const {
		return constant;
	}
	Expression* getInitializer() const {
		return initializer;
	}
	void setInitializer(Expression* expr) {
		initializer = expr;
	}
	
protected:
	IVariable(const char* name, bool constant, Expression* initializer) : name(name), initializer(initializer), constant(constant) {}

private:
	const char* name;
	Expression* initializer;
	bool constant;
};