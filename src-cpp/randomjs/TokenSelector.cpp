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
TableType EnumSelector<LiteralType>::select(RandomGenerator& rand, uint32_t list) {
	int32_t total = 0.0;

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
AssignmentOperator& OperatorSelector<AssignmentOperator>::select(RandomGenerator& rand) {
	int32_t pivot = rand.genInt(getTotal());
	int32_t probe = 0;

	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Basic)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Add)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Sub)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Mul)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, Div)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PreInc)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PostInc)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PreDec)
	SELECTOR_PROBE(ProgramOptions::AssignmentOperators, AssignmentOperator, PostDec)
}

template<>
constexpr TableType OperatorSelector<AssignmentOperator>::getTotal() {
	int32_t total = 0.0;

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