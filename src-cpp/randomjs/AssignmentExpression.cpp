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

#include "AssignmentExpression.h"
#include "ExpressionType.h"
#include "OperatorRequirement.h"
#include "GlobalFunction.h"

AssignmentExpression::AssignmentExpression(AssignmentOperator& oper, Variable* v) : oper(oper), variable(v) {}

EnumType AssignmentExpression::getType() {
	return ExpressionType::AssignmentExpression;
}

bool AssignmentExpression::isNumeric() {
	return oper.has(OperatorRequirement::NumericOnly);
}

void AssignmentExpression::writeTo(std::ostream& os) const {
	if (!variable->isLoopCounter() && oper.has(OperatorRequirement::StringLengthLimit)) {
		// ((var += expr), var = __strl(var))
		os << "((" << *variable << oper << *rhs << "),"
		   << *variable << AssignmentOperator::Basic << GlobalFunction::STRL.getName()
		   << "(" << *variable << "))";
	}
	else if (variable->isLoopCounter() || !oper.has(OperatorRequirement::NumericOnly)) {
		os << "(";
		if (oper.has(OperatorRequirement::Prefix)) {
			os << oper << *variable;
		}
		else {
			os << *variable << oper;
		}
		if (!oper.has(OperatorRequirement::WithoutRhs))
			os << *rhs;
		os << ")";
	}
	else
	{
		os << GlobalFunction::CALC.getName() << "(" << *variable << ",()=>(";
		if (oper.has(OperatorRequirement::WithoutRhs))
		{
			if (oper.has(OperatorRequirement::Prefix)) {
				os << oper << *variable;
			}
			else {
				os << *variable << oper;
			}
		}
		else
		{
			os << *variable << oper << *rhs;
		}
		os << ")," << *defaultValue << ")";
	}
}