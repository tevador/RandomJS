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

#include "FastBuffer.h"
#include <iostream>

class FastStream : public std::ostream {
public:
	FastStream() : std::ostream(&buffer) {}

	FastStream(size_t capacity) : std::ostream(&buffer), buffer(capacity) {}

	FastStream(const FastStream& other) : std::ostream(&buffer), buffer(other.buffer)
	{}

	FastStream& operator=(FastStream other) {
		std::swap(*this, other);
		return *this;
	}
	void swap(FastStream& first, FastStream& second) {
		std::swap(first.buffer, second.buffer);
	}
	const char* data() const {
		return buffer.data();
	}
	size_t size() const	{
		return buffer.size();
	}
	void clear() {
		buffer.clear();
	}
	void null() {
		buffer.null();
	}
private:
	FastBuffer buffer;
};