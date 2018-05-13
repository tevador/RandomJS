using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tevador.RandomJS.Operators;

namespace Tevador.RandomJS
{
    public class ProgramFactory
    {
        private ProgramOptions _options;
        private IRandom _randomGenerator;

        public IProgram GenProgran(byte[] seed)
        {
            _options.Seed = seed;
            return new Program().Generate(_randomGenerator, _options);
        }

        public ProgramFactory()
            : this(new Xoshiro256Plus())
        {
        }

        internal ProgramFactory(Xoshiro256Plus random)
        {
            _randomGenerator = random;
            _options = new ProgramOptions()
            {
                AssignmentOperators = new RandomTable<AssignmentOperator>()
                {
                    { 0.15, AssignmentOperator.Add },
                    { 0.05, AssignmentOperator.Sub },
                    { 0.05, AssignmentOperator.Mul },
                    { 0.05, AssignmentOperator.Div },
                    { 0.04, AssignmentOperator.Rem },
                    { 0.7, AssignmentOperator.Mov },
                    { 0.2, AssignmentOperator.PreInc },
                    { 0.2, AssignmentOperator.PostDec },
                    { 0.2, AssignmentOperator.PreDec },
                    { 0.2, AssignmentOperator.PostInc },
                },
                BinaryOperators = new RandomTable<BinaryOperator>()
                {
                    { 0.4, BinaryOperator.Add },
                    { 0.1, BinaryOperator.Sub },
                    { 0.1, BinaryOperator.Mul },
                    { 0.1, BinaryOperator.Div },
                    { 0.1, BinaryOperator.Rem },
                    { 0.1, BinaryOperator.Equal },
                    { 0.1, BinaryOperator.NotEqual },
                    { 0.05, BinaryOperator.Min },
                    { 0.05, BinaryOperator.Max },
                    { 0.1, BinaryOperator.Xor },
                    { 0.1, BinaryOperator.Less },
                    { 0.1, BinaryOperator.Greater },
                    { 0.1, BinaryOperator.Comma },
                },
                UnaryOperators = new RandomTable<UnaryOperator>()
                {
                    { 0.05, UnaryOperator.Plus },
                    { 0.1, UnaryOperator.Typeof },
                    { 0.2, UnaryOperator.Minus },
                    { 0.2, UnaryOperator.Not },
                    { 0.2, UnaryOperator.Sqrt },
                    { 0.05, UnaryOperator.Exp },
                    { 0.05, UnaryOperator.Log },
                    { 0.05, UnaryOperator.Sin },
                    { 0.05, UnaryOperator.Cos },
                    { 0.05, UnaryOperator.Atan },
                    { 0.05, UnaryOperator.Floor },
                    { 0.05, UnaryOperator.Ceil },
                },
                ConstVariableChance = 0.1,
                //DepthProtection = new CallDepthProtection(5),
                GlobalVariablesCount = 10,
                MaxExpressionDepth = 5,
                MaxStatementDepth = 2,
                MaxFunctionParameterCount = 3,
                MaxStringLiteralLength = 10,
                FuncInvocationInExprChance = 0.25,
                MaxStringVariableLength = 20,
                PreferFuncParametersToLiterals = true,
                MaxExpressionAttempts = 10,
                FpMathPrecision = 9,
                Literals = new RandomTable<LiteralType>()
                {
                    { 0.2, LiteralType.String },
                    { 0.8, LiteralType.Numeric }
                },
                NumericLiterals = new RandomTable<NumericLiteralType>()
                {
                    { 0.125, NumericLiteralType.Boolean },
                    { 0.125, NumericLiteralType.DecimalInteger },
                    { 0.05, NumericLiteralType.BinaryInteger },
                    { 0.125, NumericLiteralType.OctalInteger },
                    { 0.125, NumericLiteralType.SmallInteger },
                    { 0.125, NumericLiteralType.HexInteger },
                    { 0.125, NumericLiteralType.FixedFloat },
                    { 0.125, NumericLiteralType.ExpFloat },

                },
                Expressions = new RandomTable<ExpressionType>()
                {
                    { 0.13, ExpressionType.AssignmentExpression },
                    { 0.15, ExpressionType.BinaryExpression },
                    { 0.2, ExpressionType.FunctionExpression },
                    { 0.1, ExpressionType.Literal },
                    { 0.05, ExpressionType.TernaryExpression },
                    { 0.07, ExpressionType.UnaryExpression },
                    { 0.1, ExpressionType.VariableInvocationExpression },
                }
            };
        }
    }
}
