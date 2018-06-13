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
using System.Reflection;

namespace Tevador.RandomJS.Test
{
    class Program
    {
        private static RuntimeStats MakeStats(int threads, int count, int timeout, long seed, ProgramOptions options, bool silent, bool evalTest)
        {
            if (!silent)
                Console.WriteLine($"Collecting statistics from {count} random program executions (seed = {seed})");
            double step = 0.05;
            double next = step;
            var runner = new ParallelRunner(seed, options, evalTest);
            if (!silent)
                runner.Progress += (s, e) =>
                {
                    if (runner.Percent > next)
                    {
                        Console.Write($"{runner.Percent:P0}, ");
                        next += step;
                    }
                };
            var sw = Stopwatch.StartNew();
            RuntimeStats stats = null;
            try
            {
                stats = runner.Run(threads, count, timeout);
            }
            catch(Exception e)
            {
                if (!silent)
                    Console.WriteLine(e);
            }
            sw.Stop();
            if (!silent)
            {
                Console.WriteLine();
                Console.WriteLine($"Completed in {sw.Elapsed.TotalSeconds} seconds");
            }
            var fail = stats?.FirstOrDefault(ri => !ri.Success);
            if(fail != null)
            {
                if (!silent)
                {
                    Console.WriteLine($"Error seed: {fail.Seed}");
                    Console.WriteLine(fail.Output);
                }
                stats = null;
            }
            else if(stats != null && stats.IsComplete)
            {
                stats.Calculate();
            }
            return stats;
        }

        static int Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            int threads = 1;
            int count = 1000;
            long seed = DateTime.UtcNow.Ticks;
            bool objective = false;
            bool useCustomOptions = false;
            double runtimeTarget = 0.01; //10 ms
            double runtimeWeight = 3e+7;
            double percentileWeight = 500.0;
            double entropyWeight = 1000.0;
            double linesOfCodeWeight = 0.05;
            double halsteadDifficultyWeight = 0.5;
            double percentile = 0.999;
            double entropyLimit = 512;
            double evalTestWeightValidity = 20.0;
            double evalTestWeightRuntime = 40.0;
            bool verbose = false;
            int timeout = -1;
            bool evalTest = false;
            bool help = false;

            ProgramOptions customOptions = new ProgramOptions();
            customOptions.Initialize();
            Action<string> coValidate = (string s) =>
            {
                if (!useCustomOptions)
                    throw new InvalidOperationException($"The option {s} must be preceded by --customOptions");
            };

            OptionSet p = new OptionSet()
                .Add("threads=", (int i) => threads = i)
                .Add("count=", (int i) => count = i)
                .Add("seed=", (long i) => seed = i)
                .Add("timeout=", (int i) => timeout = 1000 * i)
                .Add("objective", s => objective = true)
                .Add("customOptions", s => useCustomOptions = true)
                .Add("verbose", s => verbose = true)
                .Add("runtimeTarget=", (double d) => runtimeTarget = d)
                .Add("runtimeWeight=", (double d) => runtimeWeight = d)
                .Add("percentileWeight=", (double d) => percentileWeight = d)
                .Add("percentile=", (double d) => percentile = d)
                .Add("entropyWeight=", (double d) => entropyWeight = d)
                .Add("entropyLimit=", (double d) => entropyLimit = d)
                .Add("linesOfCodeWeight=", (double d) => linesOfCodeWeight = d)
                .Add("halsteadDifficultyWeight=", (double d) => halsteadDifficultyWeight = d)
                .Add("evalTest", s => evalTest = true)
                .Add("evalTestWeightValidity=", (double d) => evalTestWeightValidity = d)
                .Add("evalTestWeightRuntime=", (double d) => evalTestWeightRuntime = d)
                .Add("help|h", s => help = true);


            foreach (var prop in typeof(ProgramOptions).GetProperties())
            {
                var pt = prop.PropertyType;
                if (pt.BaseType != null && pt.BaseType.IsGenericType && pt.BaseType.GetGenericTypeDefinition() == typeof(RandomTable<>))
                {
                    var itemType = pt.BaseType.GetGenericArguments()[0];
                    var instance = prop.GetValue(customOptions);
                    var addMethod = pt.BaseType.GetMethod("Add", new Type[] { itemType, typeof(double) });
                    foreach (var fld in itemType.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        string optionName = "XML_" + prop.Name + "_" + fld.Name;
                        //Console.WriteLine($"Adding option --{optionName}");
                        p.Add(optionName + "=", (double w) =>
                        {
                            coValidate(optionName);
                            addMethod.Invoke(instance, new object[] { fld.GetValue(null), w });
                        });
                    }
                }
                else if (pt == typeof(Interval))
                {
                    var instance = prop.GetValue(customOptions);
                    var minProp = pt.GetProperty(nameof(Interval.Min));
                    string optionNameMin = "XML_" + prop.Name + "_" + nameof(Interval.Min);
                    //Console.WriteLine($"Adding option --{optionNameMin}");
                    p.Add(optionNameMin + "=", (int i) =>
                    {
                        coValidate(optionNameMin);
                        minProp.SetValue(instance, i);
                    });
                    var rangeProp = pt.GetProperty(nameof(Interval.Span));
                    string optionNameRange = "XML_" + prop.Name + "_" + nameof(Interval.Span);
                    //Console.WriteLine($"Adding option --{optionNameRange}");
                    p.Add(optionNameRange + "=", (int i) =>
                    {
                        coValidate(optionNameRange);
                        rangeProp.SetValue(instance, i);
                    });
                }
                else
                {
                    string optionName = "XML_" + prop.Name;
                    //Console.WriteLine($"Adding option --{optionName}");
                    if (pt == typeof(int))
                        p.Add(optionName + "=", (int i) =>
                        {
                            coValidate(optionName);
                            prop.SetValue(customOptions, i);
                        });
                    if (pt == typeof(double))
                        p.Add(optionName + "=", (double d) =>
                        {
                            coValidate(optionName);
                            prop.SetValue(customOptions, d);
                        });
                    if (pt == typeof(bool))
                        p.Add(optionName + "=", (bool b) =>
                        {
                            coValidate(optionName);
                            prop.SetValue(customOptions, b);
                        });
                }
            }

            var unknown = p.Parse(args);

            if (help)
            {
                p.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            /*if (unknown.Any())
            {
                Console.WriteLine($"Unknown option '{unknown.First()}'");
                return 1;
            }*/

            var stats = MakeStats(threads, count, timeout, seed, useCustomOptions ? customOptions : ProgramOptions.FromXml(), objective, evalTest);

            if (objective)
            {
                if (stats != null && stats.IsComplete)
                {
                    var runtime1 = runtimeWeight * (stats.Runtime.Average - runtimeTarget) * (stats.Runtime.Average - runtimeTarget);
                    var runtime2 = percentileWeight * stats.Runtime.GetPercentile(percentile);
                    var entropy = entropyWeight / Math.Max(1, stats.OutputEntropy - entropyLimit);
                    var loc = -linesOfCodeWeight * stats.LinesOfCode.Average;
                    var halstead = -halsteadDifficultyWeight * stats.HalsteadDifficulty.Average;
                    var evalWeakness = 0.0;

                    if (evalTest)
                    {
                        evalWeakness = evalTestWeightValidity / Math.Max(0.05, 1.0 - stats.SyntaxErrorValidity) - evalTestWeightRuntime * stats.SyntaxErrorRuntime;
                    }

                    if (verbose)
                    {
                        Console.WriteLine($"Runtime: {runtime1:0.000}");
                        Console.WriteLine($"Percentile: {runtime2:0.000}");
                        Console.WriteLine($"Entropy: {entropy:0.000}");
                        Console.WriteLine($"Lines of code: {loc:0.000}");
                        Console.WriteLine($"Halstead: {halstead:0.000}");
                        if (evalTest) Console.WriteLine($"EvalWeakness: {evalWeakness:0.000}");
                    }

                    Console.WriteLine(runtime1 + runtime2 + entropy + loc + halstead + evalWeakness);
                }
                else
                    Console.WriteLine(int.MaxValue);
                return 0;
            }
            else if (stats != null && stats.IsComplete)
            {
                Console.WriteLine(stats.ToString(verbose));
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
