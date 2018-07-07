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

#include "Memory.h"

thread_local LinearAllocator LinearAllocator::instance = LinearAllocator();

LinearAllocator::LinearAllocator() {
	bufferStart = new char[size];
	bufferHead = bufferStart;
}

LinearAllocator::~LinearAllocator() {
	delete[] bufferStart;
}

void* LinearAllocator::allocate(size_t numBytes) {
	char* ptr = bufferHead;
	bufferHead += numBytes;
	if (bufferHead > bufferStart + size) {
		throw OutOfMemoryException();
	}
	return ptr;
}

void LinearAllocator::reset() {
	bufferHead = bufferStart;
}

void* AllocatorBase::operator new (size_t bytes) {
	return LinearAllocator::getInstance().allocate(bytes);
}