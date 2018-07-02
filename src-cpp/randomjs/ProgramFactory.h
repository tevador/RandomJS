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

#include "RandomGenerator.h"
#include "Program.h"
#include "CodeStatement.h"
#include "Variable.h"
#include "OutputStatement.h"
#include <cstdint>

class ProgramFactory
{
public:
	ProgramFactory(RandomGenerator&);
	Program* genProgram(void* seed);

private:
	RandomGenerator& rand;
	static CodeStatement callDepthCheck;
	static CodeStatement incrementCallSum;
	static CodeStatement incrementCallSumDepth;
	static CodeStatement loopCyclesCheck;

	int32_t genValueFromInterval(int32_t, int32_t);
	Variable* genVariable(IScope* scope, bool isParameter = false, bool isLoopCounter = false, bool isConstant = false, bool initialize = true);
	OutputStatement* genOutputStatement(Program* program, Expression* expr);
	OutputStatement* genOutputStatement(Program* program, IVariable* v);

};

