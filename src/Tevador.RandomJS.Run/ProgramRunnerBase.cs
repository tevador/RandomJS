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

namespace Tevador.RandomJS.Run
{
    public abstract class ProgramRunnerBase
    {
        public abstract void WriteProgram(IProgram program);
        public abstract RuntimeInfo ExecuteProgram(RuntimeInfo ri);

        public RuntimeInfo ExecuteProgram()
        {
            return ExecuteProgram(new RuntimeInfo());
        }

        public static ProgramRunnerBase FromUri(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    return new ProgramRunner(uri.ToString());

                case "file":
                    if (uri.IsUnc)
                    {
                        return new ExternalProgramRunner(uri.Host, uri.LocalPath.Substring(3 + uri.Host.Length));
                    }
                    else
                    {
                        return new ExternalProgramRunner(uri.LocalPath);
                    }

                default:
                    throw new UriFormatException("Unsupported runnerUri");
            }
        }
    }
}
