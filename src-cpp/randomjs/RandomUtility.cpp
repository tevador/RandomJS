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

#include "RandomUtility.h"
#include "Variable.h"

template<typename T>
void RandomUtility::shuffle(RandomGenerator& rand, List<T>& list) {
	for (auto i = list.size() - 1; i >= 1; --i) {
		auto j = rand.genInt(i + 1);
		//swap
		T temp = list[i];
		list[i] = list[j];
		list[j] = temp;
	}
}

template void RandomUtility::shuffle<Variable*>(RandomGenerator&, List<Variable*>&);

template<typename T>
T RandomUtility::select(RandomGenerator& rand, List<T>* items) {
	return (*items)[rand.genInt(items->size())];
}

template Variable* RandomUtility::select(RandomGenerator&, List<Variable*>*);