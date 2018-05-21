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
using System.Text;
using Tevador.RandomJS.Crypto.Blake;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;

namespace Tevador.RandomJS.Crypto
{
    class Miner
    {
        const int N = 8; //asymmetry: solving requires 2^N times more effort than verifying
        const int _bound = (1 << (8 - N));
        const byte _clearMask = (_bound - 1);
        const int _nonceOffset = 39;
        Blake2B256 _blake = new Blake2B256();
        Blake2B256 _blakeKeyed;
        ProgramFactory _factory = new ProgramFactory();
        byte[] _blockTemplate;
        MemoryStream _programStream = new MemoryStream(256 * 1024);
        StreamWriter _programWriter;

        public Miner()
        {
            _programWriter = new StreamWriter(_programStream) { NewLine = "\n" };
        }

        public void Reset(byte[] blockTemplate)
        {
            _blockTemplate = blockTemplate;
        }

        private int ExecuteProgram(out string output, out string error)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:18111");
                request.KeepAlive = false;
                request.Timeout = 10000;
                request.Method = "POST";
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(_programStream.GetBuffer(), 0, (int)_programStream.Length);
                reqStream.Close();
                WebResponse response = request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    output = reader.ReadToEnd();
                }
                error = null;
                return 0;
            }
            catch (WebException e)
            {
                output = null;
                var response = e.Response as HttpWebResponse;
                if (response != null)
                {
                    error = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    return (int)response.StatusCode;
                }
                else
                {
                    error = e.Message;
                    return e.HResult;
                }
            }
        }

        private void WriteProgram(IProgram program)
        {
            _programStream.SetLength(0);
            program.WriteTo(_programWriter);
            _programWriter.Flush();
        }

        public unsafe Solution Solve()
        {
            byte[] result = null;
            byte[] auxiliary = null;
            uint nonce;
            fixed (byte* block = _blockTemplate)
            {
                uint* noncePtr = (uint*)(block + _nonceOffset);
                do
                {
                    (*noncePtr)++;
                    byte[] key = _blake.ComputeHash(_blockTemplate);
                    var program = _factory.GenProgram(key);
                    WriteProgram(program);
                    _blakeKeyed = new Blake2B256(key);
                    auxiliary = _blakeKeyed.ComputeHash(_programStream.GetBuffer(), 0, (int)_programStream.Length);
                    int exitCode;
                    string output, error;
                    if(0 != (exitCode = ExecuteProgram(out output, out error)))
                    {
                        throw new Exception(string.Format($"Program execution failed (Exit code {exitCode}). Nonce value: {(*noncePtr)}. Seed: {BinaryUtils.ByteArrayToString(key)}, {error}"));
                    }
                    result = _blakeKeyed.ComputeHash(Encoding.ASCII.GetBytes(output));
                }
                while ((result[0] ^ auxiliary[0]) >= _bound);
                nonce = *noncePtr;
            }
            result[0] &= _clearMask;
            for(int i = 0; i < result.Length; ++i)
            {
                result[i] ^= auxiliary[i];
            }
            return new Solution()
            {
                Nonce = nonce,
                Result = result,
                ProofOfWork = _blakeKeyed.ComputeHash(result)
            };
        }

        public bool Verify(Solution sol)
        {
            for (int i = 0; i < 4; ++i)
            {
                _blockTemplate[_nonceOffset + i] = (byte)(sol.Nonce >> (8 * i));
            }
            byte[] key = _blake.ComputeHash(_blockTemplate);
            _blakeKeyed = new Blake2B256(key);
            var pow = _blakeKeyed.ComputeHash(sol.Result);
            if(!BinaryUtils.ArraysEqual(pow, sol.ProofOfWork))
            {
                Console.WriteLine("Invalid PoW");
                return false;
            }
            var program = _factory.GenProgram(key);
            WriteProgram(program);
            var auxiliary = _blakeKeyed.ComputeHash(_programStream.GetBuffer(), 0, (int)_programStream.Length);
            if ((auxiliary[0] ^ sol.Result[0]) >= _bound)
            {
                Console.WriteLine("Invalid Auxiliary");
                return false;
            }
            auxiliary[0] &= _clearMask;
            int exitCode;
            string output, error;
            if (0 != (exitCode = ExecuteProgram(out output, out error)))
            {
                throw new Exception(string.Format($"Program execution failed (Exit code {exitCode}). Nonce value: {sol.Nonce}. Seed: {BinaryUtils.ByteArrayToString(key)}, {error}"));
            }
            var result = _blakeKeyed.ComputeHash(Encoding.ASCII.GetBytes(output));
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] ^= auxiliary[i];
            }
            if (!BinaryUtils.ArraysEqual(sol.Result, result))
            {
                Console.WriteLine("Invalid Result");
                return false;
            }
            return true;
        }

        class RuntimeInfo : IComparable<RuntimeInfo>
        {
            public RuntimeInfo(string seed, double runtime)
            {
                Seed = seed;
                Runtime = runtime;
            }

            public string Seed { get; private set; }
            public double Runtime { get; private set; }

            public int CompareTo(RuntimeInfo other)
            {
                return Runtime.CompareTo(other.Runtime);
            }
        }

        private void MakeStats(int count)
        {
            var runtimes = new List<RuntimeInfo>(count);
            var factory = new ProgramFactory();
            string output, error;
            int exitCode = 0;
            IProgram p;
            //warm up
            p = factory.GenProgram(BinaryUtils.GenerateSeed(Environment.TickCount));
            WriteProgram(p);
            ExecuteProgram(out output, out error);
            Console.WriteLine("Warmed up");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; ++i)
            {
                var seed = Environment.TickCount + i;
                var gs = BinaryUtils.GenerateSeed(seed);
                var ss = BinaryUtils.ByteArrayToString(gs);
                if ((i & 127) == 0)
                    Console.WriteLine("Seed = {0}", ss);
                p = factory.GenProgram(gs);
                WriteProgram(p);
                exitCode = ExecuteProgram(out output, out error);
                if (exitCode != 0)
                {
                    Console.WriteLine("Error Seed = {0}", ss);
                    Console.WriteLine($"// ExitCode = {exitCode}");
                    Console.WriteLine(output);
                    Console.WriteLine(error);
                    break;
                }
                const string prefix = "RUNTIME: ";
                int runtimeIndex = output.IndexOf(prefix);
                if (runtimeIndex < 0)
                {
                    Console.WriteLine("Runtime info not found");
                    break;
                }
                string runtimeStr = output.Substring(runtimeIndex + prefix.Length);
                double runtime;
                if (!double.TryParse(runtimeStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out runtime))
                {
                    Console.WriteLine($"Invalid Runtime info {runtimeStr}");
                    break;
                }
                runtimes.Add(new RuntimeInfo(ss, 1000 * runtime));
            }
            sw.Stop();

            Console.WriteLine($"// {runtimes.Count} seeds processed in {sw.Elapsed.TotalSeconds} seconds");

            if (exitCode != 0) return;

            runtimes.Sort();
            //runtimes.RemoveAt(0);
            //runtimes.RemoveAt(runtimes.Count - 1);
            var avg = runtimes.Average(r => r.Runtime);
            var min = runtimes[0].Runtime;
            var max = runtimes[runtimes.Count - 1].Runtime;
            Console.WriteLine($"Longest runtimes:");
            for (int i = 1; i <= 10; ++i)
            {
                var r = runtimes[runtimes.Count - i];
                Console.WriteLine($"Seed = {r.Seed}, Runtime = {r.Runtime} ms");
            }
            var sqsum = runtimes.Sum(d => (d.Runtime - avg) * (d.Runtime - avg));
            var stdev = Math.Sqrt(sqsum / runtimes.Count);
            Console.WriteLine($"Runtime: Min: {min}; Max: {max}; Avg: {avg}; Stdev: {stdev};");
            int[] histogram = new int[(int)Math.Ceiling((max - min) / stdev * 10)];
            foreach (var run in runtimes)
            {
                histogram[(int)(((run.Runtime - min) / stdev * 10))]++;
            }
            Console.WriteLine("Histogram:");
            for (int j = 0; j < histogram.Length; ++j)
            {
                Console.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", j * stdev / 10 + min, histogram[j]));
            }
        }

        static void Main(string[] args)
        {
#if PERF
            var miner = new Miner();
            var count = 10000;
            var sw = Stopwatch.StartNew();
            for(int i = 0; i < count; ++i)
            {
                var seed = BinaryUtils.GenerateSeed(Environment.TickCount + i);
                var p = miner._factory.GenProgram(seed);
                miner.WriteProgram(p);
            }
            sw.Stop();
            Console.WriteLine($"{count} seeds processed in {sw.Elapsed.TotalSeconds} s");
#endif
#if !PERF
            if (args.Length > 0 && args[0] == "--stats")
            {
                var miner = new Miner();
                int count;
                if (args.Length == 1 || !int.TryParse(args[1], out count))
                    count = 10000;
                miner.MakeStats(count);
                return;
            }
            string blockTemplateHex = "0707f7a4f0d605b303260816ba3f10902e1a145ac5fad3aa3af6ea44c11869dc4f853f002b2eea0000000077b206a02ca5b1d4ce6bbfdf0acac38bded34d2dcdeef95cd20cefc12f61d56109";
            if (args.Length > 0)
            {
                blockTemplateHex = args[0];
            }
            if (blockTemplateHex.Length != 152 || blockTemplateHex.Any(c => !"0123456789abcdef".Contains(c)))
            {
                Console.WriteLine("Invalid block template (152 hex characters expected).");
            }
            else
            {
                try
                {
                    var blockTemplate = BinaryUtils.StringToByteArray(blockTemplateHex);
                    var miner = new Miner();
                    miner.Reset(blockTemplate);
                    TimeSpan period = TimeSpan.FromMinutes(1);
                    List<Solution> solutions = new List<Solution>(100);
                    Stopwatch sw = Stopwatch.StartNew();
                    while (sw.Elapsed < period)
                    {
                        var solution = miner.Solve();
                        Console.WriteLine($"Nonce = {solution.Nonce}; PoW = {BinaryUtils.ByteArrayToString(solution.ProofOfWork)}");
                        solutions.Add(solution);
                    }
                    sw.Stop();
                    var seconds = sw.Elapsed.TotalSeconds;
                    Console.WriteLine();
                    Console.WriteLine($"Solving nonces: {string.Join(", ", solutions.Select(s => s.Nonce))}");
                    Console.WriteLine();
                    Console.WriteLine($"Found {solutions.Count} solutions in {seconds} seconds. Performance = {solutions.Count / seconds} Sols./s.");
                    sw.Restart();
                    foreach (var sol in solutions)
                    {
                        if (!miner.Verify(sol))
                        {
                            Console.WriteLine($"Nonce {sol.Nonce} - verification failed");
                            return;
                        }
                    }
                    sw.Stop();
                    Console.WriteLine($"All {solutions.Count} solutions were verified in {sw.Elapsed.TotalSeconds} seconds");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: {e}");
                }
            }
#endif
        }
    }
}
