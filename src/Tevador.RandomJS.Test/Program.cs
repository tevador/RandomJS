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
using System.Linq;
using System.Diagnostics;
using Mono.Options;

namespace Tevador.RandomJS.Test
{
    class Program
    {
        private static void MakeStats(int threads, int count, long seed)
        {
            Console.WriteLine($"Collecting statistics from {count} random program executions (seed = {seed})");
            double step = 0.05;
            double next = step;
            var runner = new ParallelRunner(seed);
            runner.Progress += (s, e) =>
            {
                if(runner.Percent > next)
                {
                    Console.Write($"{runner.Percent:P0}, ");
                    next += step;
                }
            };
            var sw = Stopwatch.StartNew();
            var stats = runner.Run(threads, count);
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Completed in {sw.Elapsed.TotalSeconds} seconds");

            var fail = stats.FirstOrDefault(ri => !ri.Success);
            if(fail != null)
            {
                Console.WriteLine($"Error seed: {fail.Seed}");
                Console.WriteLine(fail.Output);
            }
            else
            {
                stats.Calculate();
                Console.WriteLine(stats);
            }
        }

        static void Main(string[] args)
        {
            int threads = 1;
            int count = 1000;
            long seed = DateTime.UtcNow.Ticks;

            OptionSet p = new OptionSet()
                .Add("threads=", (int i) => threads = i)
                .Add("count=", (int i) => count = i)
                .Add("seed=", (long i) => seed = i);

            p.Parse(args);

            MakeStats(threads, count, seed);
        }
    }
}
