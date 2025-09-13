# CFGS Documentation

## Index
- [Datatypes](#datatypes)
- [Operators](#operators)
- [Postfix](#Postfix-Operators)
- [Variables](#variables)
- [First “warm-up”](#first-warm-up)
- [Functions](#functions)
- [Arrays and Strings](#arrays-and-strings)
- [Enumerations](#Enumerations)
- [Structs](#structs)
- [REPL and Importing](#repl-and-importing)

## Datatypes

CFGS is a dynamic language, so you don't have to declare types for
variables or functions. Basic numeric types (integers through decimals)
are supported, as well as strings, arrays, and structs.

------------------------------------------------------------------------

## Operators

  Operation                    Type  Operator
  ----------------------- --------- ----------
  Addition                   Binary    `+`
  Subtraction                Binary    `-`
  Multiplication             Binary    `*`
  Division                   Binary    `/`
  Exponential                Binary    `**`
  XOR                       Bitwise    `^`
  Bit-Or                    Bitwise    `|`
  Bit-And                   Bitwise    `&`
  Equal                     Boolean    `==`
  Not-Equal                 Boolean    `!=`
  Greater-Than              Boolean    `>`
  Greater-Than or Equal     Boolean    `>=`
  Less-Than                 Boolean    `<`
  Less-Than or Equal        Boolean    `<=`
  Bool-Or                   Boolean    `||`
  Bool-And                  Boolean    `&&`

------------------------------------------------------------------------

## Postfix Operators

Postfix operators apply directly to a variable after its name. CFGS
currently supports the following:

  ------------------------------------------------------------------------
  Operator          Description             Example         Result
  ----------------- ----------------------- --------------- --------------
  `++`              Increments the value of `x = 5; x++;`   `x == 6`
                    a variable by 1                         

  `--`              Decrements the value of `x = 5; x--;`   `x == 4`
                    a variable by 1                         
  ------------------------------------------------------------------------

Postfix operators return the **original value** before applying the
operation:

``` cfgs
x = 5;
y = x++;
print(x); # 6
print(y); # 5 (y got the original value before increment)
```

The same rule applies for the decrement operator:

``` cfgs
x = 5;
y = x--;
print(x); # 4
print(y); # 5 (y got the original value before decrement)
```

------------------------------------------------------------------------

## Variables

To declare a variable, just give it a name and assign a value:

``` cfgs
myvar = 5;
othervar = false;
myarr = [1, 2, ["Hello World", 2], 3.14];
strval = "Hello";
```

We'll cover arrays, strings and structs in more detail below.

## First "warm-up"

CFGS has no required entry point --- you can simply start writing code:

``` cfgs
myvar = 5 + 5; # This is a comment
print("The result is: " + myvar);
```

------------------------------------------------------------------------

## Functions

Define functions with the `func` keyword, an identifier, and parameter
list (parameters can be empty):

``` cfgs
func myfunc(num0, num1)
{
    myvar = num0 + num1;
    return myvar;
}
print("The result is: " + myfunc(5, 5));
```

You may call a function without providing its parameters; omitted
parameters take their default values (e.g., `0`, `false` or `null`
depending on context):

``` cfgs
print("The result is: " + myfunc()); # Result is 0
```

A function may return a value but does not have to. If a function does
not execute a `return`, it returns `null`.

**Important:** functions must be defined before they are called. Forward
declarations (hoisting) are not supported yet:

``` cfgs
mycall();
func mycall() { print("Hello"); }
```

This will fail with an error such as:
`Function not defined: mycall, line Y, column X`.

------------------------------------------------------------------------

## Arrays and Strings

Create arrays with `[]`:

``` cfgs
myarr = [];
myarr = ["Hello", "World"];
```

Access elements by zero-based index:

``` cfgs
print(myarr[0]);    # "Hello"
```

Arrays may be nested:

``` cfgs
myarr = ["Hello", [1, 2, 3], "World"];
print(myarr[1][0]); # 1
```

Deep nesting works the same way:

``` cfgs
myarr = ["Hello", [["Deep Layer", "Deeply"], 2, 3], "World"];
print(myarr[1][0][1]); # Output: Deeply
```

Add and remove elements:

``` cfgs
myarr[] = "New Item";   # Appends "New Item" to the root of myarr
print(myarr[3]);

delete myarr[];         # Deletes the entire array `myarr`
delete myarr[0];        # Deletes the first element of `myarr`
```

Slicing (arrays and strings) uses `[start:end]` where `start` is
inclusive and `end` is exclusive:

``` cfgs
myarr = [1, 2, 3, 4];
arrtwo = myarr[0:2];    # arrtwo == [1, 2]
print(arrtwo[1]);       # 2

arrtwo = myarr[:2];     # same as myarr[0:2]
```

Both slice indices are optional: `[:end]` starts at 0, and `[start:]`
goes to the last element.

------------------------------------------------------------------------

## Enumerations

CFGS also supports **enumerations** (enums), which allow you to define a set of named constants.

```cfgs
enum Colors
{
    red,
    green,
    blue = 4
}

print(Colors.green);
```

In this example:

- `red` is automatically assigned the value `0`.
- `green` is automatically assigned the next value, which is `1`.
- `blue` is explicitly assigned the value `4`.

CFGS continues counting automatically from the last explicitly assigned value. For instance, if another element followed `blue`, it would be assigned `5`.

Enumerations make your code more readable and help avoid using "magic numbers" for related constants.


------------------------------------------------------------------------

## Structs

Structs are supported but currently basic:

``` cfgs
struct fstruct {
    x;
    y;
    z;
}

struct lstruct {
    target;
    source;
}

mystruct = new fstruct();
mystruct.x = new lstruct();
mystruct.x.target = 10;
mystruct.x.source = 20;
print(mystruct.x.source); # 20
```

You define a structure, create an instance with `new`, and then set or
access fields. (Struct methods and richer constructors are in
development.)

------------------------------------------------------------------------

## REPL and Importing

You can load multiple scripts into the interpreter. CFGS has an `import`
statement:

``` cfgs
import "path/to/script.cfgs";
```

Imported scripts are executed immediately when imported. (Scripts loaded
with the `-i` command-line option are not executed automatically.)

### Command-line options

    -i <filepath>            : Loads a specific script (does not execute automatically)
    -w <filepath>            : Sets the path to the main script and the working directory
    -r <code>                : Executes inline code
    -r -f <Functionname>     : Calls a specific function from loaded script(s)
    -r -m                    : Executes the main script

You can combine these options and repeat them. Important: use
`-w [scriptpath]` before importing files if your imports use relative
paths --- `-w` sets the working directory so relative imports resolve
correctly.
