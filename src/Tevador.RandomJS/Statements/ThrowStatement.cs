using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tevador.RandomJS.Expressions;

namespace Tevador.RandomJS.Statements
{
    class ThrowStatement : Statement
    {
        public Expression Value { get; set; }

        public override void WriteTo(TextWriter w)
        {
            w.Write("throw new ");
            w.Write(GlobalClass.RERR);
            w.Write("(");
            Value.WriteTo(w);
            w.Write(");");
        }

        public override bool IsTerminating
        {
            get { return true; }
        }
    }
}
