# RandomJS Generator Documentation

The general outline of the generated program is following:

```javascript
//1. Strict mode declaration
'use strict';

//2. Definition of global helper functions, constants and variables
let __depth = 0;
const __maxDepth = 3;
function __tryc(_, __) {
    try {
        return _();
    } catch (_e) {
        return _e.name + __;
    }
}
//etc.

//3. Definition of randomly generated global variables
let a = ...
let b = ...
let c = ...
//etc.

//4. Output statements
__prnt(__invk(b, ...));
__prnt(__invk(c, ...));
__prnt(__invk(a, ...));
//etc.
```

* All programs run in [strict mode](http://www.ecma-international.org/ecma-262/6.0/#sec-strict-mode-code) to prevent some problematic behavior of javascript.
* The program begins with definitions of helper functions, constants and variables. The order of these helper definitions is pseudo-random (a helper function is attached to the scope upon being first referenced from the main code).
* The number of global variables is generated at random from a specified interval.
* Each program prints its global variables in random order.
* The global scope has no statements apart variable definitions and output. Other statements are restricted inside functions.
* All variables in the program are block scoped using the [let or const declaration](http://www.ecma-international.org/ecma-262/6.0/#sec-let-and-const-declarations) (unlike [var declarations](http://www.ecma-international.org/ecma-262/6.0/#sec-variable-statement) which are function scoped).

## Helper functions and variables

These are represented by the `Global` abstract class. The names of all these global definitions begin with two underscores to prevent collisions with randomly generated variables.

#### Call depth variables
```javascript
let __depth = 0;
const __maxDepth = 3;
```
These variables are used to prevent infinite recursion during program execution. `__depth` represents the current depth of the call stack and `__maxDepth` is the maximum value (this is randomly generated from a specified interval).

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
This helper function is used for operations which can throw an error. The first parameter is a function which can fail and the second parameter is the default value. In case of an error, the [name](http://www.ecma-international.org/ecma-262/6.0/#sec-error.prototype.name) of the error is prepended to the default value. This is safe because the names of common errors, such as [SyntaxError](http://www.ecma-international.org/ecma-262/6.0/#sec-native-error-types-used-in-this-standard-syntaxerror), [ReferenceError](http://www.ecma-international.org/ecma-262/6.0/#sec-native-error-types-used-in-this-standard-referenceerror) or [TypeError](http://www.ecma-international.org/ecma-262/6.0/#sec-native-error-types-used-in-this-standard-typeerror) are part of the language specification (unlike the error message, which is implementation-defined).

#### OTST override
```javascript
Object.prototype.toString = function() {
    return __tryc(() => JSON.stringify(this), '[Object]');
};
```
This overrides the default `Object.toString` function by converting the object to its JSON representation (the default `toString` function just returns `[object o]`). The JSON conversion will throw a TypeError if the object is circular (that's why the `__tryc` function is used).

#### OVOF override
```javascript
Object.prototype.valueOf = function() {
    for (let _ in this)
        if (typeof this[_] === 'number') return this[_];
    return this;
};
```
This overrides the default `Object.valueOf` function. The function is called whenever an object has to be converted to a primitive value. In this case, we return the first numeric value among the object's properties. This increases the output variability because an object can be represented either by a JSON string or as a number (if a numeric operation was attempted on the object).

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
This function is used whenever a mathematical operation with inexact result is performed (for example `Math.exp` or other trancendental math functions). The function rounds off the number to a specific number of decimal significant digits given by the `__fpMathPrec` constant. The value of this constant is generated at random from interval 10-14. It has been determined empirically that the first 14 significant digits match across different platforms. 

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
This function is used to get the string representation of a variable for printing. It calls the object's `toString` function which behaves differently depending on the type of the argument (for example `function.prototype.toString` returns the source code of the function).

#### PRNT function
```javascript
function __prnt(_) {
    console.log(__tstr(_));
}
```
This function is used to output global variables at the end of the program.

## Expressions

Expressions in the program are generated at random from the following list:
* Literal
* AssignmentExpression
* VariableInvocationExpression
* FunctionInvocationExpression
* FunctionExpression
* UnaryExpression
* BinaryExpression
* TernaryExpression
* EvalExpression
* VariableExpression
* ObjectConstructorExpression
* ObjectSetExpression

Each experession has a specified probability of being generated. Expressions can be nested up to the specified depth.

#### EvalExpression
One of the features of the javascript language is the possiblity of evaluating code at runtime. The `EvalExpression` evaluates a random string in the current scope using the EVAL helper function (see above). The evaluated string always contains 10 random characters from the following character set: ``/cb1/|=\`+-a2+e84``. It has been empirically determined that this character set produces relatively low number of syntax errors, while exploring many features of the language. Two characters are included twice (`/` and `+`) so they have a double chance of occuring. Thus, there are 14 unique characters, giving 14<sup>10</sup> total possibilities (~290 billion). This is sufficiently high to prevent dictionary lookup and forces all implementations to include a javascript parser.

The result of `EvalExpression` has approximately the following distribution:
* Slightly over 5% chance of being a valid javascript expression.
* Around 18% chance of producing an error other than `SyntaxError` (most common are `ReferenceError` and `TypeError`).
* The remaining ~77% is a `SyntaxError`.

The following table lists some examples of `EvalExpression`:

|Expression|Result|
|----------|------|
|<code>\_\_eval(_ => eval(_), 'ab=e28&#124;/-c')</code> | <code> SyntaxErrorab=e28&#124;/-c </code> (invalid character)|
|<code>\_\_eval(_ => eval(_), '++/e+=&#124;12/')</code> | <code>ReferenceError++/e+=&#124;12/</code> (attempt to increment a regular expression)|
|<code>\_\_eval(_ => eval(_), 'e//42/b+e2')<code> | value of the `e` variable in the current scope (assuming it's declared)|
|<code>\_\_eval(_ => eval(_), '+1+4//2-&#124;-')</code> | `5`|
|<code>\_\_eval(_ => eval(_), '//++&#124;&#124;4/c2')</code> | `undefined` (the expression is a comment)|
|<code>\_\_eval(_ => eval(_), 'a\`&#124;/=2-=2\`')</code> | assuming variable `a` is a function, this evaluates a [tagged template](http://www.ecma-international.org/ecma-262/6.0/#sec-tagged-templates)|

