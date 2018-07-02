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

#include <cstdint>

using TableType = int32_t;

struct ProgramOptions {
	static constexpr int ObjectSetPropertyCount = 4;
	static constexpr int VariableInitializerDepth = 1;
	static constexpr int MaxFunctionDepth = 1;
	static constexpr int MaxObjectLiteralDepth = 1;
	static constexpr bool EnableCallDepthProtection = true;
	static constexpr bool EnableLoopCyclesProtection = true;
	static constexpr int MaxExpressionDepth = 3;
	static constexpr int MaxStatementDepth = 2;
	static constexpr double ConstVariableChance = 0.1;
	static constexpr double ElseChance = 0.4;
	static constexpr bool FunctionsAreConstants = true;
	static constexpr int VariableSelectorScopeFactor = 2;
	static constexpr double ForLoopVariableBoundsChance = 0.35;
	static constexpr bool AllowFunctionInvocationInLoop = false;
	static constexpr bool FunctionValueOfOverride = true;
	static constexpr double CatchChance = 0.4;
	static constexpr double ObjectLiteralVariableChance = 0.5;

	static constexpr int GlobalVariablesCountMin = 12;
	static constexpr int GlobalVariablesCountMax = 12;

	static constexpr int LocalVariablesCountMin = 0;
	static constexpr int LocalVariablesCountMax = 3;

	static constexpr int BlockStatementsMin = 2;
	static constexpr int BlockStatementsMax = 4;

	static constexpr int FunctionStatementsMin = 5;
	static constexpr int FunctionStatementsMax = 7;

	static constexpr int FunctionParametersCountMin = 1;
	static constexpr int FunctionParametersCountMax = 3;

	static constexpr int StringLiteralLengthMin = 0;
	static constexpr int StringLiteralLengthMax = 10;

	static constexpr int MaxCallDepthMin = 3;
	static constexpr int MaxCallDepthMax = 3;

	static constexpr int MaxLoopCyclesMin = 1000;
	static constexpr int MaxLoopCyclesMax = 2000;

	static constexpr int StringLengthMin = 35;
	static constexpr int StringLengthMax = 50;

	static constexpr int ObjectLiteralSizeMin = 0;
	static constexpr int ObjectLiteralSizeMax = 4;

	static constexpr int EvalStringLengthMin = 10;
	static constexpr int EvalStringLengthMax = 10;

	struct AssignmentOperators {
		static constexpr TableType Basic = 6;
		static constexpr TableType Add = 4;
		static constexpr TableType Sub = 1;
		static constexpr TableType Mul = 1;
		static constexpr TableType Div = 1;
		static constexpr TableType Mod = 1;
		static constexpr TableType PreInc = 2;
		static constexpr TableType PreDec = 2;
		static constexpr TableType PostInc = 2;
		static constexpr TableType PostDec = 2;
	};

	struct AssignmentInForLoop {
		static constexpr TableType Add = 3;
		static constexpr TableType Sub = 3;
		static constexpr TableType Mul = 3;
		static constexpr TableType Div = 3;
		static constexpr TableType PostInc = 2;
		static constexpr TableType PostDec = 2;
	};

	struct UnaryOperators {
		static constexpr TableType Plus = 1;
		static constexpr TableType Typeof = 1;
		static constexpr TableType Minus = 1;
		static constexpr TableType Not = 1;
		static constexpr TableType Sqrt = 1;
		static constexpr TableType Floor = 1;
		static constexpr TableType Ceil = 1;
		static constexpr TableType Abs = 1;
		static constexpr TableType Trunc = 1;
	};

	struct BinaryOperators {
		static constexpr TableType Add = 8;
		static constexpr TableType Comma = 1;
		static constexpr TableType Sub = 2;
		static constexpr TableType Mul = 2;
		static constexpr TableType Div = 2;
		static constexpr TableType Mod = 2;
		static constexpr TableType Less = 2;
		static constexpr TableType Greater = 2;
		static constexpr TableType Equal = 2;
		static constexpr TableType NotEqual = 2;
		static constexpr TableType Min = 1;
		static constexpr TableType Max = 1;
		static constexpr TableType BitAnd = 1;
		static constexpr TableType BitOr = 1;
		static constexpr TableType Xor = 1;
	};

	struct Literals {
		static constexpr TableType String = 2;
		static constexpr TableType Numeric = 4;
		static constexpr TableType Object = 2;
	};

	struct NumericLiterals {
		static constexpr TableType Boolean = 3;
		static constexpr TableType DecimalInteger = 3;
		static constexpr TableType BinaryInteger = 1;
		static constexpr TableType OctalInteger = 3;
		static constexpr TableType SmallInteger = 3;
		static constexpr TableType HexInteger = 3;
		static constexpr TableType FixedFloat = 3;
		static constexpr TableType ExpFloat = 3;
	};

	struct Expressions {
		static constexpr TableType AssignmentExpression = 4;
		static constexpr TableType BinaryExpression = 5;
		static constexpr TableType FunctionExpression = 6;
		static constexpr TableType Literal = 2;
		static constexpr TableType TernaryExpression = 1;
		static constexpr TableType UnaryExpression = 2;
		static constexpr TableType VariableInvocationExpression = 3;
		static constexpr TableType FunctionInvocationExpression = 1;
		static constexpr TableType VariableExpression = 2;
		static constexpr TableType EvalExpression = 2;
		static constexpr TableType ObjectSetExpression = 1;
		static constexpr TableType ObjectConstructorExpression = 1;
	};

	struct Statements {
		static constexpr TableType ReturnStatement = 2;
		static constexpr TableType BreakStatement = 1;
		static constexpr TableType AssignmentStatement = 9;
		static constexpr TableType IfElseStatement = 8;
		static constexpr TableType ForLoopStatement = 2;
		static constexpr TableType BlockStatement = 6;
		static constexpr TableType VariableInvocationStatement = 4;
		static constexpr TableType ObjectSetStatement = 2;
		static constexpr TableType ThrowStatement = 1;
	};
};