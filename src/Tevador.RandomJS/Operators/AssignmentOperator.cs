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

namespace Tevador.RandomJS.Operators
{
    public sealed class AssignmentOperator : Operator
    {
        public readonly static AssignmentOperator Basic = new AssignmentOperator("=");
        public readonly static AssignmentOperator Add = new AssignmentOperator("+=");
        public readonly static AssignmentOperator Sub = new AssignmentOperator("-=", OperatorRequirement.NumericOnly);
        public readonly static AssignmentOperator Mul = new AssignmentOperator("*=", OperatorRequirement.NumericOnly);
        public readonly static AssignmentOperator Div = new AssignmentOperator("/=", OperatorRequirement.NumericOnly | OperatorRequirement.RhsNonzero);
        public readonly static AssignmentOperator Mod = new AssignmentOperator("%=", OperatorRequirement.NumericOnly | OperatorRequirement.RhsNonzero);

        public readonly static AssignmentOperator PreInc = new AssignmentOperator("++", OperatorRequirement.NumericOnly | OperatorRequirement.Prefix | OperatorRequirement.WithoutRhs);
        public readonly static AssignmentOperator PostInc = new AssignmentOperator("++", OperatorRequirement.NumericOnly | OperatorRequirement.WithoutRhs);
        public readonly static AssignmentOperator PreDec = new AssignmentOperator("--", OperatorRequirement.NumericOnly | OperatorRequirement.Prefix | OperatorRequirement.WithoutRhs);
        public readonly static AssignmentOperator PostDec = new AssignmentOperator("--", OperatorRequirement.NumericOnly | OperatorRequirement.WithoutRhs);

        private AssignmentOperator(string symbol, OperatorRequirement flag = OperatorRequirement.None)
            : base(symbol, flag)
        {

        }
    }
}
