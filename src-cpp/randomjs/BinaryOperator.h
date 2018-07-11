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

#include "Operator.h"
#include "OperatorRequirement.h"

class BinaryOperator : public Operator {
public:
	static BinaryOperator Add;
	static BinaryOperator Comma;
	static BinaryOperator Sub;
	static BinaryOperator Mul;
	static BinaryOperator Div;
	static BinaryOperator Mod;
	static BinaryOperator Less;
	static BinaryOperator Greater;
	static BinaryOperator Equal;
	static BinaryOperator NotEqual;
	static BinaryOperator And;
	static BinaryOperator Or;
	static BinaryOperator BitAnd;
	static BinaryOperator BitOr;
	static BinaryOperator Xor;
	static BinaryOperator ShLeft;
	static BinaryOperator ShRight;
	static BinaryOperator UnShRight;
	static BinaryOperator Min;
	static BinaryOperator Max;
private:
	BinaryOperator(const char* symbol, EnumType flags = OperatorRequirement::None) : Operator(symbol, flags) {}
};