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

#include "BinaryExpression.h"
#include "GlobalFunction.h"

BinaryExpression::BinaryExpression(BinaryOperator* oper, Expression* lhs, Expression* rhs) : oper(oper), lhs(lhs), rhs(rhs) {}

void BinaryExpression::writeTo(std::ostream& os) const {
	if (oper->has(OperatorRequirement::FunctionCall)) {
		os << *oper << "(" << *lhs << "," << *rhs << ")";
	}
	else {
		if (oper->has(OperatorRequirement::StringLengthLimit)) {
			os << GlobalFunction::STRL.getName();
		}
		os << "(" << *lhs << *oper << *rhs << ")";
	}
}

bool BinaryExpression::isNumeric() {
	return oper->has(OperatorRequirement::NumericOnly);
}