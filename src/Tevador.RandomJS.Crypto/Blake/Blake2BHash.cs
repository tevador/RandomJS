/*
    (c) 2018 tevador <tevador@gmail.com>

    This file is part of Tevador.RandomJS.

    Tevador.RandomJS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Tevador.RandomJS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Tevador.RandomJS.  If not, see<http://www.gnu.org/licenses/>.
*/

// BLAKE2 reference source code package - C# implementation

// Written in 2012 by Christian Winnerlein  <codesinchaos@gmail.com>

// To the extent possible under law, the author(s) have dedicated all copyright
// and related and neighboring rights to this software to the public domain
// worldwide. This software is distributed without any warranty.

// You should have received a copy of the CC0 Public Domain Dedication along with
// this software. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Security.Cryptography;

namespace Tevador.RandomJS.Crypto.Blake
{
    internal class Blake2BHash : HashAlgorithm
    {
        private readonly Blake2BCore core = new Blake2BCore();
        private readonly ulong[] _rawConfig;
        private readonly byte[] _key;
        private readonly int outputSizeInBytes;
        private bool _keyed;
        private static readonly Blake2BConfig DefaultConfig = new Blake2BConfig();

        public Blake2BHash(Blake2BConfig config)
        {
            if (config == null)
                config = DefaultConfig;
            _rawConfig = Blake2IvBuilder.ConfigB(config, null);
            _key = new byte[128];
            SetKey(config.Key);
            outputSizeInBytes = config.OutputSizeInBytes;
            Initialize();
        }

        protected void SetKey(byte[] key)
        {
            if (key != null && key.Length != 0)
            {
                Array.Copy(key, _key, key.Length);
                _keyed = true;
            }
            else
            {
                _keyed = false;
            }
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            core.HashCore(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            var fullResult = core.HashFinal();
            if (outputSizeInBytes != fullResult.Length)
            {
                var result = new byte[outputSizeInBytes];
                Array.Copy(fullResult, result, result.Length);
                return result;
            }
            else return fullResult;
        }

        public override void Initialize()
        {
            core.Initialize(_rawConfig);
            if (_keyed)
            {
                core.HashCore(_key, 0, _key.Length);
            }
        }
    }
}