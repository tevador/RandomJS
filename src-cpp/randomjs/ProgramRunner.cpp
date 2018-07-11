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

#include "ProgramRunner.h"
#include "Program.h"
#include <boost/process.hpp>
#include <boost/filesystem.hpp>

constexpr size_t programBufferCapacity = 256 * 1024;

namespace bp = boost::process;
namespace bf = boost::filesystem;

void ProgramRunner::startProcess() {
	delete runnerStdout;
	delete runnerStdin;
	delete runner;
	runnerStdout = new bp::ipstream();
	runnerStdin = new bp::opstream();
	if (arguments != nullptr) {
		runner = new bp::child(executable, arguments, bp::std_out > *runnerStdout, bp::std_in < *runnerStdin);
	}
	else {
		runner = new bp::child(executable, bp::std_out > *runnerStdout, bp::std_in < *runnerStdin);
	}
}

ProgramRunner::ProgramRunner(const char* self, const char* xs)
	: stream(programBufferCapacity),
	arguments(nullptr),
	runner(nullptr),
	runnerStdout(nullptr),
	runnerStdin(nullptr)
{
	std::vector<bf::path> searchPaths;
	auto basedir = bf::system_complete(self).parent_path();
	searchPaths.push_back(basedir);
	auto fullPath = bp::search_path(xs, searchPaths);
	executable = fullPath.string();
	startProcess();
}

ProgramRunner::~ProgramRunner() {
	delete runnerStdout;
	delete runnerStdin;
	delete runner;
}

/*ProgramRunner::ProgramRunner(const char* executable, bool searchPath, const char* arguments)
	: stream(programBufferCapacity),
	arguments(arguments),
	runner(nullptr),
	runnerStdout(nullptr),
	runnerStdin(nullptr)
{
	if (searchPath) {
		auto fullPath = boost::process::search_path(executable).generic_string();
		char* pathBuffer = new char[fullPath.length()];
		std::copy(fullPath.begin(), fullPath.end(), pathBuffer);
		this->executable = pathBuffer;
	}
	else {
		this->executable = executable;
	}
	startProcess();
}*/

void ProgramRunner::writeProgram(Program* p) {
	stream.clear();
	stream << *p;
	stream.null();
}

int ProgramRunner::executeProgram(char* outputBuffer) {
	*runnerStdin << stream.data() << '\0';
	runnerStdin->flush();

	int buffPos = 0;

	char c;
	while (runnerStdout->get(c) && c != '\0') {
		outputBuffer[buffPos++] = c;
	}

	if (runnerStdout->eof()) {
		startProcess();
		return -1;
	}

	outputBuffer[buffPos] = '\0';

	return buffPos;
}