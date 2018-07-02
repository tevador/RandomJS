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

#include "Block.h"
#include <algorithm>

Block::Block(IScope* parent) : IScope(parent) {
	if (parent != nullptr) {
		statements.reserve(std::max(ProgramOptions::BlockStatementsMax, ProgramOptions::FunctionStatementsMax) + ProgramOptions::LocalVariablesCountMax + 1);
	}
	else {
		statements.reserve(Global::count + 2 * ProgramOptions::GlobalVariablesCountMax + 3);
	}
}

void Block::writeTo(std::ostream& os) const {
	os << "{";
	writeStatements(os);
	os << "}";
}

void Block::writeStatements(std::ostream& os) const {
	for (auto stmt : statements) {
		os << *stmt;
	}
}