# RandomJS Generator Documentation

The general outline of the generated program is following:

```javascript
//1. Strict mode declaration
'use strict';

//private scope
{
    //2. Definition of global helper functions, constants and variables
    let __depth = 0;
    const __maxDepth = 3;
    function __tstr(_) {
        return _ != null ? __strl(_.toString()) : _;
    }
    function __prnt(_) {
        print(__tstr(_));
    }
    //etc.

    //3. Definition of randomly generated global variables
    let a = Expression;
    let b = Expression;
    let c = Expression;
    //etc.

    //4. Output statements
    __prnt(__invk(b, Expression, ...));
    __prnt(__invk(c, Expression, ...));
    __prnt(__invk(a, Expression, ...));
    //etc.
}
```

* The program runs in [strict mode](http://www.ecma-international.org/ecma-262/6.0/#sec-strict-mode-code) to prevent some problematic behavior of javascript.
* The code is wrapped in a private code block so that the declarations in the program don't pollute the global scope. This enables a single engine to execute many programs in sequence.
* The program begins with definitions of helper functions, constants and variables. The order of these helper definitions is pseudo-random (a helper function is attached to the scope upon being first referenced from the main code).
* The number of global variables is generated at random from a specified interval.
* Each program prints its global variables in random order.
* The global scope has no statements apart variable definitions and output. Other statements are restricted inside functions. 

## Helper functions and variables

These are represented by the `Global` abstract class. The names of all these global definitions begin with two underscores to prevent collisions with randomly generated variables.

#### Call depth variables
```javascript
let __depth = 0;
const __maxDepth = 3;
```
These variables are used to prevent infinite recursion during program execution. `__depth` represents the current depth of the call stack and `__maxDepth` is the maximum value (this is randomly generated from a specified interval). `__depth` is incremented each time a function is entered and decremented just before each `return` statement.

#### Loop cycles variables
```javascript
let __cycles = 0;
const __maxCycles = 4530;
```
These variables are used to limit the number of loop executions (especially for nested loops) and to prevent infinite loops. The maximum number of loop cycles is determined by the `__maxCycles` constant (generated at random from a specified interval).

#### TRYC function
```javascript
function __tryc(_, __) {
    try {
        return _();
    } catch (_e) {
        return _e.name + __;
    }
}
```
This helper function is used for operations which can throw an error. The first parameter is a function which can fail and the second parameter is the default value. In case of an error, the [name](http://www.ecma-international.org/ecma-262/6.0/#sec-error.prototype.name) of the error is prepended to the default value. This is safe because the names of common errors, such as [SyntaxError](http://www.ecma-international.org/ecma-262/6.0/#sec-native-error-types-used-in-this-standard-syntaxerror), [ReferenceError](http://www.ecma-international.org/ecma-262/6.0/#sec-native-error-types-used-in-this-standard-referenceerror) or [TypeError](http://www.ecma-international.org/ecma-262/6.0/#sec-native-error-types-used-in-this-standard-typeerror), are part of the language specification (unlike the error message, which is implementation-defined).

#### OTST override
```javascript
Object.prototype.toString = function() {
    return __tryc(() => JSON.stringify(this), '[Object]');
};
```
This overrides the default `Object.toString` function by converting the object to its JSON representation (the default `toString` function just returns `[object o]`). The JSON conversion will throw a TypeError if the object is circular (that's why the `__tryc` function is used).

#### FTST override
```javascript
Function.prototype.toString = function() {
    return '[Function]' + this.name;
};
```
This overrides the default `Function.toString` function, which has implementation-specific output.

#### OVOF override
```javascript
Object.prototype.valueOf = function() {
    for (let _ in this)
        if (typeof this[_] === 'number') return this[_];
    return this;
};
```
This overrides the default `Object.valueOf` function. The function is called whenever an object has to be converted to a primitive value. In this case, we return the first numeric value among the object's properties. This increases the output variability because an object can be represented either by a JSON string or as a number (if a numeric operation was attempted on the object).

#### FVOF override
```javascript
const __fvof = '__fvof';

Function.prototype.valueOf = function() {
    if (!this.name) {
        (__fvof in this) || (this[__fvof] = this());
        if (typeof this[__fvof] !== 'function') return this[__fvof];
    }
    return this.toString();
};
```
This overrides the default `Object.valueOf` for the `Function` type. For an anonymous function, it returns the result of calling that function without parameters (except if the call returns a function to prevent excessive recursion). The result of the call is cached, so each function is called just once to obtain its 'value' (this is needed for performance reasons). The main reason for this override is to force a call for anonymous functions in expressions such as `(123456 + function() { ... })`. For named functions, this override simply calls `Function.toString`.

#### OBJC function
```javascript
function __objc(_, ...__) {
    if (typeof _ === 'function') return new _(...__);
    if (typeof _ === 'object') return _;
    return { a: _ };
}
```
This function is used to create a new object at runtime. If the first argument is a function, it is used as the constructor. Otherwise either the argument itself is returned (if it's an object) or a new object with property 'a' is created.

#### EVAL function
```javascript
function __eval(_f, _s) {
    return __tryc(() => _f(_s), _s);
}
```
This helper function is used by the `EvalExpression` (see below). The second parameter is the string to be evaluated and at the same time the default value for `__tryc`.

#### NUMB function
```javascript
function __numb(_, __) {
    _ = +_;
    if (!isNaN(_)) return _;
    else return __;
}
```
This function is used by `UnaryExpression` and `BinaryExpression` in case their operator expects a number. The function tries to convert its first argument to a number and if that fails, provides a default numeric value (second argument). This function is necessary to prevent all mathematical operations from becoming `NaN`.

#### STRL function
```javascript
const __maxStrlen = 38;

function __strl(_) {
    if (typeof _ === 'string' && _.length > __maxStrlen) return _.substring(0, __maxStrlen);
    return _;
}
```
This function is used to limit the length of string variables. This is required to prevent out of memory errors if string concatenation happens in a loop. The value of the `__maxStrlen` constant is generated at random from a specified interval.

#### INVK function
```javascript
function __invk(_, ...__) {
    if (typeof _ === 'function') return __strl(_(...__));
    else return __strl(_);
}
```
This function is used to 'invoke' a variable. This means the variable is called if it's a function and otherwise just the value of the variable is returned. The function gets a random number of additional arguments to pass to the function call.

#### NONZ function
```javascript
function __nonz(_) {
    return _ == 0 ? 1 : _;
}
```
This function is used by `AssignmentExpression` and `BinaryExpression` in case their operator requires a non-zero value (for division operators).

#### CALC function
```javascript
function __calc(_, _f, _d) {
    if (typeof _ === 'number') return _f(_);
    return _d;
}
```
This function is used by `AssignmentExpression` if its operator requires a numeric value (for example the increment operator or `*=` assinment). The assignment is made only if the left-hand side variable is a number to prevent the operation from becoming `NaN`. If the variable is not a number, a default numeric value is returned.

#### OBJS function
```javascript
function __objs(_, _k, _v) {
    if (Object.isExtensible(_)) _[_k] = _v;
    return _ || _v;
}
```
This function is used by the `ObjectSetExpression`. If the first argument is an extensible object, a new property is defined for that object. The value of the property also serves as a default return value in case the first argument is empty or undefined.

#### PREC function
```javascript
const __fpMathPrec = 10;

function __prec(_) {
    return +_.toPrecision(__fpMathPrec);
}
```
This function is used whenever a mathematical operation with inexact result is performed (for example `Math.exp` or other trancendental math functions). The function rounds off the number to a specific number of decimal significant digits given by the `__fpMathPrec` constant. The value of this constant is generated at random from interval 10-14. It has been determined empirically that the first 14 significant digits match across different platforms (see [Issue 3](https://github.com/tevador/RandomJS/issues/3)). 

#### NNEG function
```javascript
function __nneg(_) {
    return _ < 0 ? -_ : _;
}
```
This function is used for operations which require a non-negative number (`Math.sqrt` and `Math.log`) to prevent `NaN` results.

#### TSTR function
```javascript
function __tstr(_) {
    return _ != null ? __strl(_.toString()) : _;
}
```
This function is used to get the string representation of a variable for printing. It calls the argument's `toString` function which behaves differently depending on the prototype of the argument.

#### PRNT function
```javascript
function __prnt(_) {
    print(__tstr(_));
}
```
This function is used to output global variables at the end of the program. The underlying javascript engine should implement a `print` function that prints its argument followed by a newline character to the standard output.

## Variables
All variables in the program are block scoped using the [let or const declaration](http://www.ecma-international.org/ecma-262/6.0/#sec-let-and-const-declarations) (unlike [var declarations](http://www.ecma-international.org/ecma-262/6.0/#sec-variable-statement) which are function scoped). Constant variables are generated with specified probability. 

The names of variables follow simple alphabetical order: first variable in a given scope is *a*, second is *b*, third *c*, etc. After *z*, the sequence continues with *aa*, *ab*, *ac*, etc. Function arguments and variables declared in a for-loop follow the same naming rules. Because all variables are block scoped, the program can have many variables with the same name in different lexical scopes.

Constants and loop counters are never assigned to. Additionally, assignment is forbidden for variables that are initialized with a FunctionExpression. This is done to limit the amount of dead code (functions that can never be called because the reference has been lost).

## Expressions

Expressions in the program are generated at random from the following list:
* Literal
* EvalExpression
* AssignmentExpression
* VariableInvocationExpression
* FunctionExpression
* FunctionInvocationExpression
* UnaryExpression
* BinaryExpression
* TernaryExpression
* VariableExpression
* ObjectConstructorExpression
* ObjectSetExpression

Each experession has a specified probability of being generated. Expressions can be nested up to the specified depth.

### Literal
There are three types of literals:
1. String literal
1. Numeric literal
1. Object literal

#### StringLiteral
String literals are generated from printable ASCII characters. Strings are either single-quoted or double-quoted (chosen randomly) and the length of the literal is generated at random from the specified interval.

#### NumericLiteral
There are 8 types of numeric literals. Each numeric literal is generated as a string of random digits (the binary representation is not known to the generator). Except for Boolean, each number can be positive or negative.
* Boolean (`true` or `false`)
* SmallInteger (2 decimal digits, e.g. `-73`)
* BinaryInteger (32 binary digits, e.g. `0b11101010000100010110010111000101`)
* DecimalInteger (9 decimal digits, e.g. `-260285545`)
* OctalInteger (10 octal digits, e.g. `0o4523110006`)
* HexInteger (8 hex digits, e.g. `-0x63c49603`)
* FixedFloat (5 decimal digits before decimal point, 5 digits after, e.g. `29834.94312`)
* ExpFloat  (one decimal digit before decimal point, 5 digits after and 2 digit exponent, e.g. `9.23234e32`)

#### ObjectLiteral
Object literals are generated in the form `{ a: literal, b: literal, c: literal, ... }`. The number of properties is generated at random from the specified interval. Object literals can be nested up to the specified depth (e.g. `{ a: { a: true } }`).

### EvalExpression
One of the features of the javascript language is the possiblity of evaluating code at runtime. The `EvalExpression` evaluates a random string literal in the current scope using the EVAL helper function (see above). The evaluated string contains 10 random characters from the following character set: ``/cb1/|=`+-a2+e84``. It has been empirically determined that this character set produces relatively low number of syntax errors, while exploring many features of the language. Two characters are included twice (`/` and `+`) so they have a double chance of occuring. Thus, there are 14 unique characters, giving 14<sup>10</sup> total possibilities (~290 billion). This is sufficiently high to prevent dictionary lookup and forces all implementations to include a javascript parser.

The result of `EvalExpression` has approximately the following distribution:
* Slightly over 5% chance of being a valid javascript expression.
* Around 18% chance of producing an error other than `SyntaxError` (most common are `ReferenceError` and `TypeError`).
* The remaining ~77% is a `SyntaxError`.

The following table lists some examples of `EvalExpression`:

|Expression|Result|
|----------|------|
|<code>\_\_eval(\_ => eval(\_), 'ab=e28&#124;/-c')</code> | <code> SyntaxErrorab=e28&#124;/-c </code> (invalid character)|
|<code>\_\_eval(\_ => eval(\_), '++/e+=&#124;12/')</code> | <code>ReferenceError++/e+=&#124;12/</code> (attempt to increment a regular expression)|
|<code>\_\_eval(\_ => eval(\_), 'e//42/b+e2')<code> | value of the `e` variable in the current scope (assuming it's declared)|
|<code>\_\_eval(\_ => eval(\_), '+1+4//2-&#124;-')</code> | `5`|
|<code>\_\_eval(\_ => eval(\_), '//++&#124;&#124;4/c2')</code> | `undefined` (the expression is a comment)|
|<code>\_\_eval(\_ => eval(\_), 'a\`&#124;/=2-=2\`')</code> | assuming variable `a` is a function, this evaluates a [tagged template](http://www.ecma-international.org/ecma-262/6.0/#sec-tagged-templates)|

### AssignmentExpression
The generator supports the following assignment operators:

|Operator|Symbol|Attributes|
|--------|------|----------|
|Basic|`=`||
|Add|`+=`|StringLengthLimit|
|Sub|`-=`|NumericOnly|
|Mul|`*=`|NumericOnly|
|Div|`/=`|NumericOnly, NonzeroRHS|
|Mod|`%=`|NumericOnly, NonzeroRHS|
|PreInc|`++`|NumericOnly, Prefix, WithoutRHS|
|PostInc|`++`|NumericOnly, WithoutRHS
|PreDec|`--`|NumericOnly, Prefix, WithoutRHS|
|PostDec|`--`|NumericOnly, WithoutRHS|

Explanation of operator attributes:

* **StringLengthLimit** - the result must be wrapped in a call to the STRL helper function
* **NumericOnly** - the assignment must be wrapped in a call to the CALC helper function (to make sure the assignment takes place only if the left-hand side is numeric)
* **NonzeroRHS** - the right-hand side must be wrapped in a call to the NONZ hellper function (to prevent division by zero)
* **Prefix** - the operator comes before the variable name
* **WithoutRHS** - the operator doesn't have a right-hand side

Examples of assignment expressions: 
* `__calc(i, () => (i++), 73195.53799)`
* `(n = "Bl0t;=0$wy")`
* `((m += b), m = __strl(m))`
* `(s /= __nonz(3.65278e18))`

### VariableInvocationExpression
This expression is a substitute of a call to a function variable. Since the generator doesn't know if a variable is a function or not, the INVK helper function is used. A random number of expressions is passed to the INVK function.

Examples: 
* `__invk(m, __calc(m, () => (m--), 83))`
* `__invk(i, g, '\n')`
* `__invk(h, __calc(c, () => (c -= 7), false), true)`

### FunctionExpression
This defines a new function. The function takes a random number of arguments. Each function begins with two statements:

1. `const o = this;`
1. `if (++__depth > __maxDepth) { --__depth; return Expression; }`

First statement is to capture `this` object (for constructor calls), the second statement is for the call depth protection.

After these two statements, a random number of local variables can be defined and a random number of statements is executed (the body of a function is basically a `BlockStatement` as described below). Each function ends with a return statement (there can be multiple return statements if the function contains conditional statements).

The generator will not generate a `FunctionExpression` if the maximum function depth has been reached (to limit function nesting).

### FunctionInvocationExpression

`FunctionInvocationExpression` is like a `FunctionExpression`, but the function is immediately called with a random set of arguments.

Example: `(function (b, c) { ... })(true, 87605.64090)`

### UnaryExpression
The generator uses the following unary operators:

|Operator|Symbol|Attributes|
|--------|------|----------|
|Not|`!`||
|Plus|`+`||
|Minus|`-`|NumericOnly|
|Typeof|`typeof`||

Additionally, the following mathematical function calls can be generated as a `UnaryExpression`:

|Operator|Function|Attributes|
|--------|------|----------|
|Sqrt|`Math.sqrt`|NumericOnly, LimitedPrecision, NonnegativeRHS|
|Abs|`Math.abs`|NumericOnly|
|Ceil|`Math.ceil`|NumericOnly|
|Trunc|`Math.trunc`|NumericOnly|
|Floor|`Math.floor`|NumericOnly|
|Sin|`Math.sin`|NumericOnly, LimitedPrecision|
|Cos|`Math.cos`|NumericOnly, LimitedPrecision|
|Exp|`Math.exp`|NumericOnly, LimitedPrecision|
|Log|`Math.log`|NumericOnly, LimitedPrecision, NonnegativeRHS|
|Atan|`Math.atan`|NumericOnly, LimitedPrecision|

Explanation of operator attributes:

* **NumericOnly** - the right-hand side expression (argument) must be wrapped in a call to the NUMB helper function to ensure numeric input
* **NonnegativeRHS** - the right-hand side must be wrapped in a call to the NNEG helper function
* **LimitedPrecision** - the result of the operation must be wrapped in the PREC helper function to ensure consistency across platforms

Examples:
* `__prec(Math.log(__nneg(__numb((a=(-10)), 81711.92057))))`
* `(typeof __invk(d,{a:(-200362066),b:"r75",}))`
* `Math.floor(__numb(b, 76952.72820))`

### BinaryExpression
The generator uses the following binary operators:

|Operator|Symbol|Attributes|
|--------|------|----------|
|Add|`+`|StringLengthLimit|
|Comma|`,`||
|Sub|`-`|NumericOnly|
|Mul|`*`|NumericOnly|
|Div|`/`|NumericOnly, NonzeroRHS|
|Mod|`%`|NumericOnly, NonzeroRHS|
|Less|`<`||
|Greater|`>`||
|Equal|`==`||
|NotEqual|`!=`||
|And|`&&`||
|Or|<code> &#124;&#124; </code>||
|BitAnd|`&`|NumericOnly|
|BitOr|<code> &#124; </code>|NumericOnly|
|Xor|`^`|NumericOnly|
|ShLeft|`<<`|NumericOnly|
|ShRight|`>>`|NumericOnly|
|UnShRight|`>>>`|NumericOnly|

Additionally, the following mathematical function calls can be generated as a `BinaryExpression`:

|Operator|Function|Attributes|
|--------|------|----------|
|Min|`Math.min`|NumericOnly|
|Max|`Math.max`|NumericOnly|

Explanation of operator attributes:

* **StringLengthLimit** - the result must be wrapped in a call to the STRL helper function
* **NumericOnly** - both arguments must be wrapped in a call to the NUMB helper function to ensure numeric input
* **NonzeroRHS** - the right-hand side must be wrapped in a call to the NONZ helper function (to prevent division by zero)

### TernaryExpression
Generated in the form `Expression ? Expression : Expression`.

### VariableExpression
This expression is simply a name of a random variable, e.g. `a`.

### ObjectConstructorExpression
This expression uses the OBJC helper function in the form `__objc(Constructor, Argument1, Argument2, ...)`. The *Constructor* is either a `VariableExpression` (if any variable exists in the current scope) or a newly generated `FunctionExpression`.

### ObjectSetExpression
This expression uses the OBJS helper function in the form `__objs(Target, Property, Expression)`. The *Target* can be either a `VariableExpression` or an `ObjectLiteral`. The *Property* is a random string label, which follows the same rules as variable names (`'a'`, `'b'`, `'c'`, ..., `'z'`, `'aa'`, `'ab'`, ...). 

## Statements

The following statements can be generated:

* AssignmentStatement
* BlockStatement
* BreakStatement
* ForLoopStatement
* IfElseStatement
* ObjectSetStatement
* ReturnStatement
* VariableInvocationStatement

Each statement has a specified probability of being generated. Statements can be nested up to the specified depth.

### AssignmentStatement
The statement has the form of `AssignmentExpression;`.

### BlockStatement
BlockStatement is a code block with the following structure:
```javascript
{
    //1. Declaration of local variables
    let e = Expression;
    let f = Expression;
    const g = Expression;
    //etc.
    
    //2. Sequence of random statements
    Statement
    Statement
    ///etc.
}
```
`BlockStatement` cannot contain another `BlockStatement` directly.

### BreakStatement
`break;`
This is statement can be generated only inside a `ForLoopStatement`.

### ForLoopStatement
The statement has the form of:
```javascript
for(let loopCounter = SmallInteger; Condition && (__cycles++<__maxCycles); IteratorExpression) 
    Statement
```
The *Condition* has the form of `loopCounter < BoundExpression` or `loopCounter > BoundExpression` (chosen at random). *BoundExpression* is either a `NumericLiteral` or a `VariableExpression`. *IteratorExpression* is a random `AssignmentExpression` involving the `loopCounter`. 'NumericOnly' assignment operators are used.

### IfElseStatement
The statement has the form of: `if(Expression) Statement` or `if(Expression) Statement else Statement` (the version with `else` has a specified probability).

### ObjectSetStatement
The statement has the form of `ObjectSetExpression;`.

### ReturnStatement
The statement has the form of two statements:
```javascript
{
    --__depth;
    return Expression;
}
```
Only expressions with zero depth can be returned: `Literal`, `VariableExpression` or `EvalExpression`. If the expression is not a  literal, then it has a form of `Expression || Literal` to prevent returning an empty value.

### VariableInvocationStatement
The statement has the form of `VariableInvocationExpression;`.
