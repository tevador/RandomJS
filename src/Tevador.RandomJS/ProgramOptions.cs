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

using System;
using Tevador.RandomJS.Operators;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Tevador.RandomJS
{
    public class ProgramOptions
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(ProgramOptions), new XmlRootAttribute(nameof(ProgramOptions)));

        public static ProgramOptions FromXml()
        {
            var configurationFileName = typeof(ProgramOptions).Name + ".xml";
            try
            {
                using (var reader = File.OpenText(configurationFileName))
                    return FromXml(XmlReader.Create(reader));
            }
            catch(Exception e)
            {
                throw new ProgramOptionsException($"Failed to load ProgramOptions from file {configurationFileName}", e);
            }
        }

        public void Initialize()
        {
            AssignmentOperators = new OperatorTable<AssignmentOperator>();
            UnaryOperators = new OperatorTable<UnaryOperator>();
            BinaryOperators = new OperatorTable<BinaryOperator>();
            Literals = new EnumTable<LiteralType>();
            NumericLiterals = new EnumTable<NumericLiteralType>();
            Expressions = new EnumTable<ExpressionType>();
            Statements = new EnumTable<StatementType>();
            AssignmentInForLoop = new OperatorTable<AssignmentOperator>();
            BlockStatementsRange = new Interval();
            FunctionParametersCountRange = new Interval();
            StringLiteralLengthRange = new Interval();
            GlobalVariablesCountRange = new Interval();
            LocalVariablesCountRange = new Interval();
            SwitchLabelsCountRange = new Interval();
            MaxCallDepthRange = new Interval();
            MaxLoopCyclesRange = new Interval();
            MaxStringLengthRange = new Interval();
            MathPrecisionRange = new Interval();
            ObjectLiteralSizeRange = new Interval();
            EvalStringLength = new Interval();
        }

        internal static ProgramOptions FromXml(XmlReader reader)
        {
            var options = (ProgramOptions)_serializer.Deserialize(reader);
            options.Validate();
            return options;
        }

        internal void Validate()
        {
            if (AssignmentOperators.Total <= 0) ErrorTable(nameof(AssignmentOperators));
            if (UnaryOperators.Total <= 0) ErrorTable(nameof(UnaryOperators));
            if (BinaryOperators.Total <= 0) ErrorTable(nameof(BinaryOperators));
            if (Literals.Total <= 0) ErrorTable(nameof(Literals));
            if (NumericLiterals.Total <= 0) ErrorTable(nameof(NumericLiterals));
            if (Expressions.Total <= 0) ErrorTable(nameof(Expressions));
            if (Statements.Total <= 0) ErrorTable(nameof(Statements));
            if (AssignmentInForLoop.Total <= 0) ErrorTable(nameof(AssignmentInForLoop));
        }

        internal void ErrorTable(string table)
        {
            throw new ProgramOptionsException($"Sum of weights in table '{table}' must be greater than zero");
        }

        public OperatorTable<AssignmentOperator> AssignmentOperators { get; set; }
        public OperatorTable<UnaryOperator> UnaryOperators { get; set; }
        public OperatorTable<BinaryOperator> BinaryOperators { get; set; }
        public EnumTable<LiteralType> Literals { get; set; }
        public EnumTable<NumericLiteralType> NumericLiterals { get; set; }
        public EnumTable<ExpressionType> Expressions { get; set; }
        public EnumTable<StatementType> Statements { get; set; }
        public OperatorTable<AssignmentOperator> AssignmentInForLoop { get; set; }

        public Interval BlockStatementsRange { get; set; }
        public Interval FunctionParametersCountRange { get; set; }
        public Interval StringLiteralLengthRange { get; set; }
        public Interval GlobalVariablesCountRange { get; set; }
        public Interval LocalVariablesCountRange { get; set; }
        public Interval SwitchLabelsCountRange { get; set; }
        public Interval MaxCallDepthRange { get; set; }
        public Interval MaxLoopCyclesRange { get; set; }
        public Interval MaxStringLengthRange { get; set; }
        public Interval MathPrecisionRange { get; set; }
        public Interval ObjectLiteralSizeRange { get; set; }
        public Interval EvalStringLength { get; set; }

        public int VariableSelectorScopeFactor { get; set; }
        public int ObjectSetPropertyCount { get; set; }
        public int VariableInitializerDepth { get; set; }
        public int MaxExpressionDepth { get; set; }
        public int MaxStatementDepth { get; set; }
        public int MaxObjectLiteralDepth { get; set; }
        public int MaxFunctionDepth { get; set; }
        public bool EnableCallDepthProtection { get; set; }
        public bool EnableLoopCyclesProtection { get; set; }
        public double ConstVariableChance { get; set; }
        public double ElseChance { get; set; }
        public bool FunctionsAreConstants { get; set; }
        public double ForLoopVariableBoundsChance { get; set; }
        public bool AllowFunctionInvocationInLoop { get; set; }
        public bool FunctionValueOfOverride { get; set; }
    }
}
