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

struct OperatorRequirement {
	static constexpr uint32_t None = 0;
	static constexpr uint32_t NumericOnly = 1 << 0;
	static constexpr uint32_t RhsNonzero = 1 << 1;
	static constexpr uint32_t RhsNonnegative = 1 << 2;
	static constexpr uint32_t FunctionCall = 1 << 3;
	static constexpr uint32_t LimitedPrecision = 1 << 4;
	static constexpr uint32_t Prefix = 1 << 5;
	static constexpr uint32_t WithoutRhs = 1 << 6;
	static constexpr uint32_t StringLengthLimit = 1 << 7;
};