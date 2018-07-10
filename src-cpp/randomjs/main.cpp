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

#include "RandomGenerator.h"
#include "ProgramFactory.h"
#include "ProgramRunner.h"
#include <iostream>
#include <chrono>
#include "blake2/blake2.h"

constexpr char hexmap[] = "0123456789abcdef";
constexpr int programCount = 1000;

void outputHex(std::ostream& os, const char* data, int length) {
	for (int i = 0; i < length; ++i) {
		os << hexmap[(data[i] & 0xF0) >> 4];
		os << hexmap[data[i] & 0x0F];
	}
	os << std::endl;
}

int main(int argc, char** argv) {
	char seed[32];
	char hash[32];
	char cumulative[32] = { 0 };
	uint64_t* ch64 = (uint64_t*)cumulative;
	char outputBuffer[16 * 1024];
	unsigned char blockTemplate[] =	{
								0x07, 0x07, 0xf7, 0xa4, 0xf0, 0xd6, 0x05, 0xb3, 0x03, 0x26, 0x08, 0x16, 0xba, 0x3f, 0x10, 0x90, 0x2e, 0x1a, 0x14,
								0x5a, 0xc5, 0xfa, 0xd3, 0xaa, 0x3a, 0xf6, 0xea, 0x44, 0xc1, 0x18, 0x69, 0xdc, 0x4f, 0x85, 0x3f, 0x00, 0x2b, 0x2e,
								0xea, 0x00, 0x00, 0x00, 0x00, 0x77, 0xb2, 0x06, 0xa0, 0x2c, 0xa5, 0xb1, 0xd4, 0xce, 0x6b, 0xbf, 0xdf, 0x0a, 0xca, 
								0xc3, 0x8b, 0xde, 0xd3, 0x4d, 0x2d, 0xcd, 0xee, 0xf9, 0x5c, 0xd2, 0x0c, 0xef, 0xc1, 0x2f, 0x61, 0xd5, 0x61, 0x09 
							};
	ProgramRunner runner("./xst");
	int* nonce = (int*)(blockTemplate + 39);
	RandomGenerator rand;
	ProgramFactory pf(rand);
	auto hptStart = std::chrono::high_resolution_clock::now();
	for (int i = 0; i < programCount; ++i) {
		*nonce = i;
		blake2b(seed, sizeof(seed), blockTemplate, sizeof(blockTemplate), nullptr, 0);
		Program* p = pf.genProgram(seed);
		runner.writeProgram(p);
		int outputLength = runner.executeProgram(outputBuffer);
		if (outputLength >= 0) {
			blake2b(hash, sizeof(hash), outputBuffer, outputLength, seed, sizeof(seed));
			//outputHex(std::cout, hash, sizeof(hash));
			ch64[0] ^= ((uint64_t*)hash)[0];
			ch64[1] ^= ((uint64_t*)hash)[1];
			ch64[2] ^= ((uint64_t*)hash)[2];
			ch64[3] ^= ((uint64_t*)hash)[3];
		}
	}
	auto hptEnd = std::chrono::high_resolution_clock::now();
	std::cout << "Cumulative output hash: ";
	outputHex(std::cout, cumulative, sizeof(cumulative));
	std::cout << "Performance: " << programCount / std::chrono::duration<double>(hptEnd - hptStart).count() << " programs per second" << std::endl;
	return 0;
}