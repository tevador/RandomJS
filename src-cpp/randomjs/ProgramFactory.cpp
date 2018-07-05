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
#include "GlobalOverride.h"
#include "RandomUtility.h"
#include "VariableExpression.h"
#include "TokenSelector.h"
#include "NumericExpression.h"
#include "NonZeroExpression.h"
#include "NonNegativeExpression.h"
#include "NumericLiteralType.h"
#include "ShallowExpression.h"

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

Expression* ProgramFactory::genExpression(IScope* scope, int maxDepth, EnumType list) {
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

OutputStatement* ProgramFactory::genOutputStatement(Program* program, Expression* expr) {
	program->require(&GlobalFunction::PRNT);
	return new OutputStatement(expr);
}

OutputStatement* ProgramFactory::genOutputStatement(Program* program, IVariable* v) {
	return genOutputStatement(program, genVariableInvocationExpression(program, v, ProgramOptions::MaxExpressionDepth));
}

Literal* ProgramFactory::genLiteral(IScope* scope, int maxDepth, EnumType list) {
	if (maxDepth <= 0)
		list &= ~LiteralType::Object;
	switch (EnumSelector<LiteralType>::select(rand, list)) {
		case LiteralType::Numeric:
			return genNumericLiteral();

		case LiteralType::Object:
			return genObjectLiteral(scope, maxDepth);

		default:
			int stringLength = genValueFromInterval(ProgramOptions::StringLiteralLengthMin, ProgramOptions::StringLiteralLengthMax);
			return new Literal(RandomUtility::genStringLiteral(rand, stringLength));
	}
}

Literal* ProgramFactory::genLiteral(IScope* scope) {
	return genLiteral(scope, ProgramOptions::MaxObjectLiteralDepth);
}

AssignmentExpression* ProgramFactory::genAssignmentExpression(IScope* scope, Variable* v, int maxDepth) {
	auto oper = OperatorSelector<AssignmentOperator>::select(rand);
	AssignmentExpression* ae = new AssignmentExpression(oper, v);
	if (oper.has(OperatorRequirement::NumericOnly)) {
		scope->require(&GlobalFunction::CALC);
		ae->setDefaultValue(genNumericLiteral());
	}
	if (!oper.has(OperatorRequirement::WithoutRhs)) {
		Expression* expr = genExpression(scope, maxDepth);
		if (oper.has(OperatorRequirement::NumericOnly) && !expr->isNumeric())
		{
			expr = new NumericExpression(scope, expr, genNumericLiteral());
		}
		if (oper.has(OperatorRequirement::RhsNonzero))
		{
			expr = new NonZeroExpression(scope, expr);
		}
		ae->setRightHandSide(expr);
	}
	return ae;
}

EvalExpression* ProgramFactory::genEvalExpression(IScope* scope) {
	scope->require(&GlobalFunction::EVAL);
	return new EvalExpression(RandomUtility::genEvalString(rand, genValueFromInterval(ProgramOptions::EvalStringLengthMin, ProgramOptions::EvalStringLengthMax)));
}

UnaryExpression* ProgramFactory::genUnaryExpression(IScope* scope, int maxDepth) {
	auto op = OperatorSelector<UnaryOperator>::select(rand);
	auto expr = genExpression(scope, maxDepth);
	if (op.has(OperatorRequirement::NumericOnly) && !expr->isNumeric()) {
		expr = new NumericExpression(scope, expr, genNumericLiteral());
	}
	if (op.has(OperatorRequirement::RhsNonnegative)) {
		expr = new NonNegativeExpression(scope, expr);
	}
	if (op.has(OperatorRequirement::RhsNonzero)) {
		expr = new NonZeroExpression(scope, expr);
	}
	return new UnaryExpression(op, expr);
}

BinaryExpression* ProgramFactory::genBinaryExpression(IScope* scope, int maxDepth) {
	auto op = OperatorSelector<BinaryOperator>::select(rand);
	auto lhs = genExpression(scope, maxDepth);
	auto rhs = genExpression(scope, maxDepth);
	if (op.has(OperatorRequirement::StringLengthLimit)) {
		scope->require(&GlobalFunction::STRL);
	}
	if (op.has(OperatorRequirement::NumericOnly)) {
		if (!lhs->isNumeric())
			lhs = new NumericExpression(scope, lhs, genNumericLiteral());
		if (!rhs->isNumeric())
			rhs = new NumericExpression(scope, rhs, genNumericLiteral());
		if (op.has(OperatorRequirement::RhsNonzero))
			rhs = new NonZeroExpression(scope, rhs);
	}
	return new BinaryExpression(op, lhs, rhs);
}

TernaryExpression* ProgramFactory::genTernaryExpression(IScope* scope, int maxDepth) {
	auto condition = genExpression(scope, maxDepth, ExpressionType::All & ~ExpressionType::Literal);
	auto trueExpr = genExpression(scope, maxDepth, ExpressionType::All & ~ExpressionType::FunctionExpression);
	auto falseExpr = genExpression(scope, maxDepth, ExpressionType::All & ~ExpressionType::FunctionExpression);
	return new TernaryExpression(condition, trueExpr, falseExpr);
}

VariableInvocationExpression* ProgramFactory::genVariableInvocationExpression(IScope* scope, IVariable* v, int maxDepth) {
	auto invk = new VariableInvocationExpression(v);
	return genVariableInvocationExpression(invk, scope, maxDepth);
}

VariableInvocationExpression* ProgramFactory::genVariableInvocationExpression(VariableInvocationExpression* invk, IScope* scope, int maxDepth) {
	if (invk->getInvokeFunction() == nullptr) {
		auto func = scope->getFunctionDepth() > 0 ? &GlobalFunction::INVK : &GlobalFunction::INVC;
		invk->setInvokeFunction(func);
		scope->require(func);
	}
	int paramCount = genValueFromInterval(ProgramOptions::FunctionParametersCountMin, ProgramOptions::FunctionParametersCountMax);
	while (paramCount-- > 0) {
		invk->addParameter(genExpression(scope, maxDepth, ExpressionType::All & ~ExpressionType::FunctionExpression));
	}
	return invk;
}

FunctionInvocationExpression* ProgramFactory::genFunctionInvocationExpression(IScope* scope, int maxDepth) {
	auto func = genFunctionExpression(scope);
	auto fi = new FunctionInvocationExpression(func);
	genVariableInvocationExpression(fi, scope, maxDepth);
	return fi;
}

ObjectSetExpression* ProgramFactory::genObjectSetExpression(IScope* scope, int maxDepth) {
	scope->require(&GlobalFunction::OBJS);
	auto v = variableSelector.selectVariable(rand, scope);
	auto target = v != nullptr ? (Expression*)new VariableExpression(v) : (Expression*)genObjectLiteral(scope, ProgramOptions::MaxObjectLiteralDepth);
	auto property = Variable::getVariableName(rand.genInt(ProgramOptions::ObjectSetPropertyCount));
	auto value = genExpression(scope, maxDepth - 1);
	return new ObjectSetExpression(target, property, value);
}

ObjectConstructorExpression* ProgramFactory::genObjectConstructorExpression(IScope* scope, int maxDepth) {
	scope->require(&GlobalFunction::OBJC);
	scope->require(&GlobalOverride::OVOF);
	scope->require(&GlobalOverride::OTST);
	Expression* constructor;
	auto v = variableSelector.selectVariable(rand, scope);
	if (v != nullptr) {
		constructor = new VariableExpression(v);
	}
	else if (scope->getFunctionDepth() < ProgramOptions::MaxFunctionDepth) {
		constructor = genFunctionExpression(scope);
	}
	else {
		throw std::exception("Unable to create a constructor for ObjectCreateExpression");
	}
	auto oce = new ObjectConstructorExpression(constructor);
	genVariableInvocationExpression(oce, scope, maxDepth);
	return oce;
}

NumericLiteral* ProgramFactory::genNumericLiteral() {
	return genNumericLiteral(EnumSelector<NumericLiteralType>::select(rand, 0));
}

NumericLiteral* ProgramFactory::genNumericLiteral(EnumType type) {
	StringBuilder* sb = new StringBuilder(); //TODO reseve 37
	bool negative = false;
	if (type != NumericLiteralType::Boolean && rand.flipCoin()) {
		*sb << "(-";
		negative = true;
	}
	switch (type) {
		case NumericLiteralType::Boolean:
			if (rand.flipCoin())
				*sb << "true";
			else
				*sb << "false";
			break;

		case NumericLiteralType::SmallInteger:
			RandomUtility::genString(rand, *sb, 2, RandomUtility::decimalChars, false);
			break;

		case NumericLiteralType::BinaryInteger:
			*sb << "0b";
			RandomUtility::genString(rand, *sb, 32, RandomUtility::binaryChars);
			break;

		case NumericLiteralType::DecimalInteger:
			RandomUtility::genString(rand, *sb, 9, RandomUtility::decimalChars, false);
			break;

		case NumericLiteralType::OctalInteger:
			*sb << "0o";
			RandomUtility::genString(rand, *sb, 10, RandomUtility::octalChars);
			break;

		case NumericLiteralType::HexInteger:
			*sb << "0x";
			RandomUtility::genString(rand, *sb, 8, RandomUtility::hexChars);
			break;

		case NumericLiteralType::FixedFloat:
			RandomUtility::genString(rand, *sb, 5, RandomUtility::decimalChars, false);
			*sb << '.';
			RandomUtility::genString(rand, *sb, 5, RandomUtility::decimalChars);
			break;

		case NumericLiteralType::ExpFloat:
			*sb << RandomUtility::decimalChars[rand.genInt(10)];
			*sb << '.';
			RandomUtility::genString(rand, *sb, 5, RandomUtility::decimalChars);
			*sb << 'e';
			if (rand.flipCoin()) *sb << '-';
			RandomUtility::genString(rand, *sb, 2, RandomUtility::decimalChars);
			break;
	}
	if (negative) *sb << ")";
	return new NumericLiteral(sb->str().data());
}

ObjectLiteral* ProgramFactory::genObjectLiteral(IScope* scope, int maxDepth) {
	scope->require(&GlobalOverride::OVOF);
	scope->require(&GlobalOverride::OTST);
	auto ol = new ObjectLiteral();
	int propertiesCount = genValueFromInterval(ProgramOptions::ObjectLiteralSizeMin, ProgramOptions::ObjectLiteralSizeMax);
	Variable* v;
	while (propertiesCount-- > 0) {
		if (rand.flipCoin(ProgramOptions::ObjectLiteralVariableChance) && (v = variableSelector.selectVariable(rand, scope)) != nullptr) {
			ol->addProperty(new ShallowExpression(scope, new VariableExpression(v)));
		}
		else {
			ol->addProperty(genLiteral(scope, maxDepth - 1));
		}
	}
	return ol;
}