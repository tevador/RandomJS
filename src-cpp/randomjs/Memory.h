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

#include <memory>
#include <exception>
#include <string>
#include <vector>
#include <sstream>

class OutOfMemoryException : public std::exception {
	virtual const char* what() const throw() {
		return "Memory allocation failed";
	}
};

class LinearAllocator {
private:
	char* bufferStart;
	char* bufferHead;
	static constexpr size_t size = 1 * 1024 * 1024; //1 MiB
	static thread_local LinearAllocator instance;
	LinearAllocator();

public:
	~LinearAllocator();
	static LinearAllocator& getInstance() { return instance; }
	void* allocate(size_t numBytes);
	void reset();
	size_t getUsed() { return bufferHead - bufferStart; }
};

template<typename T>
class Allocator : public std::allocator<T> {
public:
	T* allocate(size_t count) {
		return static_cast<T*>(LinearAllocator::getInstance().allocate(count * sizeof(T)));
	}
	void deallocate(T*, size_t) noexcept {}

	Allocator() : std::allocator<T>() {}
	Allocator(const Allocator& a) { }
	template <typename U>
	Allocator(const Allocator<U>& a) { }
	~Allocator() { }

	template <typename U>
	bool operator==(const Allocator<U>&) { return true; }
	template <typename U>
	bool operator!=(const Allocator<U>&) { return false; }

	template<typename U>
	struct rebind {
		typedef Allocator<U> other;
	};
};

class AllocatorBase {
public:
	void *operator new(size_t);
};

using String = std::basic_string<char, std::char_traits<char>, Allocator<char>>;

using StringBuilder = std::basic_ostringstream<char, std::char_traits<char>, Allocator<char>>;


template <typename T>
using List = std::vector<T, Allocator<T>>;