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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tevador.RandomJS.Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            string blockTemplateHex = "0707f7a4f0d605b303260816ba3f10902e1a145ac5fad3aa3af6ea44c11869dc4f853f002b2eea0000000077b206a02ca5b1d4ce6bbfdf0acac38bded34d2dcdeef95cd20cefc12f61d56109";
            if (args.Length > 0)
            {
                blockTemplateHex = args[0];
            }
            if(blockTemplateHex.Length != 152 || blockTemplateHex.Any(c => !"0123456789abcdef".Contains(c)))
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
        }
    }
}
