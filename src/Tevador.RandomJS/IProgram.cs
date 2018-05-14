using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tevador.RandomJS
{
    public interface IProgram
    {
        int Execute(out string output, out string error);
        void WriteTo(System.IO.TextWriter w);
        byte[] Source { get; }
    }
}
