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

#include "ProgramFactory.h"
#include "GlobalFunction.h"
#include "GlobalVariable.h"
#include "RandomUtility.h"
#include "VariableExpression.h"
#include "TokenSelector.h"

//TODO
CodeStatement ProgramFactory::callDepthCheck = CodeStatement("if(++__depth>__maxDepth)");
CodeStatement ProgramFactory::incrementCallSum = CodeStatement("__callSum++;");
CodeStatement ProgramFactory::incrementCallSumDepth = CodeStatement("__callSum+=__depth--;");
CodeStatement ProgramFactory::loopCyclesCheck = CodeStatement("(__cycles++<__maxCycles)");

ProgramFactory::ProgramFactory(RandomGenerator& rand) : rand(rand) {}

Program* ProgramFactory::genProgram(void* seed) {
	LinearAllocator::getInstance().reset();
	variableSelector.init();
	rand.seed(seed);
	Program* p = new Program();
	if (ProgramOptions::EnableCallDepthProtection) {
		p->require(&GlobalVariable::MDPT);
		p->require(&GlobalVariable::DPTH);
	}
	if (ProgramOptions::EnableLoopCyclesProtection) {
		p->require(&GlobalVariable::MCYC);
		p->require(&GlobalVariable::CYCL);
	}
	int32_t globalsCount = genValueFromInterval(ProgramOptions::GlobalVariablesCountMin, ProgramOptions::GlobalVariablesCountMax);
	while (p->getVariableCounter() < globalsCount)
	{
		Variable* v = genVariable(p);
		p->declareVariable(v);
		p->addStatement(v->getDeclaration());
	}
	List<Variable*> printOrder(p->begin(), p->end());
	RandomUtility::shuffle(rand, printOrder);
	for(Variable* v : printOrder)
	{
		p->addStatement(genOutputStatement(p, v));
	}
	if (p->isDefined(&GlobalVariable::ESUM))
		p->addStatement(genOutputStatement(p, new VariableExpression(&GlobalVariable::ESUM)));
	if (p->isDefined(&GlobalVariable::CSUM))
		p->addStatement(genOutputStatement(p, new VariableExpression(&GlobalVariable::CSUM)));
	p->setGlobalVariable(GlobalVariable::STRL, genValueFromInterval(ProgramOptions::StringLengthMin, ProgramOptions::StringLengthMax));
	p->setGlobalVariable(GlobalVariable::MDPT, genValueFromInterval(ProgramOptions::MaxCallDepthMin, ProgramOptions::MaxCallDepthMax));
	p->setGlobalVariable(GlobalVariable::MCYC, genValueFromInterval(ProgramOptions::MaxLoopCyclesMin, ProgramOptions::MaxLoopCyclesMax));
	return p;
}

int32_t ProgramFactory::genValueFromInterval(int32_t min, int32_t max) {
	if (min == max)
		return min;
	return rand.genInt(min, max + 1);
}

Variable* ProgramFactory::genVariable(IScope* scope, bool isParameter, bool isLoopCounter, bool isConstant, bool initialize) {
	Expression* init = nullptr;
	if (initialize && !isParameter && !isLoopCounter)
	{
		init = genExpression(scope, ProgramOptions::VariableInitializerDepth);
		isConstant = isConstant || (ProgramOptions::FunctionsAreConstants && init->getType() == ExpressionType::FunctionExpression);
	}
	auto name = Variable::getVariableName(scope->getVariableCounter());
	bool constant = isConstant || (!isParameter && !isLoopCounter && rand.flipCoin(ProgramOptions::ConstVariableChance));
	Variable* v = new Variable(scope, name, constant, isLoopCounter, init);
	scope->declareVariable(v);
	return v;
}

Expression* ProgramFactory::genExpression(IScope* scope, int maxDepth, uint32_t list) {
	if (maxDepth <= 0) {
		list &= ExpressionType::Flat;
	}
	if (scope->getFunctionDepth() >= ProgramOptions::MaxFunctionDepth) {
		list &= ~ExpressionType::Function;
	}
	if (scope->getVariableCounter() == 0) {
		list &= ExpressionType::NoVariable;
	}
	if (scope->hasBreak() && !ProgramOptions::AllowFunctionInvocationInLoop) {
		list &= ExpressionType::NoCall;
	}

	Variable* v;
	auto type = EnumSelector<ExpressionType>::select(rand, list);

	switch (type) {
		case ExpressionType::Literal:
			return genLiteral(scope);

		case ExpressionType::AssignmentExpression:
			if ((v = variableSelector.selectVariable(rand, scope, true)) != nullptr) {
				return genAssignmentExpression(scope, v, maxDepth - 1);
			}
			break;

		case ExpressionType::VariableInvocationExpression:
			if ((v = variableSelector.selectVariable(rand, scope)) != nullptr) {
				return genVariableInvocationExpression(scope, v, maxDepth - 1);
			}
			break;

		case ExpressionType::VariableExpression:
			if ((v = variableSelector.selectVariable(rand, scope)) != nullptr) {
				return new VariableExpression(v);
			}
			break;

		case ExpressionType::EvalExpression:
			return genEvalExpression(scope);

		case ExpressionType::ObjectSetExpression:
			return genObjectSetExpression(scope, maxDepth - 1);

		case ExpressionType::ObjectConstructorExpression:
			return genObjectConstructorExpression(scope, maxDepth - 1);

		case ExpressionType::FunctionInvocationExpression:
			return genFunctionInvocationExpression(scope, maxDepth - 1);

		case ExpressionType::FunctionExpression:
			return genFunctionExpression(scope);

		case ExpressionType::UnaryExpression:
			return genUnaryExpression(scope, maxDepth - 1);

		case ExpressionType::BinaryExpression:
			return genBinaryExpression(scope, maxDepth - 1);

		case ExpressionType::TernaryExpression:
			return genTernaryExpression(scope, maxDepth - 1);
	}
	return genLiteral(scope); //fall back to a Literal
}