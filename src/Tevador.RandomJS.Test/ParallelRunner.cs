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
using Tevador.RandomJS.Run;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace Tevador.RandomJS.Test
{
    class ParallelRunner
    {
        RuntimeStats _stats;
        ProgramOptions _options;
        long _seed;
        bool _evalTest;

        public ParallelRunner(long seed, ProgramOptions options, bool evalTest)
        {
            _seed = seed;
            _options = options;
            _evalTest = evalTest;
        }

        public event EventHandler Progress;

        public RuntimeStats Run(int threadCount, int programCount, int timeout)
        {
            _stats = new RuntimeStats(programCount);
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                CancellationToken token = source.Token;
                Task[] runners = new Task[threadCount + 1];
                runners[0] = Task.Delay(timeout);
                for (int i = 1; i <= threadCount; ++i)
                {
                    runners[i] = Task.Factory.StartNew(() => _run(token), token);
                }
                while (runners.Skip(1).Any(r => !r.IsCompleted))
                {
                    var id = Task.WaitAny(runners);
                    if (id == 0 || runners[id].IsFaulted)
                    {
                        try
                        {
                            source.Cancel();
                        }
                        catch { }
                        //Console.WriteLine(runners[id].Exception);
                        break;
                    }
                }
            }
            return _stats;
        }

        public double Percent
        {
            get { return _stats.Percent; }
        }

        private void _run(CancellationToken token)
        {
            var factory = new ProgramFactory(_options);
            var runnerNode = new ProgramRunner();
            //var runnerXS = new EvalProgramRunner(@"..\moddable\build\bin\win\release\xst_x64.exe", "-s");
            RuntimeInfo ri;

            while (!token.IsCancellationRequested && (ri = _stats.Add()) != null)
            {
                var smallSeed = Interlocked.Increment(ref _seed);
                var bigSeed = BinaryUtils.GenerateSeed(smallSeed);
                ri.Seed = BinaryUtils.ByteArrayToString(bigSeed);
                var p = factory.GenProgram(bigSeed);
                runnerNode.WriteProgram(p);
                runnerNode.ExecuteProgram(ri);
                if (!ri.Success)
                {
                    throw new InvalidProgramException();
                }
                /*runnerXS.WriteProgram(p);
                var xs = runnerXS.ExecuteProgram();
                xs.Output = xs.Output.Replace("\r", "");
                if(ri.Output != xs.Output)
                {
                    Console.WriteLine("NODE:");
                    Console.WriteLine(ri.Output);
                    Console.WriteLine("---------------------");
                    Console.WriteLine("XS:");
                    Console.WriteLine(xs.Output);
                    Console.WriteLine("---------------------");
                    Console.WriteLine($"Outputs differ with seed {ri.Seed}");
                    throw new InvalidOperationException();
                }*/
                if (_evalTest)
                {
                    runnerNode.WriteProgram(new SyntaxErrorProgram(p));
                    var se = runnerNode.ExecuteProgram();
                    ri.MatchSyntaxError = (se.Output == ri.Output);
                    ri.SyntaxErrorRuntime = se.Runtime / ri.Runtime;
                }
                Progress?.Invoke(this, EventArgs.Empty);
            }            
        }

        class SyntaxErrorProgram : IProgram
        {
            static readonly System.Text.RegularExpressions.Regex _eval = new System.Text.RegularExpressions.Regex("__eval\\(_=>eval\\(_\\),['\"]([/cb1/\\|=`\\+-a2\\+e84]{10})['\"]\\)");
            string _source;

            public SyntaxErrorProgram(IProgram parent)
            {
                using (var writer = new StringWriter())
                {
                    parent.WriteTo(writer);
                    writer.Flush();
                    _source = _eval.Replace(writer.ToString(), (System.Text.RegularExpressions.Match m) => $"('SyntaxError{m.Groups[1].Value}')");
                }
            }

            public void WriteTo(TextWriter w)
            {
                w.Write(_source);
            }
        }
    }
}
