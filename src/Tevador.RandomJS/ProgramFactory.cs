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

using Tevador.RandomJS.Expressions;
using Tevador.RandomJS.Operators;
using Tevador.RandomJS.Statements;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tevador.RandomJS
{
    public class ProgramFactory
    {
        private ProgramOptions _options;
        private IRandom _rand;
        private CallDepthProtection _depthProtection;
        private LoopCyclesProtection _cyclesProtection;

        public ProgramFactory()
            : this(new Xoshiro256Plus())
        {
        }

        internal ProgramFactory(Xoshiro256Plus random)
        {
            _rand = random;
            _options = ProgramOptions.FromXml();
            if (_options.EnableCallDepthProtection)
                _depthProtection = new CallDepthProtection();
            if (_options.EnableLoopCyclesProtection)
                _cyclesProtection = new LoopCyclesProtection();
        }

        public IProgram GenProgram(byte[] seed)
        {
            _rand.Seed(seed);
            var program = new Program();
            program.Seed = seed;
            _depthProtection?.AttachTo(program);
            _cyclesProtection?.AttachTo(program);
            int globalsCount = _options.GlobalVariablesCountRange.RandomValue(_rand);
            while (program.DeclaredVariables.Count < globalsCount)
            {
                var v = GenVariable(program, false, false);
                program.DeclaredVariables.Add(v);
                program.Statements.Add(v.Declaration);
            }
            program.PrintOrder = new List<Variable>(program.DeclaredVariables);
            _rand.Shuffle(program.PrintOrder);
            foreach (var v in program.PrintOrder)
            {
                program.Statements.Add(GenOutputStatement(program, v));
            }
            program.SetGlobalVariable(GlobalFunction.STRL.References.Name, _options.MaxStringLengthRange.RandomValue(_rand));
            program.SetGlobalVariable(GlobalFunction.PREC.References.Name, _options.MathPrecisionRange.RandomValue(_rand));
            program.SetGlobalVariable(GlobalFunction.STRL.References.Name, _options.MaxStringLengthRange.RandomValue(_rand));
            program.SetGlobalVariable(CallDepthProtection.MaxDepthConstantName, _options.MaxCallDepthRange.RandomValue(_rand));
            program.SetGlobalVariable(LoopCyclesProtection.MaxCyclesConstantName, _options.MaxLoopCyclesRange.RandomValue(_rand));
            return program;
        }

        #region Codegen methods

        internal ReturnStatement GenReturnStatement(IScope scope, Expression expr = null)
        {
            expr = expr ?? GenExpression(scope, null, true);
            return new ReturnStatement(scope, new NonEmptyExpression(expr, GenLiteral()), _depthProtection);
        }

        internal Statement GenStatement(IScope scope, Statement parent)
        {
            for (int i = 0; i < _options.MaxStatementAttempts; ++i)
            {
                var type = _options.Statements.ChooseRandom(_rand);
                Variable v;
                switch (type)
                {
                    case StatementType.AssignmentStatement:
                        if (scope.VariableCounter > 0 && (v = _rand.ChooseVariable(scope, _options.VariableOptions | VariableOptions.ForWriting)) != null)
                        {
                            return new AssignmentStatement(GenAssignmentExpression(scope, v, null, false));
                        }
                        else
                        {
                            continue;
                        }

                    case StatementType.BreakStatement:
                        if (scope.HasBreak)
                        {
                            return new BreakStatement();
                        }
                        else
                        {
                            continue;
                        }

                    case StatementType.ReturnStatement:
                        if (scope.InFunc)
                        {
                            return GenReturnStatement(scope);
                        }
                        else
                        {
                            continue;
                        }

                    case StatementType.IfElseStatement:
                        if (scope.StatementDepth >= _options.MaxStatementDepth || parent.StatementDepth >= _options.MaxStatementDepth)
                            continue;
                        return GenIfElseStatement(scope, parent);

                    case StatementType.VariableInvocationStatement:
                        if (scope.VariableCounter > 0 && (v = _rand.ChooseVariable(scope)) != null)
                        {
                            if (!scope.HasBreak || _options.AllowFunctionInvocationInLoop)
                            {
                                var expr = GenVariableInvocationExpression(scope, v, null);
                                return new ExpressionStatement<VariableInvocationExpression>(expr);
                            }
                        }
                        continue;

                    case StatementType.BlockStatement:
                        if (parent is Block || scope.StatementDepth >= _options.MaxStatementDepth || parent.StatementDepth >= _options.MaxStatementDepth)
                            continue;
                        return GenBlock(scope);

                    case StatementType.ForLoopStatement:
                        return GenForLoop(scope, parent);
                }
            }
            return new EmptyStatement();
        }

        internal ForLoop GenForLoop(IScope scope, Statement parent)
        {
            var fl = new ForLoop(scope);
            Variable i = fl.IteratorVariable = GenVariable(fl, false, true);
            i.Initializer = GenNumericLiteral(NumericLiteralType.SmallInteger);
            bool decrease = _rand.FlipCoin();
            Expression iteratorExpr = null;
            if (_rand.FlipCoin(_options.ForLoopVariableBoundsChance))
            {
                Variable v = _rand.ChooseVariable(scope);
                if (v != null)
                    iteratorExpr = new NumericExpression(fl, new VariableExpression(v), GenNumericLiteral());
            }
            if(iteratorExpr == null)
            {
                iteratorExpr = GenNumericLiteral();
            }
            iteratorExpr = new BinaryExpression(null)
            {
                Operator = _rand.FlipCoin() ? BinaryOperator.Less : BinaryOperator.Greater,
                Lhs = new VariableExpression(i),
                Rhs = iteratorExpr
            };
            fl.Control = new LoopControlExpression(iteratorExpr, _cyclesProtection);
            var op = _options.AssignmentInForLoop.ChooseRandom(_rand);
            Expression rhs = null;
            if (!op.Has(OperatorRequirement.WithoutRhs))
            {
                rhs = GenNumericLiteral();
                if (op.Has(OperatorRequirement.RhsNonzero))
                {
                    rhs = new NonZeroExpression(fl, rhs);
                }
            }
            fl.IteratorExpression = new AssignmentExpression(null)
            {
                Operator = op,
                Variable = i,
                Rhs = rhs
            };
            fl.Body = GenStatement(fl, fl);
            return fl;
        }

        internal Expression GenExpression(IScope scope, Expression parent, bool isReturn)
        {
            for (int i = 0; i < _options.MaxExpressionAttempts; ++i)
            {
                Variable v = null;
                var type = _options.Expressions.ChooseRandom(_rand);
                switch (type)
                {
                    case ExpressionType.Literal:
                        return GenLiteral();

                    case ExpressionType.AssignmentExpression:
                        if (scope.VariableCounter > 0 && (v = _rand.ChooseVariable(scope, _options.VariableOptions | VariableOptions.ForWriting)) != null)
                        {
                            return GenAssignmentExpression(scope, v, parent, isReturn);
                        }
                        else
                        {
                            continue;
                        }

                    case ExpressionType.VariableInvocationExpression:
                        if(scope.InFunc && _options.PreferFuncParameters)
                        {
                            v = _rand.ChooseVariable(scope, _options.VariableOptions | VariableOptions.ParametersOnly);
                        }
                        if (v == null && scope.VariableCounter > 0)
                        {
                            v = _rand.ChooseVariable(scope);
                        }
                        if(v != null)
                        {
                            if (isReturn || (scope.HasBreak && !_options.AllowFunctionInvocationInLoop) || (parent != null && parent.ExpressionDepth >= _options.MaxExpressionDepth))
                            {
                                return new VariableExpression(v);
                            }
                            return GenVariableInvocationExpression(scope, v, parent);
                        }
                        continue;

                    case ExpressionType.FunctionInvocationExpression:
                        if (scope.StatementDepth >= _options.MaxStatementDepth || (parent != null && parent.ExpressionDepth >= _options.MaxExpressionDepth))
                        {
                            continue;
                        }
                        else if((!scope.InFunc || _options.AllowNestedFunctions) && (!scope.HasBreak || _options.AllowFunctionInvocationInLoop))
                        {
                            return GenFunctionInvocationExpression(scope, parent);
                        }
                        continue;

                    case ExpressionType.FunctionExpression:
                        if (scope.StatementDepth >= _options.MaxStatementDepth)
                        {
                            continue;
                        }
                        else
                        {
                            if (parent == null)
                            {
                                if (scope.InFunc && !_options.AllowNestedFunctions) continue;
                                return GenFunctionExpression(scope, parent);
                            }
                            else
                            {
                                continue;
                            }
                        }

                    case ExpressionType.UnaryExpression:
                        if ((parent != null && parent.ExpressionDepth >= _options.MaxExpressionDepth))
                            continue;
                        return GenUnaryExpression(scope, parent, isReturn);

                    case ExpressionType.BinaryExpression:
                        if ((parent != null && parent.ExpressionDepth >= _options.MaxExpressionDepth))
                            continue;
                        return GenBinaryExpression(scope, parent, isReturn);

                    case ExpressionType.TernaryExpression:
                        if ((parent != null && parent.ExpressionDepth >= _options.MaxExpressionDepth))
                            continue;
                        return GenTernaryExpression(scope, parent, isReturn);
                }
            }
            return GenLiteral(); //fall back to a Literal
        }

        internal FunctionInvocationExpression GenFunctionInvocationExpression(IScope scope, Expression parent)
        {
            var expr = GenFunctionExpression(scope, parent);
            FunctionInvocationExpression fi = new FunctionInvocationExpression(parent);
            fi.Function = GenFunctionExpression(scope, fi);
            int paramCount = _options.FunctionParametersCountRange.RandomValue(_rand);
            while (paramCount-- > 0)
            {
                fi.Parameters.Add(GenExpression(scope, fi, false));
            }
            return fi;
        }

        internal AssignmentExpression GenAssignmentExpression(IScope scope, Variable v, Expression parent, bool isReturn = false)
        {
            AssignmentOperator op = _options.AssignmentOperators.ChooseRandom(_rand);
            AssignmentExpression ae = new AssignmentExpression(parent) { Operator = op };
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                scope.Require(GlobalFunction.CALC);
                ae.DefaultValue = GenNumericLiteral();
            }
            if (!op.Has(OperatorRequirement.WithoutRhs))
            {
                Expression expr = GenExpression(scope, ae, isReturn);
                if (op.Has(OperatorRequirement.NumericOnly))
                {
                    expr = new NumericExpression(scope, expr, GenNumericLiteral());
                }
                if (op.Has(OperatorRequirement.RhsNonzero))
                {
                    expr = new NonZeroExpression(scope, expr);
                }
                ae.Rhs = expr;
            }
            ae.Variable = v;
            return ae;
        }

        internal NumericLiteral GenNumericLiteral()
        {
            return GenNumericLiteral(_options.NumericLiterals.ChooseRandom(_rand));
        }

        internal NumericLiteral GenNumericLiteral(NumericLiteralType type, bool allowNegative = true)
        {
            StringBuilder sb = new StringBuilder(35);
            switch (type)
            {
                case NumericLiteralType.Boolean:
                    if (_rand.FlipCoin())
                        sb.Append("true");
                    else
                        sb.Append("false");
                    break;

                case NumericLiteralType.SmallInteger:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    sb.Append(_rand.GenInt(_options.MaxSmallInteger + 1));
                    break;

                case NumericLiteralType.BinaryInteger:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    sb.Append("0b");
                    _rand.GenString(sb, 32, RandomExtensions.BinaryChars);
                    break;

                case NumericLiteralType.OctalInteger:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    sb.Append("0o");
                    _rand.GenString(sb, 10, RandomExtensions.OctalChars);
                    break;

                case NumericLiteralType.HexInteger:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    sb.Append("0x");
                    _rand.GenString(sb, 8, RandomExtensions.HexChars);
                    break;

                case NumericLiteralType.FixedFloat:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    _rand.GenString(sb, 5, RandomExtensions.DecimalChars, false);
                    sb.Append('.');
                    _rand.GenString(sb, 5, RandomExtensions.DecimalChars, true);
                    break;

                case NumericLiteralType.ExpFloat:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    sb.Append(RandomExtensions.DecimalChars[_rand.GenInt(10)]);
                    sb.Append('.');
                    _rand.GenString(sb, 5, RandomExtensions.DecimalChars, true);
                    sb.Append('e');
                    if (_rand.FlipCoin()) sb.Append('-');
                    sb.Append(_rand.GenInt(_options.MaxSmallInteger + 1));
                    break;

                default:
                    if (allowNegative && _rand.FlipCoin()) sb.Append('-');
                    _rand.GenString(sb, 9, RandomExtensions.DecimalChars, false);
                    break;
            }
            return new NumericLiteral(sb.ToString());
        }

        internal VariableInvocationExpression GenVariableInvocationExpression(IScope scope, Variable v, Expression parent)
        {
            var invk = new VariableInvocationExpression(scope, v, parent);
            int paramCount = _options.FunctionParametersCountRange.RandomValue(_rand);
            while (paramCount-- > 0)
            {
                invk.Parameters.Add(GenExpression(scope, invk, false));
            }
            return invk;
        }

        internal UnaryExpression GenUnaryExpression(IScope scope, Expression parent, bool isReturn)
        {
            UnaryExpression ue = new UnaryExpression(parent);
            UnaryOperator op = _options.UnaryOperators.ChooseRandom(_rand);
            ue.Operator = op;
            Expression expr = GenExpression(scope, ue, isReturn);
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                expr = new NumericExpression(scope, expr, GenNumericLiteral());
            }
            if (op.Has(OperatorRequirement.RhsNonnegative))
            {
                expr = new NonNegativeExpression(scope, expr);
            }
            if (op.Has(OperatorRequirement.RhsNonzero))
            {
                expr = new NonZeroExpression(scope, expr);
            }
            if (op.Has(OperatorRequirement.LimitedPrecision))
            {
                scope.Require(GlobalFunction.PREC);
            }
            ue.Value = expr;
            return ue;
        }

        internal BinaryExpression GenBinaryExpression(IScope scope, Expression parent, bool isReturn)
        {
            BinaryExpression be = new BinaryExpression(parent);
            var op = _options.BinaryOperators.ChooseRandom(_rand);
            be.Operator = op;
            var lhs = GenExpression(scope, be, isReturn);
            var rhs = GenExpression(scope, be, isReturn);
            if (op.Has(OperatorRequirement.StringLengthLimit))
            {
                scope.Require(GlobalFunction.STRL);
            }
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                lhs = new NumericExpression(scope, lhs, GenNumericLiteral());
                rhs = new NumericExpression(scope, rhs, GenNumericLiteral());
                if (op.Has(OperatorRequirement.RhsNonzero))
                {
                    rhs = new NonZeroExpression(scope, rhs);
                }
            }
            be.Lhs = lhs;
            be.Rhs = rhs;
            return be;
        }

        internal TernaryExpression GenTernaryExpression(IScope scope, Expression parent, bool isReturn)
        {
            TernaryExpression te = new TernaryExpression(parent);
            te.Condition = GenExpression(scope, te, isReturn);
            te.TrueExpr = GenExpression(scope, te, isReturn);
            te.FalseExpr = GenExpression(scope, te, isReturn);
            return te;
        }

        internal Literal GenLiteral()
        {
            switch (_options.Literals.ChooseRandom(_rand))
            {
                case LiteralType.Numeric:
                    return GenNumericLiteral();

                default:
                    int stringLength = _options.StringLiteralLengthRange.RandomValue(_rand);
                    return new Literal(_rand.GenStringLiteral(stringLength));
            }
        }

        internal FunctionExpression GenFunctionExpression(IScope scope, Expression parent = null)
        {
            var func = new FunctionExpression(scope, parent);
            int paramCount = _options.FunctionParametersCountRange.RandomValue(_rand);
            for (int i = 0; i < paramCount; ++i)
            {
                func.Parameters.Add(GenVariable(func, true, false));
            }
            //func._unusedVariables.AddRange(func.Parameters);
            func.DefaultReturnValue = GenExpression(func, null, true);
            func.Body = GenBlock(func);
            return func;
        }

        internal Variable GenVariable(IScope scope, bool isParameter, bool isLoopCounter)
        {
            var v = new Variable();
            v.Name = Variable.GetVariableName(scope.VariableCounter++);
            v.Parent = scope;
            v.IsParameter = isParameter;
            v.IsLoopCounter = isLoopCounter;
            if (!v.IsParameter && !v.IsLoopCounter && _rand.FlipCoin(_options.ConstVariableChance))
            {
                v.IsConstant = true;
            }
            if (!v.IsParameter && !v.IsLoopCounter)
            {
                v.Initializer = GenExpression(scope, scope as Expression, false);
            }
            return v;
        }

        internal IfElseStatement GenIfElseStatement(IScope scope, Statement parent)
        {
            var ife = new IfElseStatement(parent);
            ife.Condition = GenExpression(scope, null, false);
            ife.Body = GenStatement(scope, ife);
            if (_rand.FlipCoin(_options.ElseChance))
            {
                ife.ElseBody = GenStatement(scope, ife);
            }
            return ife;
        }

        internal Block GenBlock(IScope scope)
        {
            var block = new Block(scope);
            var func = scope as FunctionExpression;
            if (func != null)
            {
                if (_depthProtection != null)
                {
                    block.Statements.Add(_depthProtection.Check);
                    block.Statements.Add(GenReturnStatement(block, func.DefaultReturnValue));
                }
                int localVariablesCount = _options.LocalVariablesCountRange.RandomValue(_rand);
                while (localVariablesCount-- > 0)
                {
                    var v = GenVariable(block, false, false);
                    block.DeclaredVariables.Add(v);
                    block.Statements.Add(v.Declaration);
                }
            }
            int statementsCount = _options.BlockStatementsRange.RandomValue(_rand);
            while(statementsCount-- > 0)
            {
                var stmt = GenStatement(block, block);
                block.Statements.Add(stmt);
                if (stmt.IsTerminating)
                    break;
            }
            if (func != null && (block.Statements.Count == 0 || !(block.Statements.Last() is ReturnStatement)))
            {
                block.Statements.Add(GenReturnStatement(block));
            }
            return block;
        }

        internal OutputStatement GenOutputStatement(Program program, Variable v)
        {
            program.Require(GlobalFunction.PRNT);
            var os = new OutputStatement() { Value = GenVariableInvocationExpression(program, v, null) };
            return os;
        }

        #endregion
    }
}
