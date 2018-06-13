COMPILER=mcs
BINDIR=bin/
CSFLAGS=-target:exe -unsafe -out:$@
RANDOMJS=$(BINDIR)Tevador.RandomJS.exe
MINER=$(BINDIR)Tevador.RandomJS.Miner.exe
RANDOMJSCS=$(addprefix ./Tevador.RandomJS/, EnumTable.cs ProgramOptionsHandler.cs GlobalVariable.cs RandomExtensions.cs Program.cs ProgramOptions.cs IVariable.cs LiteralType.cs BinaryUtils.cs Xoshiro256Plus.cs StatementType.cs CallDepthProtection.cs LoopCyclesProtection.cs Interval.cs NumericLiteralType.cs OperatorTable.cs IScope.cs Variable.cs VariableOptions.cs Operators/Operator.cs Operators/OperatorRequirement.cs Operators/AssignmentOperator.cs Operators/UnaryOperator.cs Operators/BinaryOperator.cs Global.cs Properties/AssemblyInfo.cs RandomTable.cs Expressions/NonEmptyExpression.cs Expressions/VariableExpression.cs Expressions/UnaryExpression.cs Expressions/Expression.cs Expressions/NumericLiteral.cs Expressions/NumericExpression.cs Expressions/NonNegativeExpression.cs Expressions/RoundedExpression.cs Expressions/AssignmentExpression.cs Expressions/BinaryExpression.cs Expressions/TernaryExpression.cs Expressions/GlobalFunctionExpression.cs Expressions/FunctionExpression.cs Expressions/NonZeroExpression.cs Expressions/LoopControlExpression.cs Expressions/FunctionInvocationExpression.cs Expressions/Literal.cs Expressions/VariableInvocationExpression.cs Expressions/EvalExpression.cs Expressions/ObjectSetExpression.cs Expressions/ObjectConstructorExpression.cs Statements/OutputStatement.cs Statements/VariableDeclaration.cs Statements/Statement.cs Statements/AssignmentStatement.cs Statements/ReturnStatement.cs Statements/IfElseStatement.cs Statements/Block.cs Statements/BreakableStatement.cs Statements/ForLoop.cs Statements/BreakStatement.cs Statements/Loop.cs Statements/EmptyStatement.cs Statements/ExpressionStatement.cs Statements/ObjectSetStatement.cs ProgramOptionsException.cs IProgram.cs IRandom.cs ProgramFactory.cs ExpressionType.cs GlobalFunction.cs TableEntry.cs Expressions/ObjectLiteral.cs GlobalOverride.cs VariableSelector.cs)
MINERCS=$(addprefix ./Tevador.RandomJS.Miner/, Solution.cs Properties/AssemblyInfo.cs Blake/Blake2BConfig.cs Blake/Blake2BCoreUnrolled.cs Blake/Blake2B256.cs Blake/Blake2IvBuilder.cs Blake/Blake2BCore.cs Blake/Blake2BHash.cs Blake/Blake2BTreeConfig.cs Miner.cs)
RUNDLL=$(BINDIR)Tevador.RandomJS.Run.dll
RUNCS=$(addprefix ./Tevador.RandomJS.Run/, Properties/AssemblyInfo.cs ProgramRunnerBase.cs ProgramRunner.cs ExternalProgramRunner.cs RuntimeInfo.cs)
TESTEXE=$(BINDIR)Tevador.RandomJS.Test.exe
TESTCS=$(addprefix ./Tevador.RandomJS.Test/, Properties/AssemblyInfo.cs EntropyCounter.cs ListStats.cs ParallelRunner.cs RuntimeStats.cs Program.cs Options.cs)
XMLOPTIONS=ProgramOptions.xml

$(MINER): $(RANDOMJS) $(TESTEXE) $(MINERCS)
	$(COMPILER) /reference:$(RANDOMJS),$(RUNDLL) $(CSFLAGS) $(MINERCS)

$(TESTEXE): $(RUNDLL) $(RANDOMJS) $(TESTCS)
	$(COMPILER) /reference:$(RANDOMJS),$(RUNDLL) $(CSFLAGS) $(TESTCS)

$(RUNDLL): $(RANDOMJS) $(RUNCS)
	$(COMPILER) /reference:$(RANDOMJS) -target:library -out:$@ $(RUNCS)

$(RANDOMJS): $(RANDOMJSCS) $(BINDIR)$(XMLOPTIONS)
	$(COMPILER) $(CSFLAGS) $(RANDOMJSCS)

$(BINDIR)$(XMLOPTIONS): | $(BINDIR)
	cp ./Tevador.RandomJS/$(XMLOPTIONS) $(BINDIR)

$(BINDIR):
	mkdir -p $(BINDIR)

clean:
	rm -f $(RANDOMJS) $(RUNDLL) $(MINER) $(TESTEXE) $(BINDIR)$(XMLOPTIONS)