COMPILER=mcs
BINDIR=bin/
CSFLAGS=-target:exe -unsafe -out:$@
RANDOMJS=$(BINDIR)Tevador.RandomJS.exe
MINER=$(BINDIR)Tevador.RandomJS.Crypto.exe
RANDOMJSCS=$(addprefix ./Tevador.RandomJS/, Operators/OperatorRequirement.cs Operators/BinaryOperator.cs Operators/Operator.cs Operators/AssignmentOperator.cs Operators/UnaryOperator.cs Loop.cs ExpressionStatement.cs Global.cs Xoshiro256Plus.cs IVariable.cs ReturnStatement.cs LoopCyclesProtection.cs GlobalVariable.cs BinaryUtils.cs ForLoop.cs IfElseStatement.cs ProgramFactory.cs RandomExtensions.cs Expressions/UnaryExpression.cs Expressions/Expression.cs Expressions/RoundedExpression.cs Expressions/GlobalFunctionExpression.cs Expressions/NumericExpression.cs Expressions/BinaryExpression.cs Expressions/NumericLiteral.cs Expressions/VariableInvocationExpression.cs Expressions/FunctionInvocationExpression.cs Expressions/VariableExpression.cs Expressions/FunctionExpression.cs Expressions/Literal.cs Expressions/TernaryExpression.cs Expressions/AssignmentExpression.cs Expressions/NonZeroExpression.cs Expressions/NonEmptyExpression.cs Expressions/NonNegativeExpression.cs GlobalFunction.cs AssignmentStatement.cs LoopControlExpression.cs ExpressionType.cs BreakStatement.cs LiteralType.cs IRandom.cs IProgram.cs ProgramOptions.cs CallDepthProtection.cs Block.cs VariableDeclaration.cs Variable.cs RandomTable.cs Properties/AssemblyInfo.cs TableEntry.cs StatementType.cs NumericLiteralType.cs Program.cs Statement.cs OutputStatement.cs IScope.cs EnumTable.cs OperatorTable.cs ProgramOptionsException.cs ProgramOptionsHandler.cs)
MINERCS=$(addprefix ./Tevador.RandomJS.Crypto/, Solution.cs Properties/AssemblyInfo.cs Blake/Blake2BConfig.cs Blake/Blake2BCoreUnrolled.cs Blake/Blake2B256.cs Blake/Blake2IvBuilder.cs Blake/Blake2BCore.cs Blake/Blake2BHash.cs Blake/Blake2BTreeConfig.cs Miner.cs)
XMLOPTIONS=ProgramOptions.xml

$(MINER): $(RANDOMJS) $(MINERCS)
	$(COMPILER) /reference:$(RANDOMJS) $(CSFLAGS) $(MINERCS)
    
$(RANDOMJS): $(RANDOMJSCS) $(BINDIR)$(XMLOPTIONS)
	$(COMPILER) $(CSFLAGS) $(RANDOMJSCS)

$(BINDIR)$(XMLOPTIONS): | $(BINDIR)
	cp ./Tevador.RandomJS/$(XMLOPTIONS) $(BINDIR)

$(BINDIR):
	mkdir -p $(BINDIR)

clean:
	rm -f $(RANDOMJS) $(MINER) $(BINDIR)$(XMLOPTIONS)