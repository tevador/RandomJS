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

#include <iostream>

class Operator {
public:
	bool has(uint32_t flag) {
		return (flags & flag) != 0;
	}

	friend std::ostream& operator<<(std::ostream& os, const Operator& op) {
		os << op.symbol;
		return os;
	}
protected:
	Operator(const char* symbol, uint32_t flags) : symbol(symbol), flags(flags) {}
private:
	const char* symbol;
	uint32_t flags;
};