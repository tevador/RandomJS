using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tevador.RandomJS
{
    class BreakStatement : Statement
    {
        public override void WriteTo(TextWriter w)
        {
            w.Write("break;");
        }
    }
}
