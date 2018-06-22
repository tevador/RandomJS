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

using System.Collections.Generic;
using System.IO;

namespace Tevador.RandomJS.Statements
{
    class TryCatchStatement : Statement
    {
        public readonly string CaughtErrorIdentifier = "__error";

        public Block TryBody { get; set; }

        //should be Blocks, but let's keep it simple
        public Statement CatchStatement { get; set; }
        public List<Statement> FinallyStatements { get; private set; } = new List<Statement>();

        public override void WriteTo(TextWriter w)
        {
            w.Write("try");
            TryBody.WriteTo(w);
            if (CatchStatement != null)
            {
                w.Write("catch(");
                w.Write(CaughtErrorIdentifier);
                w.Write("){");
                CatchStatement.WriteTo(w);
                w.Write("}");
            }
            w.Write("finally{");
            foreach(var stmt in FinallyStatements)
                stmt.WriteTo(w);
            w.Write("}");
        }
    }
}
