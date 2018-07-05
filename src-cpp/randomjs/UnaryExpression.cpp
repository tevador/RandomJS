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

#include "UnaryExpression.h"
#include "ExpressionType.h"

UnaryExpression::UnaryExpression(UnaryOperator& oper, Expression* expr) : oper(oper), expr(expr) {}


bool UnaryExpression::isNumeric() {
	return oper.has(OperatorRequirement::NumericOnly);
}

void UnaryExpression::writeTo(std::ostream& os) const {
	if (oper.has(OperatorRequirement::FunctionCall)) {
		os << oper << "(" << *expr << ")";
	}
	else {
		os << "(" << oper << *expr << ")";
	}
}

uint32_t UnaryExpression::getType() {
	return ExpressionType::UnaryExpression;
}