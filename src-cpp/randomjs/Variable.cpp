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

#include "Variable.h"
#include "Memory.h"
#include <algorithm>

Variable Variable::This = Variable(nullptr, "this", true, false);

const char* Variable::getVariableName(int index) {
	String* str = new String();
	while (index >= 0) {
		int mod = index % 26;
		str->push_back((char)('a' + mod));
		index = index / 26 - 1;
	}
	std::reverse(str->begin(), str->end());
	return str->data();
}

std::ostream& operator<<(std::ostream& os, const Variable& v) {
	os << v.getName();
	return os;
}