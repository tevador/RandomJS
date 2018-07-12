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

const std::string RandomUtility::printableChars = "STsqQ$jM`@yZ2wav(794)N,{iGW0[Ot_K/me3.nLHrf]=-!b~g<pcV&k%1o+;YB':h^dCP86}IDE?5*\tA#X\\F\"RzJx\nulU>| ";
const std::string RandomUtility::hexChars = "5b2de387f419ac60";
const std::string RandomUtility::decimalChars = "4651320978";
const std::string RandomUtility::octalChars = "37652014";
const std::string RandomUtility::binaryChars = "01";
const std::string RandomUtility::evalChars = "/cb1/|=`+-a2+e84";

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

template<class T>
T* RandomUtility::select(RandomGenerator& rand, List<T*>* items) {
	if (items->size() == 0)
		return nullptr;
	return (*items)[rand.genInt(items->size())];
}

template Variable* RandomUtility::select<Variable>(RandomGenerator&, List<Variable*>*);

void RandomUtility::genString(RandomGenerator& rand, String* str, int length, const std::string& charset, bool canStartWithZero) {
	if (!canStartWithZero) {
		char c = '\0';
		while (length-- > 0 && (c = charset[rand.genInt(charset.length())]) == '0');
		str->push_back(c);
	}
	while (length-- > 0) {
		str->push_back(charset[rand.genInt(charset.length())]);
	}
}

const char* RandomUtility::genEvalString(RandomGenerator& rand, int length) {
	return genStringLiteral(rand, length, evalChars);
}

const char* RandomUtility::genStringLiteral(RandomGenerator& rand, int length) {
	return genStringLiteral(rand, length, printableChars);
}

const char* RandomUtility::genStringLiteral(RandomGenerator& rand, int length, const std::string& charset) {
	char quote = rand.flipCoin() ? '\'' : '"';
	String* str = new (LinearAllocator::getInstance().allocate(sizeof(String))) String();
	str->reserve(2 * length);
	str->push_back(quote);
	while (length-- > 0) {
		char c = charset[rand.genInt(charset.length())];
		if (c == '\n') {
			str->append("\\n");
			continue;
		}
		if (c == '\t') {
			str->append("\\t");
			continue;
		}
		if (c == quote || c == '\\') {
			str->push_back('\\');
		}
		str->push_back(c);
	}
	str->push_back(quote);
	return str->data();
}