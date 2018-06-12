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

using System.IO;
using System.Diagnostics;

namespace Tevador.RandomJS.Run
{
    public class ExternalProgramRunner : ProgramRunnerBase
    {
        string _executable;
        string _arguments;
        string _sourceFile;

        public ExternalProgramRunner(string executable, string arguments = "")
        {
            _executable = executable;
            _arguments = arguments;
        }

        public override RuntimeInfo ExecuteProgram(RuntimeInfo ri)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = _executable;
            startInfo.Arguments = _arguments + " " +_sourceFile;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            Stopwatch sw = Stopwatch.StartNew();
            process.Start();
            process.WaitForExit();
            sw.Stop();
            ri.Success = process.ExitCode == 0;
            ri.Output = process.StandardOutput.ReadToEnd();
            ri.Runtime = sw.Elapsed.TotalSeconds;
            return ri;
        }

        public override void WriteProgram(IProgram program)
        {
            if (File.Exists(_sourceFile)) File.Delete(_sourceFile);
            _sourceFile = Path.GetTempFileName();
            using (var fs = File.OpenWrite(_sourceFile))
            {
                using(var writer = new StreamWriter(fs))
                {
                    program.WriteTo(writer);
                }
            }
        }
    }
}
