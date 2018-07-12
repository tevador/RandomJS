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

#include "VariableSelector.h"
#include "RandomUtility.h"
#include "ProgramOptions.h"


VariableSelector::VariableSelector() {}

void VariableSelector::init() {
	currentScope = nullptr;
	readableVars = new (LinearAllocator::getInstance().allocate(sizeof(List<Variable*>))) List<Variable*>();
	readableVars->reserve(50);
	writableVars = new (LinearAllocator::getInstance().allocate(sizeof(List<Variable*>))) List<Variable*>();
	writableVars->reserve(50);
}

void VariableSelector::probe(IScope* scope) {
	if (currentScope != scope) {
		currentScope = scope;
		scopeVariableCounter = 0;
		readableVars->clear();
		writableVars->clear();
	}
	for (auto i = scope->begin() + scopeVariableCounter; i != scope->end(); ++i) {
		Variable* v = *i;
		for (uint32_t j = 0; j <= ProgramOptions::VariableSelectorScopeFactor * v->getParent()->getFunctionDepth(); ++j) {
			readableVars->push_back(v);
			if (!v->isLoopCounter() && !v->isConstant())
				writableVars->push_back(v);
		}
		scopeVariableCounter++;
	}
}

Variable* VariableSelector::selectVariable(RandomGenerator& rand, IScope* scope, bool forWriting) {
	probe(scope);
	return RandomUtility::select<Variable>(rand, forWriting ? writableVars : readableVars);
}
