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

using System.Net;
using System.IO;
using System.Globalization;

namespace Tevador.RandomJS.Run
{
    public class ProgramRunner : ProgramRunnerBase
    {
        MemoryStream _programStream;
        StreamWriter _programWriter;
        int _port;

        public ProgramRunner(int port = 18111, int bufferSize = 256 * 1024)
        {
            _programStream = new MemoryStream(bufferSize);
            _programWriter = new StreamWriter(_programStream) { NewLine = "\n" };
            _port = port;
        }

        public override RuntimeInfo ExecuteProgram(RuntimeInfo ri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:" + _port);
            request.KeepAlive = false;
            request.Timeout = 15000;
            request.Method = "POST";
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(_programStream.GetBuffer(), 0, (int)_programStream.Length);
            reqStream.Close();
            WebResponse response = request.GetResponse();
            ri.Success = bool.Parse(response.Headers.Get("X-Success"));       
            ri.Runtime = double.Parse(response.Headers.Get("X-Execution-Time"), CultureInfo.InvariantCulture);
            if (ri.Success)
            {
                var xcc = response.Headers.Get("X-Complexity-Cyclomatic");
                if(xcc != null) ri.CyclomaticComplexity = int.Parse(xcc);
                var xch = response.Headers.Get("X-Complexity-Halstead");
                if(xch != null) ri.HalsteadDifficulty = double.Parse(xch, CultureInfo.InvariantCulture);
                var loc = response.Headers.Get("X-Logical-Lines");
                if (loc != null) ri.LinesOfCode = int.Parse(loc);
            }
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                ri.Output = reader.ReadToEnd();
            }
            return ri;
        }

        public override void WriteProgram(IProgram program)
        {
            _programStream.SetLength(0);
            program.WriteTo(_programWriter);
            _programWriter.Flush();
        }

        public int ProgramLength
        {
            get { return (int)_programStream.Length; }
        }

        public byte[] Buffer
        {
            get { return _programStream.GetBuffer(); }
        }
    }
}
