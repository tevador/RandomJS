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

#include <cstdint>

struct ExpressionType {
	static constexpr uint32_t Literal = 1 << 0;
	static constexpr uint32_t AssignmentExpression = 1 << 1;
	static constexpr uint32_t VariableInvocationExpression = 1 << 2;
	static constexpr uint32_t FunctionInvocationExpression = 1 << 3;
	static constexpr uint32_t FunctionExpression = 1 << 4;
	static constexpr uint32_t UnaryExpression = 1 << 5;
	static constexpr uint32_t BinaryExpression = 1 << 6;
	static constexpr uint32_t TernaryExpression = 1 << 7;
	static constexpr uint32_t EvalExpression = 1 << 8;
	static constexpr uint32_t VariableExpression = 1 << 9;
	static constexpr uint32_t ObjectConstructorExpression = 1 << 10;
	static constexpr uint32_t ObjectSetExpression = 1 << 11;

	static constexpr uint32_t None = 0;
	static constexpr uint32_t All = ~None;
	static constexpr uint32_t Function = FunctionExpression | FunctionInvocationExpression;
	static constexpr uint32_t NoVariable = Literal | FunctionInvocationExpression | FunctionExpression | EvalExpression | ObjectConstructorExpression;
	static constexpr uint32_t NoCall = Literal | AssignmentExpression | UnaryExpression | BinaryExpression | TernaryExpression | VariableExpression | ObjectSetExpression;
	static constexpr uint32_t Flat = Literal | VariableExpression | EvalExpression;
};