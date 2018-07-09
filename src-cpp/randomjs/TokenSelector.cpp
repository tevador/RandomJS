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

#include "TokenSelector.h"
#include "LiteralType.h"
#include "ExpressionType.h"
#include "AssignmentOperator.h"	
#include "NumericLiteralType.h"
#include "UnaryOperator.h"
#include "BinaryOperator.h"
#include "StatementType.h"

#define SELECTOR_REGISTER(item)										\
		total += TDistribution::item;


#define SELECTOR_PROBE(typeResult, item)							\
	if (TDistribution::item > 0) {									\
		probe += TDistribution::item;								\
		if (pivot < probe)											\
			return typeResult::item;								\
	}


#define SELECTOR_REGISTER_LIST(typeSource, typeResult, item)		\
	if (typeSource::item > 0 && (typeResult::item & list))			\
		total += typeSource::item;


#define SELECTOR_PROBE_LIST(typeSource, typeResult, item)			\
	if (typeSource::item > 0 && (typeResult::item & list)) {		\
		probe += typeSource::item;									\
		if (pivot < probe)											\
			return typeResult::item;								\
	}


template<typename TDistribution>
class OperatorSelector<AssignmentOperator, TDistribution> {
public:
	static AssignmentOperator& select(RandomGenerator&);
protected:
	static constexpr TableType getTotal();
};

template<typename TDistribution>
class OperatorSelector<UnaryOperator, TDistribution> {
public:
	static UnaryOperator& select(RandomGenerator&);
protected:
	static constexpr TableType getTotal();
};

template<typename TDistribution>
class OperatorSelector<BinaryOperator, TDistribution> {
public:
	static BinaryOperator& select(RandomGenerator&);
protected:
	static constexpr TableType getTotal();
};

template class OperatorSelector<AssignmentOperator, ProgramOptions::AssignmentOperators>;
template class OperatorSelector<AssignmentOperator, ProgramOptions::AssignmentInForLoop>;
template class OperatorSelector<UnaryOperator, ProgramOptions::UnaryOperators>;
template class OperatorSelector<BinaryOperator, ProgramOptions::BinaryOperators>;

template<typename TDistribution>
AssignmentOperator& OperatorSelector<AssignmentOperator, TDistribution>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(AssignmentOperator, Basic)
	SELECTOR_PROBE(AssignmentOperator, Add)
	SELECTOR_PROBE(AssignmentOperator, Sub)
	SELECTOR_PROBE(AssignmentOperator, Mul)
	SELECTOR_PROBE(AssignmentOperator, Div)
	SELECTOR_PROBE(AssignmentOperator, Mod)
	SELECTOR_PROBE(AssignmentOperator, PreInc)
	SELECTOR_PROBE(AssignmentOperator, PostInc)
	SELECTOR_PROBE(AssignmentOperator, PreDec)
	//SELECTOR_PROBE(AssignmentOperator, PostDec)

	return AssignmentOperator::PostDec;
}

template<typename TDistribution>
constexpr TableType OperatorSelector<AssignmentOperator, TDistribution>::getTotal() {
	int32_t total = 0;

	SELECTOR_REGISTER(Basic)
	SELECTOR_REGISTER(Add)
	SELECTOR_REGISTER(Sub)
	SELECTOR_REGISTER(Mul)
	SELECTOR_REGISTER(Div)
	SELECTOR_REGISTER(Mod)
	SELECTOR_REGISTER(PreInc)
	SELECTOR_REGISTER(PostInc)
	SELECTOR_REGISTER(PreDec)
	SELECTOR_REGISTER(PostDec)

	return total;
}

template<typename TDistribution>
UnaryOperator& OperatorSelector<UnaryOperator, TDistribution>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(UnaryOperator, Not)
	SELECTOR_PROBE(UnaryOperator, Plus)
	SELECTOR_PROBE(UnaryOperator, Typeof)
	SELECTOR_PROBE(UnaryOperator, Minus)
	SELECTOR_PROBE(UnaryOperator, Sqrt)
	SELECTOR_PROBE(UnaryOperator, Abs)
	SELECTOR_PROBE(UnaryOperator, Ceil)
	SELECTOR_PROBE(UnaryOperator, Floor)
	//SELECTOR_PROBE(UnaryOperator, Trunc)

	return UnaryOperator::Trunc;
}

template<typename TDistribution>
constexpr TableType OperatorSelector<UnaryOperator, TDistribution>::getTotal() {
	int32_t total = 0;

	SELECTOR_REGISTER(Not)
	SELECTOR_REGISTER(Plus)
	SELECTOR_REGISTER(Typeof)
	SELECTOR_REGISTER(Minus)
	SELECTOR_REGISTER(Sqrt)
	SELECTOR_REGISTER(Abs)
	SELECTOR_REGISTER(Ceil)
	SELECTOR_REGISTER(Floor)
	SELECTOR_REGISTER(Trunc)

	return total;
}

template<typename TDistribution>
BinaryOperator& OperatorSelector<BinaryOperator, TDistribution>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(BinaryOperator, Add)
	SELECTOR_PROBE(BinaryOperator, Comma)
	SELECTOR_PROBE(BinaryOperator, Sub)
	SELECTOR_PROBE(BinaryOperator, Mul)
	SELECTOR_PROBE(BinaryOperator, Div)
	SELECTOR_PROBE(BinaryOperator, Mod)
	SELECTOR_PROBE(BinaryOperator, Less)
	SELECTOR_PROBE(BinaryOperator, Greater)
	SELECTOR_PROBE(BinaryOperator, Equal)
	SELECTOR_PROBE(BinaryOperator, NotEqual)
	SELECTOR_PROBE(BinaryOperator, And)
	SELECTOR_PROBE(BinaryOperator, Or)
	SELECTOR_PROBE(BinaryOperator, BitAnd)
	SELECTOR_PROBE(BinaryOperator, BitOr)
	SELECTOR_PROBE(BinaryOperator, Xor)
	SELECTOR_PROBE(BinaryOperator, ShLeft)
	SELECTOR_PROBE(BinaryOperator, ShRight)
	SELECTOR_PROBE(BinaryOperator, UnShRight)
	SELECTOR_PROBE(BinaryOperator, Min)
	//SELECTOR_PROBE(BinaryOperator, Max)

	return BinaryOperator::Max;
}

template<typename TDistribution>
constexpr TableType OperatorSelector<BinaryOperator, TDistribution>::getTotal() {
	int32_t total = 0;

	SELECTOR_REGISTER(Add)
	SELECTOR_REGISTER(Comma)
	SELECTOR_REGISTER(Sub)
	SELECTOR_REGISTER(Mul)
	SELECTOR_REGISTER(Div)
	SELECTOR_REGISTER(Mod)
	SELECTOR_REGISTER(Less)
	SELECTOR_REGISTER(Greater)
	SELECTOR_REGISTER(Equal)
	SELECTOR_REGISTER(NotEqual)
	SELECTOR_REGISTER(And)
	SELECTOR_REGISTER(Or)
	SELECTOR_REGISTER(BitAnd)
	SELECTOR_REGISTER(BitOr)
	SELECTOR_REGISTER(Xor)
	SELECTOR_REGISTER(ShLeft)
	SELECTOR_REGISTER(ShRight)
	SELECTOR_REGISTER(UnShRight)
	SELECTOR_REGISTER(Min)
	SELECTOR_REGISTER(Max)

	return total;
}

template<>
EnumType EnumSelector<NumericLiteralType>::select(RandomGenerator& rand, EnumType list) {
	return rand.genInt(NumericLiteralType::Count);
}

template<>
EnumType EnumSelector<LiteralType>::select(RandomGenerator& rand, EnumType list) {
	int32_t total = 0;

	SELECTOR_REGISTER_LIST(ProgramOptions::Literals, LiteralType, String)
	SELECTOR_REGISTER_LIST(ProgramOptions::Literals, LiteralType, Numeric)
	SELECTOR_REGISTER_LIST(ProgramOptions::Literals, LiteralType, Object)

	int32_t pivot = rand.genInt(total);
	int32_t probe = 0;

	SELECTOR_PROBE_LIST(ProgramOptions::Literals, LiteralType, String)
	SELECTOR_PROBE_LIST(ProgramOptions::Literals, LiteralType, Numeric)
	//SELECTOR_PROBE_LIST(ProgramOptions::Literals, LiteralType, Object)

	return LiteralType::Object;
}

template<>
EnumType EnumSelector<ExpressionType>::select(RandomGenerator& rand, EnumType list) {
	int32_t total = 0;

	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, AssignmentExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, BinaryExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, FunctionExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, Literal)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, TernaryExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, UnaryExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, VariableInvocationExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, FunctionInvocationExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, VariableExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, EvalExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, ObjectSetExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ExpressionType, ObjectConstructorExpression)

	int32_t pivot = rand.genInt(total);
	int32_t probe = 0;

	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, AssignmentExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, BinaryExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, FunctionExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, Literal)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, TernaryExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, UnaryExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, VariableInvocationExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, FunctionInvocationExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, VariableExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, EvalExpression)
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, ObjectSetExpression)
	//SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, ObjectConstructorExpression)

	return ExpressionType::ObjectConstructorExpression;
}

template<>
EnumType EnumSelector<StatementType>::select(RandomGenerator& rand, EnumType list) {
	int32_t total = 0;

	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, ReturnStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, BreakStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, AssignmentStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, ObjectSetStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, IfElseStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, ForLoopStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, BlockStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, VariableInvocationStatement)
	SELECTOR_REGISTER_LIST(ProgramOptions::Statements, StatementType, ThrowStatement)

	int32_t pivot = rand.genInt(total);
	int32_t probe = 0;

	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, ReturnStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, BreakStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, AssignmentStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, ObjectSetStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, IfElseStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, ForLoopStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, BlockStatement)
	SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, VariableInvocationStatement)
	//SELECTOR_PROBE_LIST(ProgramOptions::Statements, StatementType, ThrowStatement)

	return StatementType::ThrowStatement;
}