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

#include "Enum.h"

struct StatementType {
	static constexpr EnumType ReturnStatement = 1 << 0;
	static constexpr EnumType BreakStatement = 1 << 1;
	static constexpr EnumType AssignmentStatement = 1 << 2;
	static constexpr EnumType ObjectSetStatement = 1 << 3;
	static constexpr EnumType IfElseStatement = 1 << 4;
	static constexpr EnumType ForLoopStatement = 1 << 5;
	//static constexpr EnumType WhileLoop; TODO
	//static constexpr EnumType DoWhileLoop; TODO
	static constexpr EnumType BlockStatement = 1 << 6;
	static constexpr EnumType VariableInvocationStatement = 1 << 7;
	static constexpr EnumType ThrowStatement = 1 << 8;

	static constexpr EnumType None = 0;
	static constexpr EnumType All = ~None;

	static constexpr EnumType Flat = ReturnStatement | BreakStatement | AssignmentStatement | ObjectSetStatement | VariableInvocationStatement | ThrowStatement;
	static constexpr EnumType Terminating = ReturnStatement | BreakStatement | ThrowStatement;
	static constexpr EnumType NoCall = All & ~VariableInvocationStatement;
};