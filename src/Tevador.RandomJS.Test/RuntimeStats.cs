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

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Tevador.RandomJS.Run;
using System.IO;
using System.Security.Cryptography;

namespace Tevador.RandomJS.Test
{
    class RuntimeStats : IEnumerable<RuntimeInfo>
    {
        List<RuntimeInfo> _list;
        EntropyCounter _ec;
        object _lock = new object();
        int _target;

        public RuntimeStats(int count)
        {
            _target = count;
            _list = new List<RuntimeInfo>(count);
            _ec = new EntropyCounter();
        }

        public ListStats<RuntimeInfo> Runtime { get; set; }
        public ListStats<RuntimeInfo> CyclomaticComplexity { get; set; }
        public ListStats<RuntimeInfo> HalsteadDifficulty { get; set; }
        public ListStats<RuntimeInfo> LinesOfCode { get; set; }
        public double OutputEntropy { get; set; }

        public void Calculate()
        {
            if (_list.Count < _target || Runtime != null) return;
            _list.Sort();
            Runtime = new ListStats<RuntimeInfo>(_list, r => r.Runtime, false);
            CyclomaticComplexity = new ListStats<RuntimeInfo>(_list, r => r.CyclomaticComplexity);
            HalsteadDifficulty = new ListStats<RuntimeInfo>(_list, r => r.HalsteadDifficulty);
            LinesOfCode = new ListStats<RuntimeInfo>(_list, r => r.LinesOfCode);
            foreach (var ri in _list)
            {
                _ec.Add(ri.Output);
            }
            OutputEntropy = _ec.GetEntropy();
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool withHistogram)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                writer.WriteLine($"Longest runtimes:");
                for (int i = 1; i <= 10; ++i)
                {
                    var r = _list[_list.Count - i];
                    writer.WriteLine($"Seed = {r.Seed}, Runtime = {r.Runtime:0.00000} s");
                }
                writer.WriteLine($"Runtime [s] Min: {Runtime.Min:0.00000}; Max: {Runtime.Max:0.00000}; Avg: {Runtime.Average:0.00000}; Stdev: {Runtime.StdDev:0.00000};");
                writer.WriteLine($"Runtime [s] 99.99th percentile: {Runtime.GetPercentile(0.9999)}");
                writer.WriteLine($"Average entropy of program output (est.): {OutputEntropy:0.00} bits");
                writer.WriteLine($"Cyclomatic complexity Min: {CyclomaticComplexity.Min}; Max: {CyclomaticComplexity.Max}; Avg: {CyclomaticComplexity.Average}; Stdev: {CyclomaticComplexity.StdDev};");
                writer.WriteLine($"Lines of code Min: {LinesOfCode.Min}; Max: {LinesOfCode.Max}; Avg: {LinesOfCode.Average}; Stdev: {LinesOfCode.StdDev};");
                writer.WriteLine($"Halstead difficulty Min: {HalsteadDifficulty.Min}; Max: {HalsteadDifficulty.Max}; Avg: {HalsteadDifficulty.Average}; Stdev: {HalsteadDifficulty.StdDev};");
                if (withHistogram)
                {
                    int[] histogram = new int[(int)Math.Ceiling((Runtime.Max - Runtime.Min) / Runtime.StdDev * 10)];
                    foreach (var run in _list)
                    {
                        var index = (int)(((run.Runtime - Runtime.Min) / Runtime.StdDev * 10));
                        histogram[index]++;
                    }
                    writer.WriteLine("Runtime histogram:");
                    for (int j = 0; j < histogram.Length; ++j)
                    {
                        writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00000} {1}", j * Runtime.StdDev / 10 + Runtime.Min, histogram[j]));
                    }
                }
                using (var sha256 = new SHA256Managed())
                {
                    byte[] cumulative = new byte[sha256.HashSize / 8];
                    foreach (var ri in _list)
                    {
                        var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(ri.Output));
                        for(int i = 0; i < hash.Length; ++i)
                        {
                            cumulative[i] ^= hash[i];
                        }
                    }
                    writer.WriteLine($"Cumulative output hash: {BinaryUtils.ByteArrayToString(cumulative)}");
                }
            }
            return sb.ToString();
        }

        public RuntimeInfo Add()
        {
            RuntimeInfo ri = null;
            lock (_lock)
            {
                if (_list.Count < _target)
                {
                    ri = new RuntimeInfo();
                    _list.Add(ri);
                    Percent = _list.Count / (double)_target;
                }
            }
            return ri;
        }

        public double Percent
        {
            get; private set;
        }

        public IEnumerator<RuntimeInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
