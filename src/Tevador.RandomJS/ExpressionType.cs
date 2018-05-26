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
    public enum ExpressionType : ulong
    {
        All = ~0UL,
        Literal = 1 << 0,
        AssignmentExpression = 1 << 1,
        VariableInvocationExpression = 1 << 2,
        FunctionInvocationExpression = 1 << 3,
        FunctionExpression = 1 << 4,
        UnaryExpression = 1 << 5,
        BinaryExpression = 1 << 6,
        TernaryExpression = 1 << 7,
        EvalExpression = 1 << 8,
        VariableExpression = 1 << 9,
        ObjectConstructorExpression = 1 << 10,
        ObjectSetExpression = 1 << 11,

        Function = FunctionExpression | FunctionInvocationExpression,
        NoVariable = Literal | FunctionInvocationExpression | FunctionExpression | EvalExpression | ObjectConstructorExpression,
        NoCall = Literal | AssignmentExpression | UnaryExpression | BinaryExpression | TernaryExpression | VariableExpression | ObjectSetExpression,
        Flat = Literal | VariableExpression | EvalExpression,
    }
}
