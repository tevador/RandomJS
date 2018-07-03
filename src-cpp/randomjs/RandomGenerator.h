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

------------------------------------------------------------------------

Based on C code from http://xoshiro.di.unimi.it/xoshiro256plus.c

Original copyright statement:

Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)

To the extent possible under law, the author has dedicated all copyright
and related and neighboring rights to this software to the public domain
worldwide. This software is distributed without any warranty.

See <http://creativecommons.org/publicdomain/zero/1.0/>.
*/

#pragma once

#include <cstdint>

static inline uint64_t rotl(const uint64_t x, int k) {
	return (x << k) | (x >> (64 - k));
}

static inline double to_double(uint64_t x) {
	union { uint64_t i; double d; } u;
	u.i = UINT64_C(0x3FF) << 52 | x >> 12;
	return u.d - 1.0;
}

class RandomGenerator {
private:
	uint64_t s[4];
public:
	void seed(void* buffer) {
		uint64_t* lbuff = (uint64_t*)buffer;
		s[0] = lbuff[0];
		s[1] = lbuff[1];
		s[2] = lbuff[2];
		s[3] = lbuff[3];
	}

	uint64_t genInt64() {
		const uint64_t result_plus = s[0] + s[3];

		const uint64_t t = s[1] << 17;

		s[2] ^= s[0];
		s[3] ^= s[1];
		s[1] ^= s[2];
		s[0] ^= s[3];

		s[2] ^= t;

		s[3] = rotl(s[3], 45);

		return result_plus;
	}

	double gen() {
		return to_double(genInt64());
	}

	int32_t genInt(int32_t max) {
		return (int32_t)(gen() * max);
	}

	int32_t genInt(int32_t min, int32_t max) {
		return min + genInt(max - min);
	}

	bool flipCoin(double chance) {
		return gen() < chance;
	}

	bool flipCoin() {
		return flipCoin(0.5);
	}
};