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

namespace Tevador.RandomJS.Expressions
{
    abstract class Expression : Statement
    {
        public int ExpressionDepth { get; private set; }

        public Expression ParentExpression
        {
            set
            {
                if (value != null)
                {
                    ExpressionDepth = value.ExpressionDepth + 1;
                }
            }
        }

        public static Expression Generate(IRandom rand, IScope scope, Expression parent, bool isReturn)
        {
            if (parent != null && parent.ExpressionDepth >= scope.Options.MaxExpressionDepth)
            {
                return Literal.Generate(rand, scope);
            }
            for(int i = 0; i < scope.Options.MaxExpressionAttempts; ++i)
            {
                Variable v;
                var type = scope.Options.Expressions.ChooseRandom(rand);
                switch (type)
                {
                    case ExpressionType.Literal:
                        if (scope.Options.PreferFuncParametersToLiterals && scope.InFunc && (v = rand.ChooseVariable(scope, false, true)) != null)
                        {
                            if (isReturn)
                            {
                                return new VariableExpression(v);
                            }
                            return VariableInvocationExpression.Generate(rand, scope, v, parent);
                        }
                        return Literal.Generate(rand, scope);

                    case ExpressionType.AssignmentExpression:
                        if (scope.VariableCounter > 0 && (v = rand.ChooseVariable(scope, true)) != null)
                        {
                            return AssignmentExpression.Generate(rand, scope, v, parent, true);
                        }
                        else
                        {
                            continue;
                        }

                    case ExpressionType.VariableInvocationExpression:
                        if (scope.VariableCounter > 0 && (v = rand.ChooseVariable(scope)) != null)
                        {
                            if (isReturn)
                            {
                                return new VariableExpression(v);
                            }
                            return VariableInvocationExpression.Generate(rand, scope, v, parent);
                        }
                        else
                        {
                            continue;
                        }

                    case ExpressionType.FunctionExpression:
                        if (isReturn) continue;
                        var stmtDepth = scope.StatementDepth;
                        if (stmtDepth >= scope.Options.MaxStatementDepth)
                        {
                            continue;
                        }
                        else
                        {
                            if (parent == null)
                            {
                                if (scope.InFunc && !scope.Options.AllowFunctionsInsideFunctions) continue;
                                return FunctionExpression.Generate(rand, scope, parent);
                            }
                            else if (rand.FlipCoin(scope.Options.FuncInvocationInExprChance))
                            {
                                return FunctionInvocationExpression.Generate(rand, scope, parent);
                            }
                            else
                            {
                                continue;
                            }
                        }

                    case ExpressionType.UnaryExpression:
                        return UnaryExpression.Generate(rand, scope, parent, isReturn);

                    case ExpressionType.BinaryExpression:
                        return BinaryExpression.Generate(rand, scope, parent, isReturn);

                    case ExpressionType.TernaryExpression:
                        return TernaryExpression.Generate(rand, scope, parent, isReturn);
                }
            }
            return Literal.Generate(rand, scope); //fall back to a Literal
        }
    }
}
