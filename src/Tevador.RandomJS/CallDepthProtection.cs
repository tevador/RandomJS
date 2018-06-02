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

using Tevador.RandomJS.Statements;

namespace Tevador.RandomJS
{
    class CallDepthProtection
    {
        static readonly string _depthVaribleName = "__depth";
        public static readonly string MaxDepthConstantName = "__maxDepth";

        readonly GlobalVariable _depthVarible = new GlobalVariable(_depthVaribleName, false, new Expressions.Literal("0"));
        readonly GlobalVariable _maxDepthConstant = new GlobalVariable(MaxDepthConstantName, true);

        public CallDepthProtection()
        {
            Check = new CDPCheck();
            Cleanup = new CDPCleanup();
        }

        public void AttachTo(IScope scope)
        {
            scope.Require(_depthVarible);
            scope.Require(_maxDepthConstant);
        }

        public Statement Check { get; private set; }
        public Statement Cleanup { get; private set; }

        class CDPCheck : Statement
        {
            public override void WriteTo(System.IO.TextWriter w)
            {
                w.Write("if(++{0}>{1})", _depthVaribleName, MaxDepthConstantName);
            }
        }

        class CDPCleanup : Statement
        {
            public override void WriteTo(System.IO.TextWriter w)
            {
                w.Write("--{0};", _depthVaribleName);
            }
        }
    }
}
