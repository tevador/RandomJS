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

#include "Program.h"
#include "GlobalVariable.h"
#include "Literal.h"

Program::Program() : Block(nullptr) {
	globals.reserve(Global::count);
}

bool Program::isDefined(Global* global) {
	return definedGlobals[global->getIndex()] != nullptr;
}

void Program::require(Global* global) {
	if (!isDefined(global)) {
		global = global->clone();
		definedGlobals[global->getIndex()] = global;
		if (global->getReference() != nullptr) {
			require(global->getReference());
		}
		globals.push_back(global);
	}
}

void Program::writeTo(std::ostream& os) const {
	os << "'use strict';";
	Block::writeTo(os);
}

void Program::writeStatements(std::ostream& os) const {
	for (auto stmt : globals) {
		os << *stmt;
	}
	for (auto stmt : statements) {
		os << *stmt;
	}
}

template<typename T>
void Program::setGlobalVariable(GlobalVariable& gVar, T value) {
	Global* g;
	if ((g = definedGlobals[gVar.getIndex()]) != nullptr) {
		GlobalVariable* gv = (GlobalVariable*)g;
		gv->setInitializer(new Literal(value));
	}
}
