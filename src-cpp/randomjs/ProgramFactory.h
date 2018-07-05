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

#include "RandomGenerator.h"
#include "Program.h"
#include "CodeStatement.h"
#include "Variable.h"
#include "OutputStatement.h"
#include "ExpressionType.h"
#include "VariableSelector.h"
#include "Literal.h"
#include "ObjectLiteral.h"
#include "LiteralType.h"
#include "StatementType.h"
#include "AssignmentExpression.h"
#include "EvalExpression.h"
#include "UnaryExpression.h"
#include "BinaryExpression.h"
#include "TernaryExpression.h"
#include "VariableInvocationExpression.h"
#include "FunctionInvocationExpression.h"
#include "ObjectSetExpression.h"
#include "FunctionExpression.h"
#include "ObjectConstructorExpression.h"
#include "FunctionExpression.h"
#include <cstdint>

class ProgramFactory
{
public:
	ProgramFactory(RandomGenerator&);
	Program* genProgram(void* seed);

private:
	RandomGenerator& rand;
	VariableSelector variableSelector;
	static CodeStatement callDepthCheck;
	static CodeStatement incrementCallSum;
	static CodeStatement incrementCallSumDepth;
	static CodeStatement loopCyclesCheck;

	int32_t genValueFromInterval(int32_t, int32_t);
	Variable* genVariable(IScope* scope, bool isParameter = false, bool isLoopCounter = false, bool isConstant = false, bool initialize = true);
	OutputStatement* genOutputStatement(Program* program, Expression* expr);
	OutputStatement* genOutputStatement(Program* program, IVariable* v);
	Expression* genExpression(IScope* scope, int maxDepth, EnumType list = ExpressionType::All);
	Literal* genLiteral(IScope* scope, int maxDepth, EnumType list = LiteralType::All);
	Literal* genLiteral(IScope* scope);
	AssignmentExpression* genAssignmentExpression(IScope* scope, Variable* v, int maxDepth);
	EvalExpression* genEvalExpression(IScope* scope);
	UnaryExpression* genUnaryExpression(IScope* scope, int maxDepth);
	BinaryExpression* genBinaryExpression(IScope* scope, int maxDepth);
	TernaryExpression* genTernaryExpression(IScope* scope, int maxDepth);
	VariableInvocationExpression* genVariableInvocationExpression(IScope* scope, IVariable* v, int maxDepth);
	VariableInvocationExpression* genVariableInvocationExpression(VariableInvocationExpression* invk, IScope* scope, int maxDepth);
	FunctionInvocationExpression* genFunctionInvocationExpression(IScope* scope, int maxDepth);
	ObjectSetExpression* genObjectSetExpression(IScope* scope, int maxDepth);
	ObjectConstructorExpression* genObjectConstructorExpression(IScope* scope, int maxDepth);
	FunctionExpression* genFunctionExpression(IScope* scope);
	NumericLiteral* genNumericLiteral();
	NumericLiteral* genNumericLiteral(EnumType type);
	ObjectLiteral* genObjectLiteral(IScope* scope, int maxDepth);

	Statement* genStatement(IScope* scope, int maxDepth, EnumType list = StatementType::All);
};

