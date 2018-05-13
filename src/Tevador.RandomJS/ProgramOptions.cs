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

using Tevador.RandomJS.Operators;

namespace Tevador.RandomJS
{
    class ProgramOptions
    {
        public int GlobalVariablesCount { get; set; }
        public CallDepthProtection DepthProtection { get; set; }
        public LoopCyclesProtection CyclesProtection { get; set; }
        public int MaxExpressionDepth { get; set; }
        public int MaxStatementDepth { get; set; }
        public double ConstVariableChance { get; set; }
        public int MaxFunctionParameterCount { get; set; }
        public int MaxStringLiteralLength { get; set; }
        public int MaxStringVariableLength { get; set; }
        public double FuncInvocationInExprChance { get; set; }
        public bool AllowFunctionOverwriting { get; set; }
        public bool AllowFunctionsInsideFunctions { get; set; }
        public bool PreferFuncParametersToLiterals { get; set; }
        public int MaxExpressionAttempts { get; set; }
        public int FpMathPrecision { get; set; }

        public byte[] Seed { get; set; }
        
        public RandomTable<LiteralType> Literals { get; set; }
        public RandomTable<NumericLiteralType> NumericLiterals { get; set; }
        public RandomTable<ExpressionType> GlobalExpressions { get; set; }
        public RandomTable<ExpressionType> Expressions { get; set; }
        public RandomTable<AssignmentOperator> AssignmentOperators { get; set; }
        public RandomTable<UnaryOperator> UnaryOperators { get; set; }
        public RandomTable<BinaryOperator> BinaryOperators { get; set; }

        public override string ToString()
        {
            return "Seed: " + BinaryUtils.ByteArrayToString(Seed);
        }
    }
}
