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

namespace Tevador.RandomJS
{
    public enum StatementType : ulong
    {
        All = ~0UL,
        ReturnStatement = 1 << 0,
        BreakStatement = 1 << 1,
        AssignmentStatement = 1 << 2,
        ObjectSetStatement = 1 << 3,
        IfElseStatement = 1 << 4,
        ForLoopStatement = 1 << 5,
        //WhileLoop, TODO
        //DoWhileLoop, TODO
        BlockStatement = 1 << 6,
        VariableInvocationStatement = 1 << 7,

        Flat = ReturnStatement | BreakStatement | AssignmentStatement | ObjectSetStatement | VariableInvocationStatement,
        NoCall = All & ~VariableInvocationStatement,
    }
}
