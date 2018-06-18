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
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Tevador.RandomJS.Run
{
    public class ExternalProgramRunner : ProgramRunnerBase
    {
        Process _process;
        string _executable;
        string _arguments;
        StringBuilder _output;
        StreamWriter _input;

        public ExternalProgramRunner(string executable, string arguments = "")
        {
            _executable = executable;
            _arguments = arguments;
            _output = new StringBuilder(1024);
            _startProcess();
        }

        private void _startProcess()
        {
            _process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = _executable;
            startInfo.Arguments = _arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            _process.StartInfo = startInfo;
            _process.Start();
            _input = new StreamWriter(_process.StandardInput.BaseStream, Encoding.ASCII);
        }

        public override RuntimeInfo ExecuteProgram(RuntimeInfo ri)
        {
            _output.Clear();
            var stream = _process.StandardOutput.BaseStream;
            int ch;
            Stopwatch sw = Stopwatch.StartNew();
            while ((ch = stream.ReadByte()) > 0)
            {
                _output.Append((char)ch);
            }
            sw.Stop();
            if (ch == -1)
            {
                ri.Success = false;
                Console.WriteLine("Stdout finished, waiting for process exit...");
                _process.WaitForExit();
                Console.WriteLine($"Exit code: {_process.ExitCode}");
                Console.WriteLine(_process.StandardError.ReadToEnd());
                _startProcess();
            }
            else
            {
                ri.Success = true;
            }
            ri.Output = _output.ToString();
            ri.Runtime = sw.Elapsed.TotalSeconds;
            return ri;
        }

        public override void WriteProgram(IProgram program)
        {
            program.WriteTo(_input);
            _input.Write('\0');
            _input.Flush();
        }
    }
}
