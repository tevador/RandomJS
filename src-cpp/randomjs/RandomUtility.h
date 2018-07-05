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
#include "Memory.h"

class RandomUtility
{
public:
	static const std::string printableChars;
	static const std::string hexChars;
	static const std::string decimalChars;
	static const std::string octalChars;
	static const std::string binaryChars;
	static const std::string evalChars;

	template<typename T>
	static void shuffle(RandomGenerator& rand, List<T>& list);
	static const char* genEvalString(RandomGenerator& rand, int length);
	static void genString(RandomGenerator& rand, StringBuilder& sb, int length, const std::string& charset, bool canStartWithZero = true);
	static const char* genStringLiteral(RandomGenerator& rand, int length, const std::string& charset);
	static const char* genStringLiteral(RandomGenerator& rand, int length);
	template<typename T>
	static T select(RandomGenerator& rand, List<T>* items);
};

