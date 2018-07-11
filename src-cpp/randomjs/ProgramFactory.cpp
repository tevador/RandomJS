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
#include "ReturnStatement.h"
#include "NonEmptyExpression.h"
#include "EmptyStatement.h"
#include "BreakStatement.h"
#include "ExpressionStatement.h"
#include "GlobalClass.h"
#include "InvalidOperationException.h"

//TODO
CodeStatement ProgramFactory::callDepthCheck = CodeStatement("if(++__depth>__maxDepth)");
CodeStatement ProgramFactory::incrementCallSum = CodeStatement("__callSum++;");
CodeStatement ProgramFactory::incrementCallSumDepth = CodeStatement("__callSum+=__depth--;");
CodeStatement ProgramFactory::loopCyclesCheck = CodeStatement("(__cycles++<__maxCycles)");
CodeStatement ProgramFactory::catchReturn = CodeStatement("return __error;");

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
	while (p->getVariableCounter() < globalsCount) {
		Variable* v = genVariable(p);
		p->addStatement(v->getDeclaration());
	}
	List<Variable*> printOrder(p->begin(), p->end());
	RandomUtility::shuffle(rand, printOrder);
	for (Variable* v : printOrder) {
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
	auto name = Variable::getVariableName(scope->getVariableCounter());
	scope->incrementCounter();
	isConstant = isConstant || (!isParameter && !isLoopCounter && rand.flipCoin(ProgramOptions::ConstVariableChance));
	Expression* init = nullptr;
	if (initialize && !isParameter && !isLoopCounter) {
		init = genExpression(scope, ProgramOptions::VariableInitializerDepth);
		isConstant = isConstant || (ProgramOptions::FunctionsAreConstants && init->getType() == ExpressionType::FunctionExpression);
	}
	Variable* v = new Variable(scope, name, isConstant, isLoopCounter, init);
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
	auto& oper = OperatorSelector<AssignmentOperator, ProgramOptions::AssignmentOperators>::select(rand);
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
	auto& op = OperatorSelector<UnaryOperator, ProgramOptions::UnaryOperators>::select(rand);
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
	auto& op = OperatorSelector<BinaryOperator, ProgramOptions::BinaryOperators>::select(rand);
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
		throw InvalidOperationException("Unable to create a constructor for ObjectCreateExpression");
	}
	auto oce = new ObjectConstructorExpression(constructor);
	genVariableInvocationExpression(oce, scope, maxDepth);
	return oce;
}

NumericLiteral* ProgramFactory::genNumericLiteral() {
	return genNumericLiteral(EnumSelector<NumericLiteralType>::select(rand, 0));
}

NumericLiteral* ProgramFactory::genNumericLiteral(EnumType type) {
	String* str = new (LinearAllocator::getInstance().allocate(sizeof(String))) String();
	str->reserve(37);
	bool negative = false;
	if (type != NumericLiteralType::Boolean && rand.flipCoin()) {
		str->append("(-");
		negative = true;
	}
	switch (type) {
		case NumericLiteralType::Boolean:
			if (rand.flipCoin())
				str->append("true");
			else
				str->append("false");
			break;

		case NumericLiteralType::SmallInteger:
			RandomUtility::genString(rand, str, 2, RandomUtility::decimalChars, false);
			break;

		case NumericLiteralType::BinaryInteger:
			str->append("0b");
			RandomUtility::genString(rand, str, 32, RandomUtility::binaryChars);
			break;

		case NumericLiteralType::DecimalInteger:
			RandomUtility::genString(rand, str, 9, RandomUtility::decimalChars, false);
			break;

		case NumericLiteralType::OctalInteger:
			str->append("0o");
			RandomUtility::genString(rand, str, 10, RandomUtility::octalChars);
			break;

		case NumericLiteralType::HexInteger:
			str->append("0x");
			RandomUtility::genString(rand, str, 8, RandomUtility::hexChars);
			break;

		case NumericLiteralType::FixedFloat:
			RandomUtility::genString(rand, str, 5, RandomUtility::decimalChars, false);
			str->push_back('.');
			RandomUtility::genString(rand, str, 5, RandomUtility::decimalChars);
			break;

		case NumericLiteralType::ExpFloat:
			str->push_back(RandomUtility::decimalChars[rand.genInt(10)]);
			str->push_back('.');
			RandomUtility::genString(rand, str, 5, RandomUtility::decimalChars);
			str->push_back('e');
			if (rand.flipCoin()) str->push_back('-');
			RandomUtility::genString(rand, str, 2, RandomUtility::decimalChars);
			break;
	}
	if (negative) str->push_back(')');
	return new NumericLiteral(str->data());
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

FunctionBody* ProgramFactory::genFunctionBody(FunctionExpression* parent) {
	parent->require(&GlobalVariable::CSUM);
	auto tryBody = new Block(parent);
	auto funcBody = new FunctionBody();
	funcBody->setTry(tryBody);
	if (ProgramOptions::EnableCallDepthProtection) {
		tryBody->addStatement(&callDepthCheck);
		tryBody->addStatement(new ReturnStatement(genSafeReturnExpression(parent)));
		funcBody->setFinally(&incrementCallSumDepth);
	}
	else {
		funcBody->setFinally(&incrementCallSum);
	}
	if (rand.flipCoin(ProgramOptions::CatchChance)) {
		funcBody->setCatch(&catchReturn);
	}
	else {
		funcBody->setCatch(nullptr);
	}
	auto that = genVariable(tryBody, false, false, true, false);
	that->setInitializer(new VariableExpression(&Variable::This));
	tryBody->addStatement(that->getDeclaration());
	int localVariablesCount = genValueFromInterval(ProgramOptions::LocalVariablesCountMin, ProgramOptions::LocalVariablesCountMax);
	while (localVariablesCount-- > 0) {
		auto v = genVariable(tryBody);
		tryBody->addStatement(v->getDeclaration());
	}
	genBlock(tryBody, ProgramOptions::FunctionStatementsMin, ProgramOptions::FunctionStatementsMax, ProgramOptions::MaxStatementDepth, StatementType::All & ~StatementType::Terminating);
	if (tryBody->getStatementCount() == 0 || !tryBody->getLastStatement()->isTerminating()) {
		tryBody->addStatement(new ReturnStatement(genExpression(tryBody, ProgramOptions::MaxExpressionDepth)));
	}
	return funcBody;
}

FunctionExpression* ProgramFactory::genFunctionExpression(IScope* scope) {
	scope->require(&GlobalOverride::FTST);
	if (ProgramOptions::FunctionValueOfOverride)
		scope->require(&GlobalOverride::FVOF);
	auto func = new FunctionExpression(scope);
	int paramCount = genValueFromInterval(ProgramOptions::FunctionParametersCountMin, ProgramOptions::FunctionParametersCountMax);
	for (int i = 0; i < paramCount; ++i) {
		func->addParameter(genVariable(func, true));
	}
	func->setBody(genFunctionBody(func));
	return func;
}

Expression* ProgramFactory::genSafeReturnExpression(IScope* scope) {
	auto expr = genExpression(scope, 0, ExpressionType::VariableExpression | ExpressionType::Literal);
	if (expr->getType() != ExpressionType::Literal) {
		expr = new NonEmptyExpression(expr, genLiteral(scope));
	}
	return expr;
}

Block* ProgramFactory::genBlock(Block* block, int minStatements, int maxStatements, int maxDepth, EnumType list) {
	list &= ~StatementType::BlockStatement;
	int statementsCount = genValueFromInterval(minStatements, maxStatements);
	while (statementsCount-- > 0) {
		auto stmt = genStatement(block, maxDepth, list);
		block->addStatement(stmt);
		if (stmt->isTerminating())
			break;
	}
	return block;
}

Block* ProgramFactory::genBlock(IScope* scope, int maxDepth) {
	auto block = new Block(scope);
	return genBlock(block, ProgramOptions::BlockStatementsMin, ProgramOptions::BlockStatementsMax, maxDepth);
}

Statement* ProgramFactory::genStatement(IScope* scope, int maxDepth, EnumType list) {
	if (maxDepth <= 0) {
		list &= StatementType::Flat;
	}
	if (scope->getVariableCounter() == 0) {
		list &= ~StatementType::AssignmentStatement;
	}
	if (!scope->hasBreak()) {
		list &= ~StatementType::BreakStatement;
	}
	else if (!ProgramOptions::AllowFunctionInvocationInLoop) {
		list &= StatementType::NoCall;
	}
	if (scope->getFunctionDepth() == 0) {
		list &= ~StatementType::ReturnStatement;
	}

	auto type = EnumSelector<StatementType>::select(rand, list);
	Variable* v;
	switch (type)
	{
		case StatementType::AssignmentStatement:
			if ((v = variableSelector.selectVariable(rand, scope, true)) != nullptr) {
				return new ExpressionStatement(genAssignmentExpression(scope, v, ProgramOptions::MaxExpressionDepth));
			}
			break;

		case StatementType::BreakStatement:
			return new BreakStatement();

		case StatementType::ObjectSetStatement:
			return new ExpressionStatement(genObjectSetExpression(scope, ProgramOptions::MaxExpressionDepth));

		case StatementType::ReturnStatement:
			return new ReturnStatement(genExpression(scope, ProgramOptions::MaxExpressionDepth));

		case StatementType::IfElseStatement:
			return genIfElseStatement(scope, maxDepth - 1);

		case StatementType::VariableInvocationStatement:
			if ((v = variableSelector.selectVariable(rand, scope)) != nullptr) {
				auto expr = genVariableInvocationExpression(scope, v, ProgramOptions::MaxExpressionDepth);
				return new ExpressionStatement(expr);
			}
			break;

		case StatementType::BlockStatement:
			return genBlock(scope, maxDepth - 1);

		case StatementType::ForLoopStatement:
			return genForLoopStatement(scope, maxDepth - 1);

		case StatementType::ThrowStatement:
			return genThrowStatement(scope);
	}
	return new EmptyStatement();
}

IfElseStatement* ProgramFactory::genIfElseStatement(IScope* scope, int maxDepth) {
	auto condition = genExpression(scope, ProgramOptions::MaxExpressionDepth, ExpressionType::All & ~ExpressionType::Literal);
	auto body = genStatement(scope, maxDepth);
	Statement* elseBody = nullptr;
	if (rand.flipCoin(ProgramOptions::ElseChance)) {
		elseBody = genStatement(scope,  maxDepth);
	}
	return new IfElseStatement(condition, body, elseBody);
}

ThrowStatement* ProgramFactory::genThrowStatement(IScope* scope) {
	scope->require(&GlobalClass::RERR);
	auto value = genExpression(scope, ProgramOptions::ThrowExpressionDepth);
	return new ThrowStatement(value);
}

ForLoopStatement* ProgramFactory::genForLoopStatement(IScope* scope, int maxDepth) {
	auto fl = new ForLoopStatement(scope);
	auto i = genVariable(fl, false, true);
	i->setInitializer(genNumericLiteral(NumericLiteralType::SmallInteger));
	fl->setCounter(i);
	Expression* control = nullptr;
	if (rand.flipCoin(ProgramOptions::ForLoopVariableBoundsChance)) {
		auto v = variableSelector.selectVariable(rand, scope);
		if (v != nullptr)
			control = new NumericExpression(fl, new VariableExpression(v), genNumericLiteral());
	}
	if (control == nullptr) {
		control = genNumericLiteral();
	}
	control = new BinaryExpression(rand.flipCoin() ? BinaryOperator::Less : BinaryOperator::Greater, new VariableExpression(i), control);
	fl->setControl(new LoopControlExpression(control, &loopCyclesCheck));
	auto& oper = OperatorSelector<AssignmentOperator, ProgramOptions::AssignmentInForLoop>::select(rand);
	Expression* rhs = nullptr;
	if (!oper.has(OperatorRequirement::WithoutRhs)) {
		rhs = genNumericLiteral();
		if (oper.has(OperatorRequirement::RhsNonzero)) {
			rhs = new NonZeroExpression(fl, rhs);
		}
	}
	auto iterator = new AssignmentExpression(oper, i);
	iterator->setRightHandSide(rhs);
	fl->setIterator(iterator);
	fl->setBody(genStatement(fl, maxDepth, StatementType::All & ~StatementType::ReturnStatement & ~StatementType::ThrowStatement));
	return fl;
}