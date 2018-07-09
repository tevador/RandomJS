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
#include <vector>

class FastBuffer : public std::streambuf {
public:
	FastBuffer() {}
	FastBuffer(size_t capacity) {
		buffer.reserve(capacity);
	}
	FastBuffer(const FastBuffer& other) : buffer(other.buffer) {}
	FastBuffer& operator=(FastBuffer other) {
		std::swap(buffer, other.buffer);
		return *this;
	}
	const char* data() const {
		return buffer.data();
	}
	size_t size() const {
		return buffer.size();
	}
	void clear() {
		buffer.clear();
	}
	void null() {
		buffer.push_back('\0');
	}
protected:
	std::streamsize xsputn(const char_type* s, std::streamsize count) override {
		for (std::streamsize i = 0; i < count; ++i, ++s)
			buffer.push_back(*s);
		return count;
	}

	int_type overflow(int_type ch) override {
		if (traits_type::not_eof(ch)) {
			char c = traits_type::to_char_type(ch);
			buffer.push_back(c);
		}
		return ch;
	}
private:
	std::vector<char> buffer;
};