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

        public static ProgramOptions FromXml(XmlReader reader)
        {
            var options = (ProgramOptions)_serializer.Deserialize(reader);
            options.Validate();
            return options;
        }

        public void Validate()
        {
            if (AssignmentOperators.Total <= 0) ErrorTable(nameof(AssignmentOperators));
            if (UnaryOperators.Total <= 0) ErrorTable(nameof(UnaryOperators));
            if (BinaryOperators.Total <= 0) ErrorTable(nameof(BinaryOperators));
            if (Literals.Total <= 0) ErrorTable(nameof(Literals));
            if (NumericLiterals.Total <= 0) ErrorTable(nameof(NumericLiterals));
            if (Expressions.Total <= 0) ErrorTable(nameof(Expressions));
            if (Statements.Total <= 0) ErrorTable(nameof(Statements));
            if (AssignmentInForLoop.Total <= 0) ErrorTable(nameof(AssignmentInForLoop));

            if (MaxSmallInteger <= 0) throw new ProgramOptionsException($"Value of {nameof(MaxSmallInteger)} must be greater than zero.");
        }

        public void ErrorTable(string table)
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

        public int MaxCallDepth
        {
            get
            {
                if (DepthProtection != null) return DepthProtection.MaxDepth;
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    DepthProtection = new CallDepthProtection(value);
                }
            }
        }

        public int MaxLoopCycles
        {
            get
            {
                if (CyclesProtection != null) return CyclesProtection.MaxCycles;
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    CyclesProtection = new LoopCyclesProtection(value);
                }
            }
        }
        public int MaxExpressionDepth { get; set; }
        public int MaxStatementDepth { get; set; }
        public int MaxStringVariableLength { get; set; }
        public int MaxExpressionAttempts { get; set; }
        public int MaxStatementAttempts { get; set; }
        public int FpMathPrecision { get; set; }
        public double ConstVariableChance { get; set; }
        public double ElseChance { get; set; }
        public bool AllowFunctionOverwriting { get; set; }
        public bool AllowNestedFunctions { get; set; }
        public bool PreferFuncParameters { get; set; }
        public int MaxSmallInteger { get; set; }
        public double ForLoopVariableBoundsChance { get; set; }

        [XmlIgnore]
        internal CallDepthProtection DepthProtection { get; set; }

        [XmlIgnore]
        internal LoopCyclesProtection CyclesProtection { get; set; }

        [XmlIgnore]
        internal VariableOptions VariableOptions
        {
            get { return AllowFunctionOverwriting ? VariableOptions.NonFunctionInitializer : 0; }
        }
    }
}
