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
        private VariableSelector _varSelector;

        public ProgramFactory()
            : this(ProgramOptions.FromXml(), new Xoshiro256Plus())
        {
        }

        public ProgramFactory(ProgramOptions options)
            : this(options, new Xoshiro256Plus())
        {
        }

        internal ProgramFactory(ProgramOptions options, IRandom random)
        {
            _rand = random;
            _options = options;
            _options.Validate();
            if (_options.EnableCallDepthProtection)
                _depthProtection = new CallDepthProtection();
            if (_options.EnableLoopCyclesProtection)
                _cyclesProtection = new LoopCyclesProtection();
            _varSelector = new VariableSelector(_rand, _options.VariableSelectorScopeFactor);
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
                var v = GenVariable(program);
                program.DeclaredVariables.Add(v);
                program.Statements.Add(v.Declaration);
            }
            program.PrintOrder = new List<Variable>(program.DeclaredVariables);
            _rand.Shuffle(program.PrintOrder);
            foreach (var v in program.PrintOrder)
            {
                program.Statements.Add(GenOutputStatement(program, v));
            }
            if (program.IsDefined(GlobalVariable.ESUM))
                program.Statements.Add(GenOutputStatement(program, new VariableExpression(GlobalVariable.ESUM)));
            if (program.IsDefined(GlobalVariable.CSUM))
                program.Statements.Add(GenOutputStatement(program, new VariableExpression(GlobalVariable.CSUM)));
            program.SetGlobalVariable(GlobalVariable.STRL.Name, _options.MaxStringLengthRange.RandomValue(_rand));
            program.SetGlobalVariable(GlobalVariable.PREC.Name, _options.MathPrecisionRange.RandomValue(_rand));
            program.SetGlobalVariable(GlobalVariable.STRL.Name, _options.MaxStringLengthRange.RandomValue(_rand));
            program.SetGlobalVariable(CallDepthProtection.MaxDepth.Name, _options.MaxCallDepthRange.RandomValue(_rand));
            program.SetGlobalVariable(LoopCyclesProtection.MaxCycles.Name, _options.MaxLoopCyclesRange.RandomValue(_rand));
            return program;
        }

        #region Codegen methods

        internal ObjectSetStatement GenObjectSetStatement(IScope scope)
        {
            return new ObjectSetStatement(GenObjectSetExpression(scope, _options.MaxExpressionDepth));
        }

        internal ReturnStatement GenReturnStatement(IScope scope, Expression expr = null)
        {
            expr = expr ?? GenExpression(scope, 0);
            if(!(expr is Literal))
            {
                expr = new NonEmptyExpression(expr, GenLiteral(scope));
            }
            return new ReturnStatement(expr);
        }

        internal Statement GenStatement(IScope scope, Statement parent, int maxDepth, StatementType list = StatementType.All)
        {
            if (maxDepth <= 0)
            {
                list &= StatementType.Flat;
            }
            if(scope.VariableCounter == 0)
            {
                list &= ~StatementType.AssignmentStatement;
            }
            if (!scope.HasBreak)
            {
                list &= ~StatementType.BreakStatement;
            }
            else if (!_options.AllowFunctionInvocationInLoop)
            {
                list &= StatementType.NoCall;
            }
            if (scope.FunctionDepth == 0)
            {
                list &= StatementType.ReturnStatement;
            }

                var type = _options.Statements.ChooseRandom(_rand, list);
                Variable v;
                switch (type)
                {
                    case StatementType.AssignmentStatement:
                        if ((v = _varSelector.ChooseVariable(scope, VariableOptions.ForWriting)) != null)
                        {
                            return new AssignmentStatement(GenAssignmentExpression(scope, v, _options.MaxExpressionDepth));
                        }
                        break;

                    case StatementType.BreakStatement:
                        return new BreakStatement();

                case StatementType.ObjectSetStatement:
                    return GenObjectSetStatement(scope);

                    case StatementType.ReturnStatement:
                        return GenReturnStatement(scope);

                    case StatementType.IfElseStatement:
                        return GenIfElseStatement(scope, parent, maxDepth - 1);

                    case StatementType.VariableInvocationStatement:
                        if ((v = _varSelector.ChooseVariable(scope)) != null)
                        {
                            var expr = GenVariableInvocationExpression(scope, v, _options.MaxExpressionDepth);
                            return new ExpressionStatement<VariableInvocationExpression>(expr);
                        }
                        break;

                    case StatementType.BlockStatement:
                        return GenBlock(scope, maxDepth - 1);

                    case StatementType.ForLoopStatement:
                        return GenForLoop(scope, parent, maxDepth - 1);

                case StatementType.ThrowStatement:
                    return GenThrowStatement(scope);
                }
            return new EmptyStatement();
        }

        internal ThrowStatement GenThrowStatement(IScope scope)
        {
            scope.Require(GlobalClass.RERR);
            var th = new ThrowStatement();
            th.Value = GenExpression(scope, 2);
            return th;
        }

        internal ForLoop GenForLoop(IScope scope, Statement parent, int maxDepth)
        {
            var fl = new ForLoop(scope);
            Variable i = fl.IteratorVariable = GenVariable(fl, false, true);
            i.Initializer = GenNumericLiteral(NumericLiteralType.SmallInteger);
            bool decrease = _rand.FlipCoin();
            Expression iteratorExpr = null;
            if (_rand.FlipCoin(_options.ForLoopVariableBoundsChance))
            {
                Variable v = _varSelector.ChooseVariable(scope);
                if (v != null)
                    iteratorExpr = new NumericExpression(fl, new VariableExpression(v), GenNumericLiteral());
            }
            if(iteratorExpr == null)
            {
                iteratorExpr = GenNumericLiteral();
            }
            iteratorExpr = new BinaryExpression()
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
            fl.IteratorExpression = new AssignmentExpression()
            {
                Operator = op,
                Variable = i,
                Rhs = rhs
            };
            fl.Body = GenStatement(fl, fl, maxDepth - 1, StatementType.All & ~StatementType.ReturnStatement & ~StatementType.ThrowStatement);
            return fl;
        }

        internal ObjectSetExpression GenObjectSetExpression(IScope scope, int maxDepth)
        {
            scope.Require(GlobalFunction.OBJS);
            var ose = new ObjectSetExpression();
            Variable v = _varSelector.ChooseVariable(scope);
            ose.Target = v != null ? (Expression)new VariableExpression(v) : GenObjectLiteral(scope, _options.MaxObjectLiteralDepth);
            ose.Value = GenExpression(scope, maxDepth - 1);
            ose.Property = Variable.GetVariableName(_rand.GenInt(_options.ObjectSetPropertyCount));
            return ose;
        }

        internal Expression GenExpression(IScope scope, int maxDepth, ExpressionType list = ExpressionType.All)
        {
            if(maxDepth <= 0)
            {
                list &= ExpressionType.Flat;
            }
            if(scope.FunctionDepth >= _options.MaxFunctionDepth)
            {
                list &= ~ExpressionType.Function;
            }
            if(scope.VariableCounter == 0)
            {
                list &= ExpressionType.NoVariable;
            }
            if(scope.HasBreak && !_options.AllowFunctionInvocationInLoop)
            {
                list &= ExpressionType.NoCall;
            }

            Variable v = null;
            var type = _options.Expressions.ChooseRandom(_rand, list);
            switch (type)
            {
                case ExpressionType.Literal:
                    return GenLiteral(scope);

                case ExpressionType.AssignmentExpression:
                    if ((v = _varSelector.ChooseVariable(scope, VariableOptions.ForWriting)) != null)
                    {
                        return GenAssignmentExpression(scope, v, maxDepth - 1);
                    }
                    break;

                case ExpressionType.VariableInvocationExpression:
                    if((v = _varSelector.ChooseVariable(scope)) != null)
                    {
                        return GenVariableInvocationExpression(scope, v, maxDepth - 1);
                    }
                    break;

                case ExpressionType.VariableExpression:
                    if ((v = _varSelector.ChooseVariable(scope)) != null)
                    {
                        return new VariableExpression(v);
                    }
                    break;

                case ExpressionType.EvalExpression:
                    return GenEvalExpression(scope);

                case ExpressionType.ObjectSetExpression:
                    return GenObjectSetExpression(scope, maxDepth - 1);

                case ExpressionType.ObjectConstructorExpression:
                    return GenObjectConstructorExpression(scope, maxDepth - 1);

                case ExpressionType.FunctionInvocationExpression:
                    return GenFunctionInvocationExpression(scope, maxDepth - 1);

                case ExpressionType.FunctionExpression:
                    return GenFunctionExpression(scope);

                case ExpressionType.UnaryExpression:
                    return GenUnaryExpression(scope, maxDepth - 1);

                case ExpressionType.BinaryExpression:
                    return GenBinaryExpression(scope, maxDepth - 1);

                case ExpressionType.TernaryExpression:
                    return GenTernaryExpression(scope, maxDepth - 1);
            }
            return GenLiteral(scope); //fall back to a Literal
        }

        internal FunctionInvocationExpression GenFunctionInvocationExpression(IScope scope, int maxDepth)
        {
            FunctionInvocationExpression fi = new FunctionInvocationExpression();
            fi.Function = GenFunctionExpression(scope);
            GenVariableInvocationExpression(fi, scope, maxDepth);
            return fi;
        }

        internal AssignmentExpression GenAssignmentExpression(IScope scope, Variable v, int maxDepth)
        {
            AssignmentOperator op = _options.AssignmentOperators.ChooseRandom(_rand);
            AssignmentExpression ae = new AssignmentExpression() { Operator = op };
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                scope.Require(GlobalFunction.CALC);
                ae.DefaultValue = GenNumericLiteral();
            }
            if (!op.Has(OperatorRequirement.WithoutRhs))
            {
                Expression expr = GenExpression(scope, maxDepth);
                if (op.Has(OperatorRequirement.NumericOnly) && !expr.IsNumeric)
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

        internal Literal GenLiteral(IScope scope)
        {
            return GenLiteral(scope, _options.MaxObjectLiteralDepth);
        }

        internal Literal GenLiteral(IScope scope, int maxDepth, LiteralType list = LiteralType.All)
        {
            if (maxDepth <= 0)
                list &= ~LiteralType.Object;
            switch (_options.Literals.ChooseRandom(_rand, list))
            {
                case LiteralType.Numeric:
                    return GenNumericLiteral();

                case LiteralType.Object:
                    return GenObjectLiteral(scope, maxDepth);

                default:
                    int stringLength = _options.StringLiteralLengthRange.RandomValue(_rand);
                    return new Literal(_rand.GenStringLiteral(stringLength));
            }
        }

        internal ObjectLiteral GenObjectLiteral(IScope scope, int maxDepth)
        {
            scope.Require(GlobalOverride.OVOF);
            scope.Require(GlobalOverride.OTST);
            var ol = new ObjectLiteral();
            int propertiesCount = _options.ObjectLiteralSizeRange.RandomValue(_rand);
            while(propertiesCount-- > 0)
            {
                ol.Values.Add(GenLiteral(scope, maxDepth - 1));
            }
            return ol;
        }

        internal NumericLiteral GenNumericLiteral()
        {
            return GenNumericLiteral(_options.NumericLiterals.ChooseRandom(_rand));
        }

        internal NumericLiteral GenNumericLiteral(NumericLiteralType type)
        {
            StringBuilder sb = new StringBuilder(37);
            bool negative = false;
            switch (type)
            {
                case NumericLiteralType.Boolean:
                    if (_rand.FlipCoin())
                        sb.Append("true");
                    else
                        sb.Append("false");
                    break;

                case NumericLiteralType.SmallInteger:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    _rand.GenString(sb, 2, RandomExtensions.DecimalChars, false);
                    break;

                case NumericLiteralType.BinaryInteger:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    sb.Append("0b");
                    _rand.GenString(sb, 32, RandomExtensions.BinaryChars);
                    break;

                case NumericLiteralType.OctalInteger:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    sb.Append("0o");
                    _rand.GenString(sb, 10, RandomExtensions.OctalChars);
                    break;

                case NumericLiteralType.HexInteger:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    sb.Append("0x");
                    _rand.GenString(sb, 8, RandomExtensions.HexChars);
                    break;

                case NumericLiteralType.FixedFloat:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    _rand.GenString(sb, 5, RandomExtensions.DecimalChars, false);
                    sb.Append('.');
                    _rand.GenString(sb, 5, RandomExtensions.DecimalChars, true);
                    break;

                case NumericLiteralType.ExpFloat:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    sb.Append(RandomExtensions.DecimalChars[_rand.GenInt(10)]);
                    sb.Append('.');
                    _rand.GenString(sb, 5, RandomExtensions.DecimalChars, true);
                    sb.Append('e');
                    if (_rand.FlipCoin()) sb.Append('-');
                    _rand.GenString(sb, 2, RandomExtensions.DecimalChars, true);
                    break;

                default:
                    if (_rand.FlipCoin())
                    {
                        sb.Append("(-");
                        negative = true;
                    }
                    _rand.GenString(sb, 9, RandomExtensions.DecimalChars, false);
                    break;
            }
            if (negative) sb.Append(')');
            return new NumericLiteral(sb.ToString());
        }

        internal EvalExpression GenEvalExpression(IScope scope)
        {
            scope.Require(GlobalFunction.EVAL);
            var ee = new EvalExpression();
            ee.Code = _rand.GenEvalString(_options.EvalStringLength.RandomValue(_rand));
            return ee;
        }

        internal VariableInvocationExpression GenVariableInvocationExpression(IScope scope, IVariable v, int maxDepth)
        {
            var invk = new VariableInvocationExpression(scope, v);
            return GenVariableInvocationExpression(invk, scope, maxDepth);
        }

        internal VariableInvocationExpression GenVariableInvocationExpression(VariableInvocationExpression invk, IScope scope, int maxDepth)
        {
            invk.InvokeFunction = scope.FunctionDepth > 0 ? GlobalFunction.INVK : GlobalFunction.INVC;
            scope.Require(invk.InvokeFunction);
            int paramCount = _options.FunctionParametersCountRange.RandomValue(_rand);
            while (paramCount-- > 0)
            {
                invk.Parameters.Add(GenExpression(scope, maxDepth, ExpressionType.All & ~ExpressionType.FunctionExpression));
            }
            return invk;
        }

        internal UnaryExpression GenUnaryExpression(IScope scope, int maxDepth)
        {
            UnaryExpression ue = new UnaryExpression();
            UnaryOperator op = _options.UnaryOperators.ChooseRandom(_rand);
            ue.Operator = op;
            Expression expr = GenExpression(scope, maxDepth);
            if (op.Has(OperatorRequirement.NumericOnly) && !expr.IsNumeric)
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

        internal BinaryExpression GenBinaryExpression(IScope scope, int maxDepth)
        {
            BinaryExpression be = new BinaryExpression();
            var op = _options.BinaryOperators.ChooseRandom(_rand);
            be.Operator = op;
            var lhs = GenExpression(scope, maxDepth);
            var rhs = GenExpression(scope, maxDepth);
            if (op.Has(OperatorRequirement.StringLengthLimit))
            {
                scope.Require(GlobalFunction.STRL);
            }
            if (op.Has(OperatorRequirement.NumericOnly))
            {
                if(!lhs.IsNumeric)
                    lhs = new NumericExpression(scope, lhs, GenNumericLiteral());
                if (!rhs.IsNumeric)
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

        internal TernaryExpression GenTernaryExpression(IScope scope, int maxDepth)
        {
            TernaryExpression te = new TernaryExpression();
            te.Condition = GenExpression(scope, maxDepth);
            te.TrueExpr = GenExpression(scope, maxDepth);
            te.FalseExpr = GenExpression(scope, maxDepth);
            return te;
        }

        internal FunctionExpression GenFunctionExpression(IScope scope)
        {
            scope.Require(GlobalOverride.FTST);
            if (_options.FunctionValueOfOverride)
                scope.Require(GlobalOverride.FVOF);
            var func = new FunctionExpression(scope);
            int paramCount = _options.FunctionParametersCountRange.RandomValue(_rand);
            for (int i = 0; i < paramCount; ++i)
            {
                func.Parameters.Add(GenVariable(func, true));
            }
            //func._unusedVariables.AddRange(func.Parameters);
            func.DefaultReturnValue = GenExpression(func, 0, ExpressionType.VariableExpression | ExpressionType.Literal);
            func.Body = GenFunctionBody(func);
            return func;
        }

        internal ObjectConstructorExpression GenObjectConstructorExpression(IScope scope, int maxDepth)
        {
            scope.Require(GlobalFunction.OBJC);
            scope.Require(GlobalOverride.OVOF);
            scope.Require(GlobalOverride.OTST);
            var oce = new ObjectConstructorExpression();
            int paramCount = _options.FunctionParametersCountRange.RandomValue(_rand);
            while (paramCount-- > 0)
            {
                oce.Parameters.Add(GenExpression(scope, maxDepth));
            }
            Expression constructor;
            Variable v = _varSelector.ChooseVariable(scope);
            if(v != null)
            {
                constructor = new VariableExpression(v);
            }
            else if(scope.FunctionDepth < _options.MaxFunctionDepth)
            {
                constructor = GenFunctionExpression(scope);
            }
            else
            {
                throw new System.Exception("Unable to create a constructor for ObjectCreateExpression");
            }
            oce.Constructor = constructor;
            return oce;
        }

        internal Variable GenVariable(IScope scope, bool isParameter = false, bool isLoopCounter = false, bool isConstant = false, bool initialize = true)
        {
            var v = new Variable();
            v.Name = Variable.GetVariableName(scope.VariableCounter++);
            v.Parent = scope;
            v.IsParameter = isParameter;
            v.IsLoopCounter = isLoopCounter;
            if (isConstant || (!v.IsParameter && !v.IsLoopCounter && _rand.FlipCoin(_options.ConstVariableChance)))
            {
                v.IsConstant = true;
            }
            if (initialize && !v.IsParameter && !v.IsLoopCounter)
            {
                v.Initializer = GenExpression(scope, _options.VariableInitializerDepth);
                if(!v.IsConstant && _options.FunctionsAreConstants && v.Initializer is FunctionExpression)
                {
                    v.IsConstant = true;
                }
            }
            return v;
        }

        internal IfElseStatement GenIfElseStatement(IScope scope, Statement parent, int maxDepth)
        {
            var ife = new IfElseStatement(parent);
            ife.Condition = GenExpression(scope, _options.MaxExpressionDepth);
            ife.Body = GenStatement(scope, ife, maxDepth - 1);
            if (_rand.FlipCoin(_options.ElseChance))
            {
                ife.ElseBody = GenStatement(scope, ife, maxDepth - 1);
            }
            return ife;
        }

        internal FunctionBody GenFunctionBody(FunctionExpression parent)
        {
            var body = new FunctionBody(parent, _depthProtection, _rand.FlipCoin(_options.CatchChance));
            var block = body.TryBody;
            var that = GenVariable(block, false, false, true, false);
            that.Initializer = new VariableExpression(Variable.This);
            block.DeclaredVariables.Add(that);
            block.Statements.Add(that.Declaration);
            int localVariablesCount = _options.LocalVariablesCountRange.RandomValue(_rand);
            while (localVariablesCount-- > 0)
            {
                var v = GenVariable(block);
                block.DeclaredVariables.Add(v);
                block.Statements.Add(v.Declaration);
            }
            GenBlock(block, _options.MaxStatementDepth);
            if (block.Statements.Count == 0 || !block.Statements.Last().IsTerminating)
            {
                block.Statements.Add(GenReturnStatement(block));
            }
            return body;
        }

        internal Block GenBlock(Block block, int maxDepth)
        {
            int statementsCount = _options.BlockStatementsRange.RandomValue(_rand);
            while (statementsCount-- > 0)
            {
                var stmt = GenStatement(block, block, maxDepth - 1, StatementType.All & ~StatementType.BlockStatement);
                block.Statements.Add(stmt);
                if (stmt.IsTerminating)
                    break;
            }
            return block;
        }

        internal Block GenBlock(IScope scope, int maxDepth)
        {
            var block = new Block(scope);
            return GenBlock(block, maxDepth);
        }

        internal OutputStatement GenOutputStatement(Program program, IVariable v)
        {
            return GenOutputStatement(program, GenVariableInvocationExpression(program, v, _options.MaxExpressionDepth));
        }

        internal OutputStatement GenOutputStatement(Program program, Expression expr)
        {
            program.Require(GlobalFunction.PRNT);
            var os = new OutputStatement() { Value = expr };
            return os;
        }

        #endregion
    }
}
