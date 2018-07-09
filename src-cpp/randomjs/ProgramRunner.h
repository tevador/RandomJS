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

#include "FastStream.h"

class Program;

//forward declarations to avoid including boost headers
namespace boost {
	namespace process {
		class child;

		template<class CharT, class Traits = std::char_traits<CharT>>
		class basic_ipstream;

		template<class CharT, class Traits = std::char_traits<CharT>>
		class basic_opstream;

		typedef basic_ipstream<char> ipstream;
		typedef basic_opstream<char> opstream;
	}
}

class ProgramRunner
{
public:
	ProgramRunner(const char* executable, bool searchPath = false, const char* arguments = nullptr);
	void writeProgram(Program*);
	const char* getProgramBuffer() const {
		return stream.data();
	}
	size_t getProgramSize() const {
		return stream.size();
	}
	int executeProgram(char*);
private:
	void startProcess();

	FastStream stream;
	const char* executable;
	const char* arguments;
	boost::process::child* runner;
	boost::process::ipstream* runnerStdout;
	boost::process::opstream* runnerStdin;
};

