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
#include "RandomUtility.h"
#include "VariableExpression.h"

ProgramFactory::ProgramFactory(RandomGenerator& rand) : rand(rand) {}

Program* ProgramFactory::genProgram(void* seed) {
	LinearAllocator::getInstance().reset();
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
	List<Variable*> printOrder(p->getVariables(), p->getVariablesEnd());
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
