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

#define SELECTOR_REGISTER(type, item)								\
		total += type::item;

#define SELECTOR_PROBE(typeSource, typeResult, item)				\
	if (typeSource::item > 0) {										\
		probe += typeSource::item;									\
		if (pivot < probe)											\
			return typeResult::item;								\
	}						

#define SELECTOR_REGISTER_LIST(type, item)							\
	if (type::item & list)											\
		total += type::item;

#define SELECTOR_PROBE_LIST(typeSource, typeResult, item)			\
	if ((typeResult::item & list) && typeSource::item > 0) {		\
		probe += typeSource::item;									\
		if (pivot < probe)											\
			return typeResult::item;								\
	}

template<>
EnumType EnumSelector<LiteralType>::select(RandomGenerator& rand, EnumType list) {
	int32_t total = 0;

	SELECTOR_REGISTER_LIST(ProgramOptions::Literals, String)
	SELECTOR_REGISTER_LIST(ProgramOptions::Literals, Numeric)
	SELECTOR_REGISTER_LIST(ProgramOptions::Literals, Object)

	int32_t pivot = rand.genInt(total);
	int32_t probe = 0;

	SELECTOR_PROBE_LIST(ProgramOptions::Literals, LiteralType, String)
	SELECTOR_PROBE_LIST(ProgramOptions::Literals, LiteralType, Numeric)
	SELECTOR_PROBE_LIST(ProgramOptions::Literals, LiteralType, Object)

	return LiteralType::None;
}

template<>
EnumType EnumSelector<ExpressionType>::select(RandomGenerator& rand, EnumType list) {
	int32_t total = 0;

	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, AssignmentExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, BinaryExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, FunctionExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, Literal)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, TernaryExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, UnaryExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, VariableInvocationExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, FunctionInvocationExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, VariableExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, EvalExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ObjectSetExpression)
	SELECTOR_REGISTER_LIST(ProgramOptions::Expressions, ObjectConstructorExpression)

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
	SELECTOR_PROBE_LIST(ProgramOptions::Expressions, ExpressionType, ObjectConstructorExpression)

	return ExpressionType::None;
}

template<>
AssignmentOperator& OperatorSelector<AssignmentOperator>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Basic)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Add)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Sub)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Mul)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Div)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Mod)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PreInc)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PostInc)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PreDec)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PostDec)
}

template<>
constexpr TableType OperatorSelector<AssignmentOperator>::getTotal() {
	int32_t total = 0;

	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, Basic)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, Add)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, Sub)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, Mul)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, Div)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, Mod)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, PreInc)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, PostInc)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, PreDec)
	SELECTOR_REGISTER(ProgramOptions::AssignmentOperators, PostDec)

	return total;
}

template<>
EnumType EnumSelector<NumericLiteralType>::select(RandomGenerator& rand, EnumType list) {
	return rand.genInt(NumericLiteralType::Count);
}

template<>
UnaryOperator& OperatorSelector<UnaryOperator>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Not)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Plus)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Typeof)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Minus)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Sqrt)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Abs)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Ceil)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Floor)
	SELECTOR_PROBE(ProgramOptions::UnaryOperators, UnaryOperator, Trunc)
}

template<>
constexpr TableType OperatorSelector<UnaryOperator>::getTotal() {
	int32_t total = 0;

	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Not)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Plus)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Typeof)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Minus)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Sqrt)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Abs)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Ceil)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Floor)
	SELECTOR_REGISTER(ProgramOptions::UnaryOperators, Trunc)

	return total;
}

template<>
BinaryOperator& OperatorSelector<BinaryOperator>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Add)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Comma)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Sub)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Mul)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Div)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Mod)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Less)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Greater)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Equal)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, NotEqual)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, And)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Or)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, BitAnd)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, BitOr)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Xor)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, ShLeft)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, ShRight)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, UnShRight)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Min)
	SELECTOR_PROBE(ProgramOptions::BinaryOperators, BinaryOperator, Max)
}

template<>
constexpr TableType OperatorSelector<BinaryOperator>::getTotal() {
	int32_t total = 0;

	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Add)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Comma)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Sub)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Mul)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Div)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Mod)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Less)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Greater)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Equal)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, NotEqual)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, And)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Or)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, BitAnd)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, BitOr)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Xor)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, ShLeft)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, ShRight)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, UnShRight)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Min)
	SELECTOR_REGISTER(ProgramOptions::BinaryOperators, Max)

	return total;
}