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
#include <iostream>
#include <sstream>
#include <boost/process.hpp>

using namespace boost::process;

void readResult(std::istream& is) {
	char c;
	while (is.get(c) && c != '\0') {
		std::cout << c;
	}
	std::cout << "---------------------------------" << std::endl;
}

int main(int argc, char** argv) {
	char seed[32];
	ipstream is;
	opstream os;
	try {
		child xs("Q:\\projects\\moddable\\build\\bin\\win\\debug\\xst.exe", std_out > is, std_in < os);
		for (int i = 0; i < 100; ++i) {
			seed[0] = i;
			RandomGenerator rand;
			ProgramFactory pf(rand);
			Program* p = pf.genProgram(seed);
			os << *p << '\0';
			os.flush();
			readResult(is);
			if (!xs.running()) {
				return 1;
			}
		}
	}
	catch(...){
		return 1;
	}
	return 0;
}