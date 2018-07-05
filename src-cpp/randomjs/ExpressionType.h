/*
(c) 2018 tevador <tevador@gmail.com>

This file is part of RandomJS.

RandomJS is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License; or
(at your option) any later version.

RandomJS is distributed in the hope that it will be useful;
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RandomJS.  If not; see<http://www.gnu.org/licenses/>.
*/

#pragma once

#include "Enum.h"

struct ExpressionType {
	static constexpr EnumType Literal = 1 << 0;
	static constexpr EnumType AssignmentExpression = 1 << 1;
	static constexpr EnumType VariableInvocationExpression = 1 << 2;
	static constexpr EnumType FunctionInvocationExpression = 1 << 3;
	static constexpr EnumType FunctionExpression = 1 << 4;
	static constexpr EnumType UnaryExpression = 1 << 5;
	static constexpr EnumType BinaryExpression = 1 << 6;
	static constexpr EnumType TernaryExpression = 1 << 7;
	static constexpr EnumType EvalExpression = 1 << 8;
	static constexpr EnumType VariableExpression = 1 << 9;
	static constexpr EnumType ObjectConstructorExpression = 1 << 10;
	static constexpr EnumType ObjectSetExpression = 1 << 11;

	static constexpr EnumType None = 0;
	static constexpr EnumType All = ~None;
	static constexpr EnumType Function = FunctionExpression | FunctionInvocationExpression;
	static constexpr EnumType NoVariable = Literal | FunctionInvocationExpression | FunctionExpression | EvalExpression | ObjectConstructorExpression;
	static constexpr EnumType NoCall = Literal | AssignmentExpression | UnaryExpression | BinaryExpression | TernaryExpression | VariableExpression | ObjectSetExpression;
	static constexpr EnumType Flat = Literal | VariableExpression | EvalExpression;
};