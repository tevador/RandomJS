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

#include <cstdint>
#include "Memory.h"
#include "Variable.h"
#include "Global.h"

class IScope {
public:
	List<Variable*>::iterator getVariables() {
		return variables.begin();
	}

	List<Variable*>::iterator getVariablesEnd() {
		return variables.end();
	}
	
	virtual bool hasBreak() {
		return parent != nullptr && parent->hasBreak();
	}

	virtual void require(Global* global) {
		if (parent != nullptr) {
			parent->require(global);
		}
	}

	virtual IScope* getParent() {
		return parent;
	}

	uint32_t getFunctionDepth() {
		return functionDepth;
	}

	uint32_t getVariableCounter() {
		return variables.size();
	}

	void declareVariable(Variable* variable) {
		variables.push_back(variable);
	}

protected:
	uint32_t functionDepth;

	IScope(IScope* parent = nullptr);

private:
	IScope* parent;
	List<Variable*> variables;
};