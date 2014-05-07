﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.NodejsTools.Analysis.Values;
using Microsoft.NodejsTools.Parsing;

namespace Microsoft.NodejsTools.Analysis {
    /// <summary>
    /// Builds the global object, builtin functions, etc...
    /// </summary>
    class GlobalBuilder {
        private readonly JsAnalyzer _analyzer;

        private GlobalBuilder(JsAnalyzer analyzer) {
            _analyzer = analyzer;
        }

        public static Globals MakeGlobal(JsAnalyzer analyzer) {
            return new GlobalBuilder(analyzer).MakeGlobal();
        }

        private Globals MakeGlobal() {
            JsAnalyzer analyzer = _analyzer;
            var builtinEntry = analyzer._builtinEntry;

            var stringValue = _analyzer.GetConstant("");
            var boolValue = _analyzer.GetConstant(true);
            var doubleValue = _analyzer.GetConstant(0.0);
            AnalysisValue numberPrototype, stringPrototype, booleanPrototype, functionPrototype;
            FunctionValue arrayFunction;

            var globalObject = new ObjectValue(builtinEntry) {
                (arrayFunction = ArrayFunction()),
                BooleanFunction(out booleanPrototype),
                DateFunction(),
                ErrorFunction(),
                ErrorFunction("EvalError"),
                FunctionFunction(out functionPrototype),
                Member("Infinity", analyzer.GetConstant(double.PositiveInfinity)),
                Member("JSON", MakeJSONObject()),
                Member("Math", MakeMathObject()),
                Member("Infinity", analyzer.GetConstant(double.NaN)),
                NumberFunction(out numberPrototype),
                ObjectFunction(),
                ErrorFunction("RangeError"),
                ErrorFunction("ReferenceError"),
                RegExpFunction(),
                StringFunction(out stringPrototype),
                ErrorFunction("SyntaxError"),
                ErrorFunction("TypeError"),
                ErrorFunction("URIError"),
                ReturningFunction(
                    "decodeURI", 
                    stringValue,
                    "Gets the unencoded version of an encoded Uniform Resource Identifier (URI).",
                    Parameter("encodedURI", "A value representing an encoded URI.")
                ),
                ReturningFunction(
                    "decodeURIComponent", 
                    stringValue,
                    "Gets the unencoded version of an encoded component of a Uniform Resource Identifier (URI).",
                    Parameter("encodedURIComponent", "A value representing an encoded URI component.")
                ),
                ReturningFunction(
                    "encodeURI", 
                    stringValue,
                    "Encodes a text string as a valid Uniform Resource Identifier (URI)",
                    Parameter("uri", "A value representing an encoded URI.")
                ),
                ReturningFunction(
                    "encodeURIComponent", 
                    stringValue,
                    "Encodes a text string as a valid component of a Uniform Resource Identifier (URI).",
                    Parameter("uriComponent", "A value representing an encoded URI component.")
                ),
                ReturningFunction("escape", stringValue),
                BuiltinFunction(
                    "eval",
                    "Evaluates JavaScript code and executes it.",
                    Parameter("x", "A String value that contains valid JavaScript code.")
                ),
                ReturningFunction(
                    "isFinite", 
                    boolValue,
                    "Determines whether a supplied number is finite.",
                    Parameter("number", "Any numeric value.")
                ),
                ReturningFunction(
                    "isNaN", 
                    boolValue,
                    "Returns a Boolean value that indicates whether a value is the reserved value NaN (not a number).",
                    Parameter("number", "A numeric value.")
                ),
                ReturningFunction(
                    "parseFloat", 
                    doubleValue,
                    "Converts a string to a floating-point number.",
                    Parameter("string", "A string that contains a floating-point number.")
                ),
                ReturningFunction(
                    "parseInt", 
                    doubleValue,
                    "Converts A string to an integer.",
                    Parameter("s", "A string to convert into a number."),
                    Parameter("radix", @"A value between 2 and 36 that specifies the base of the number in numString. 
If this argument is not supplied, strings with a prefix of '0x' are considered hexadecimal.
All other strings are considered decimal.", isOptional:true)
                ),
                ReturningFunction("unescape", stringValue),
                Member("undefined", analyzer._undefined),

                SpecializedFunction("require", Require)
            };

            // aliases for global object:
            globalObject.Add("GLOBAL", globalObject);
            globalObject.Add("global", globalObject);
            globalObject.Add("root", globalObject);

            // Node specific stuff:
            //'setImmediate',
            //'setInterval',
            //'setTimeout',
            //'url',
            //'module',
            //'clearImmediate',
            //'clearInterval',
            //'clearTimeout',
            //'ArrayBuffer',
            //'Buffer',
            //'Float32Array',
            //'Float64Array',
            //'Int16Array',
            //'Int32Array',
            //'Int8Array',
            //'Uint16Array',
            //'Uint32Array',
            //'Uint8Array',
            //'Uint8ClampedArray',
            //'COUNTER_HTTP_CLIENT_REQUEST',
            //'COUNTER_HTTP_CLIENT_RESPONSE',
            //'COUNTER_HTTP_SERVER_REQUEST',
            //'COUNTER_HTTP_SERVER_RESPONSE',
            //'COUNTER_NET_SERVER_CONNECTION',
            //'COUNTER_NET_SERVER_CONNECTION_CLOSE',
            //'DTRACE_HTTP_CLIENT_REQUEST',
            //'DTRACE_HTTP_CLIENT_RESPONSE',
            //'DTRACE_HTTP_SERVER_REQUEST',
            //'DTRACE_HTTP_SERVER_RESPONSE',
            //'DTRACE_NET_SERVER_CONNECTION',
            //'DTRACE_NET_SOCKET_READ',
            //'DTRACE_NET_SOCKET_WRITE',
            //'DTRACE_NET_STREAM_END',
            //'DataView',

            // Node modules:
            //'buffer',
            //'child_process',
            //'string_decoder',
            //'querystring',
            //'console',
            //'cluster',
            //'assert',
            //'fs',
            //'punycode',
            //'events',
            //'dgram',
            //'dns',
            //'domain',
            //'path',
            //'process',
            //'http',
            //'https',
            //'net',
            //'os',
            //'crypto',
            //'readline',
            //'require',
            //'stream',
            //'tls',
            //'tty',
            //'util',
            //'vm',
            //'zlib' ]
            return new Globals(
                globalObject, 
                numberPrototype, 
                stringPrototype, 
                booleanPrototype, 
                functionPrototype,
                arrayFunction
            );
        }

        private BuiltinFunctionValue ArrayFunction() {
            var builtinEntry = _analyzer._builtinEntry;

            return new BuiltinFunctionValue(builtinEntry, "Array", createPrototype:false) { 
                Member("prototype", 
                    new ObjectValue(builtinEntry) {
                        BuiltinFunction("concat"),
                        BuiltinFunction("constructor"),
                        BuiltinFunction("every"),
                        BuiltinFunction("filter"),
                        SpecializedFunction("forEach", ArrayForEach),
                        BuiltinFunction("indexOf"),
                        BuiltinFunction("join"),
                        BuiltinFunction("lastIndexOf"),
                        BuiltinFunction("length"),
                        BuiltinFunction("map"),
                        BuiltinFunction("pop"),
                        BuiltinFunction("push"),
                        BuiltinFunction("reduce"),
                        BuiltinFunction("reduceRight"),
                        BuiltinFunction("reverse"),
                        BuiltinFunction("shift"),
                        BuiltinFunction("slice"),
                        BuiltinFunction("some"),
                        BuiltinFunction("sort"),
                        BuiltinFunction("splice"),
                        BuiltinFunction("toLocaleString"),
                        ReturningFunction("toString", _analyzer._emptyStringValue),
                        BuiltinFunction("unshift"),
                    }
                ),
                new ReturningFunctionValue(builtinEntry, "isArray", _analyzer._falseInst)
            };
        }

        private IAnalysisSet ArrayForEach(FunctionValue func, Node node, AnalysisUnit unit, IAnalysisSet @this, IAnalysisSet[] args) {
            if (args.Length >= 1) {
                foreach (var value in @this) {
                    ArrayValue arr = value as ArrayValue;
                    if (arr != null) {
                        for (int i = 0; i < arr.IndexTypes.Length; i++) {
                            foreach (var indexType in arr.IndexTypes) {
                                args[0].Call(
                                    node, 
                                    unit, 
                                    null, 
                                    new IAnalysisSet[] { 
                                        indexType.Types, 
                                        AnalysisSet.Empty, 
                                        @this 
                                    }
                                );
                            }
                        }
                    }
                }
            }
            return _analyzer._undefined;
        }

        private BuiltinFunctionValue BooleanFunction(out AnalysisValue booleanPrototype) {
            var builtinEntry = _analyzer._builtinEntry;
            var prototype = Member("prototype",
                new ObjectValue(builtinEntry) {
                    BuiltinFunction("constructor"),
                    ReturningFunction("toString", _analyzer._emptyStringValue),
                    BuiltinFunction("valueOf"),
                }
            );
            booleanPrototype = prototype.Value;
            return new BuiltinFunctionValue(builtinEntry, "Boolean", createPrototype: false) { 
                prototype
            };
        }

        private BuiltinFunctionValue DateFunction() {
            var builtinEntry = _analyzer._builtinEntry;

            return new BuiltinFunctionValue(builtinEntry, "Date", createPrototype: false) { 
                Member("prototype", 
                    new ObjectValue(builtinEntry) {
                        BuiltinFunction("constructor"),
                        ReturningFunction(
                            "getDate",
                            _analyzer._zeroIntValue,
                            "Gets the day-of-the-month, using local time."
                        ),
                        ReturningFunction(
                            "getDay",
                            _analyzer._zeroIntValue,
                            "Gets the day of the week, using local time."
                        ),
                        ReturningFunction(
                            "getFullYear",
                            _analyzer._zeroIntValue,
                            "Gets the year, using local time."
                        ),
                        ReturningFunction(
                            "getHours",
                            _analyzer._zeroIntValue,
                            "Gets the hours in a date, using local time."
                        ),
                        ReturningFunction(
                            "getMilliseconds",
                            _analyzer._zeroIntValue,
                            "Gets the milliseconds of a Date, using local time."
                        ),
                        ReturningFunction(
                            "getMinutes",
                            _analyzer._zeroIntValue,
                            "Gets the minutes of a Date object, using local time."
                        ),
                        ReturningFunction(
                            "getMonth",
                            _analyzer._zeroIntValue,
                            "Gets the month, using local time."
                        ),
                        ReturningFunction(
                            "getSeconds",
                            _analyzer._zeroIntValue,
                            "Gets the seconds of a Date object, using local time."
                        ),
                        ReturningFunction(
                            "getTime",
                            _analyzer._zeroIntValue,
                            "Gets the time value in milliseconds."
                        ),
                        ReturningFunction(
                            "getTimezoneOffset",
                            _analyzer._zeroIntValue,
                            "Gets the difference in minutes between the time on the local computer and Universal Coordinated Time (UTC)."
                        ),

                        
                        ReturningFunction(
                            "getUTCDate",
                            _analyzer._zeroIntValue,
                            "Gets the day-of-the-month, using Universal Coordinated Time (UTC)."
                        ),
                        ReturningFunction(
                            "getUTCDay",
                            _analyzer._zeroIntValue,
                            "Gets the day of the week using Universal Coordinated Time (UTC)."
                        ),
                        BuiltinFunction("getFullYear"),
                        ReturningFunction(
                            "getUTCHours",
                            _analyzer._zeroIntValue,
                            "Gets the hours value in a Date object using Universal Coordinated Time (UTC)."
                        ),
                        ReturningFunction(
                            "getUTCMilliseconds",
                            _analyzer._zeroIntValue,
                            "Gets the milliseconds of a Date object using Universal Coordinated Time (UTC)."
                        ),
                        ReturningFunction(
                            "getUTCMinutes",
                            _analyzer._zeroIntValue,
                            "Gets the minutes of a Date object using Universal Coordinated Time (UTC)."
                        ),
                        ReturningFunction(
                            "getUTCMonth",
                            _analyzer._zeroIntValue,
                            "Gets the month of a Date object using Universal Coordinated Time (UTC)."
                        ),
                        ReturningFunction(
                            "getUTCSeconds",
                            _analyzer._zeroIntValue,
                            "Gets the seconds of a Date object using Universal Coordinated Time (UTC)."
                        ),
                        ReturningFunction(
                            "getYear",
                            _analyzer._zeroIntValue,
                            "Gets the year minus 2000, using local time."
                        ),
                        ReturningFunction(
                            "setDate",
                            _analyzer._zeroIntValue,
                            "Sets the numeric day-of-the-month value of the Date object using local time. ",
                            Parameter("date", "A numeric value equal to the day of the month.")
                        ),
                        BuiltinFunction(
                            "setFullYear",
                            "Sets the year of the Date object using local time.",
                            Parameter("year", "A numeric value for the year."),
                            Parameter("month", "A zero-based numeric value for the month (0 for January, 11 for December). Must be specified if numDate is specified.", isOptional:true),
                            Parameter("date", "A numeric value equal for the day of the month.", isOptional:true)
                        ),
                        BuiltinFunction(
                            "setHours",
                            "Sets the hour value in the Date object using local time.",
                            Parameter("hours", "A numeric value equal to the hours value."),
                            Parameter("min", "A numeric value equal to the minutes value.", isOptional: true),
                            Parameter("sec", "A numeric value equal to the seconds value.", isOptional: true),
                            Parameter("ms", "A numeric value equal to the milliseconds value.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setMilliseconds",
                            _analyzer._zeroIntValue,
                            "Sets the milliseconds value in the Date object using local time.",
                            Parameter("ms", "A numeric value equal to the millisecond value.")
                        ),
                        ReturningFunction(
                            "setMinutes",
                            _analyzer._zeroIntValue,
                            "Sets the minutes value in the Date object using local time.",
                            Parameter("min", "A numeric value equal to the minutes value."),
                            Parameter("sec", "A numeric value equal to the seconds value.", isOptional: true),
                            Parameter("ms", "A numeric value equal to the milliseconds value.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setMonth",
                            _analyzer._zeroIntValue,
                            "Sets the month value in the Date object using local time.",
                            Parameter("month", "A numeric value equal to the month. The value for January is 0, and other month values follow consecutively."),
                            Parameter("date", "A numeric value representing the day of the month. If this value is not supplied, the value from a call to the getDate method is used.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setSeconds",
                            _analyzer._zeroIntValue,
                            "Sets the seconds value in the Date object using local time.",
                            Parameter("sec", "A numeric value equal to the seconds value."),
                            Parameter("ms", "A numeric value equal to the milliseconds value.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setTime",
                            _analyzer._zeroIntValue,
                            "Sets the date and time value in the Date object.",
                            Parameter("time", "A numeric value representing the number of elapsed milliseconds since midnight, January 1, 1970 GMT.")
                        ),
                        ReturningFunction(
                            "setUTCDate",
                            _analyzer._zeroIntValue,
                            "Sets the numeric day of the month in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("date", "A numeric value equal to the day of the month.")
                        ),
                        ReturningFunction(
                            "setUTCFullYear",
                            _analyzer._zeroIntValue,
                            "Sets the year value in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("year", "A numeric value equal to the year."),
                            Parameter("month", "A numeric value equal to the month. The value for January is 0, and other month values follow consecutively. Must be supplied if numDate is supplied.", isOptional: true),
                            Parameter("date", "A numeric value equal to the day of the month.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setUTCHours",
                            _analyzer._zeroIntValue,
                            "Sets the hours value in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("hours", "A numeric value equal to the hours value."),
                            Parameter("min", "A numeric value equal to the minutes value.", isOptional: true),
                            Parameter("sec", "A numeric value equal to the seconds value.", isOptional: true),
                            Parameter("ms", "A numeric value equal to the milliseconds value.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setUTCMilliseconds",
                            _analyzer._zeroIntValue,
                            "Sets the milliseconds value in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("ms", "A numeric value equal to the millisecond value.")
                        ),
                        ReturningFunction(
                            "setUTCMinutes",
                            _analyzer._zeroIntValue,
                            "Sets the minutes value in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("min", "A numeric value equal to the minutes value."),
                            Parameter("sec", "A numeric value equal to the seconds value. ", isOptional: true),
                            Parameter("ms", "A numeric value equal to the milliseconds value.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setUTCMonth",
                            _analyzer._zeroIntValue,
                            "Sets the month value in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("month", "A numeric value equal to the month. The value for January is 0, and other month values follow consecutively."),
                            Parameter("date", "A numeric value representing the day of the month. If it is not supplied, the value from a call to the getUTCDate method is used.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setUTCSeconds",
                            _analyzer._zeroIntValue,
                            "Sets the seconds value in the Date object using Universal Coordinated Time (UTC).",
                            Parameter("sec", "A numeric value equal to the seconds value."),
                            Parameter("ms", "A numeric value equal to the milliseconds value.", isOptional: true)
                        ),
                        ReturningFunction(
                            "setYear",
                            _analyzer._zeroIntValue
                        ),
                        ReturningFunction(
                            "toDateString",
                            _analyzer._emptyStringValue,
                            "Returns a date as a string value."
                        ),
                        BuiltinFunction("toGMTString"),
                        ReturningFunction(
                            "toISOString",
                            _analyzer._emptyStringValue,
                            "Returns a date as a string value in ISO format."
                        ),
                        ReturningFunction(
                            "toJSON",
                            _analyzer._emptyStringValue,
                            "Used by the JSON.stringify method to enable the transformation of an object's data for JavaScript Object Notation (JSON) serialization."
                        ),
                        ReturningFunction(
                            "toLocaleDateString",
                            _analyzer._emptyStringValue,
                            "Returns a date as a string value appropriate to the host environment's current locale."
                        ),
                        ReturningFunction(
                            "toLocaleString",
                            _analyzer._emptyStringValue,
                            "Returns a value as a string value appropriate to the host environment's current locale."
                        ),
                        ReturningFunction(
                            "toLocaleTimeString",
                            _analyzer._emptyStringValue,
                            "Returns a time as a string value appropriate to the host environment's current locale."
                        ),
                        ReturningFunction(
                            "toString",
                            _analyzer._emptyStringValue,
                            "Returns a string representation of a date. The format of the string depends on the locale."
                        ),
                        ReturningFunction(
                            "toTimeString",
                            _analyzer._emptyStringValue,
                            "Returns a time as a string value."
                        ),

                        BuiltinFunction("toUTCString"),
                        BuiltinFunction("valueOf"),
                    }
                )
            };
        }

        private BuiltinFunctionValue ErrorFunction() {
            var builtinEntry = _analyzer._builtinEntry;

            return new BuiltinFunctionValue(builtinEntry, "Error", createPrototype: false) { 
                Member("prototype", 
                    new ObjectValue(builtinEntry) {
                        BuiltinFunction("constructor"),
                        BuiltinProperty("message", _analyzer._emptyStringValue),
                        BuiltinProperty("name", _analyzer._emptyStringValue),
                        ReturningFunction("toString", _analyzer._emptyStringValue),
                    }
                ),
                new BuiltinFunctionValue(builtinEntry, "captureStackTrace"),
                Member("stackTraceLimit", _analyzer.GetConstant(10.0))
            };
        }

        private BuiltinFunctionValue ErrorFunction(string errorName) {
            var builtinEntry = _analyzer._builtinEntry;

            return new BuiltinFunctionValue(builtinEntry, errorName, createPrototype: false) { 
                Member("prototype", 
                    new ObjectValue(builtinEntry) {
                        BuiltinFunction("arguments"),
                        BuiltinFunction("constructor"),
                        BuiltinProperty("name", _analyzer._emptyStringValue),
                        BuiltinFunction("stack"),
                        BuiltinFunction("type"),
                    }
                )
            };
        }

        private BuiltinFunctionValue FunctionFunction(out AnalysisValue functionPrototype) {
            var builtinEntry = _analyzer._builtinEntry;
            var prototype = Member("prototype",
                new ReturningConstructingFunctionValue(builtinEntry, "Empty", _analyzer._undefined, null) {
                    BuiltinFunction(
                        "apply",
                        "Calls the function, substituting the specified object for the this value of the function, and the specified array for the arguments of the function.",
                        Parameter("thisArg", "The object to be used as the this object."),
                        Parameter("argArray", "A set of arguments to be passed to the function.")
                    ),
                    BuiltinFunction(
                        "bind",
                        @"For a given function, creates a bound function that has the same body as the original function. 
The this object of the bound function is associated with the specified object, and has the specified initial parameters.",
                        Parameter("thisArg", "An object to which the this keyword can refer inside the new function."),
                        Parameter("argArray", "A list of arguments to be passed to the new function.")

                    ),
                    BuiltinFunction(
                        "call",
                        "Calls a method of an object, substituting another object for the current object.",
                        Parameter("thisArg", "The object to be used as the current object."),
                        Parameter("argArray", "A list of arguments to be passed to the method.")
                    ),
                    BuiltinFunction("constructor"),
                    ReturningFunction("toString", _analyzer._emptyStringValue),
                }
            );
            functionPrototype = prototype.Value;
            return new BuiltinFunctionValue(builtinEntry, "Function", createPrototype: false) { 
                prototype
            };
        }

        private ObjectValue MakeJSONObject() {
            var builtinEntry = _analyzer._builtinEntry;

            // TODO: Should we see if we have something that we should parse?
            // TODO: Should we have a per-node value for the result of parse?
            var parseResult = new ObjectValue(builtinEntry);
            return new ObjectValue(builtinEntry) { 
                ReturningFunction(
                    "parse", 
                    parseResult,
                    "Converts a JavaScript Object Notation (JSON) string into an object.",
                    Parameter("text", "A valid JSON string."),
                    Parameter("reviver", @"A function that transforms the results. This function is called for each member of the object. 
If a member contains nested objects, the nested objects are transformed before the parent object is.", isOptional:true)
                ),
                ReturningFunction(
                    "stringify", 
                    _analyzer._emptyStringValue,
                    "Converts a JavaScript value to a JavaScript Object Notation (JSON) string.",
                    Parameter("value", "A JavaScript value, usually an object or array, to be converted.")
                ),
            };
        }

        private ObjectValue MakeMathObject() {
            var builtinEntry = _analyzer._builtinEntry;

            var doubleResult = _analyzer.GetConstant(0.0);
            return new ObjectValue(builtinEntry) { 
                Member("E", _analyzer.GetConstant(Math.E)),
                Member("LN10", doubleResult),
                Member("LN2", doubleResult),
                Member("LOG2E", doubleResult),
                Member("LOG10", doubleResult),
                Member("PI", _analyzer.GetConstant(Math.PI)),
                Member("SQRT1_2", _analyzer.GetConstant(Math.Sqrt(1.0/2.0))),
                Member("SQRT2", _analyzer.GetConstant(Math.Sqrt(2))),
                ReturningFunction(
                    "random", 
                    doubleResult, 
                    "Returns a pseudorandom number between 0 and 1."
                ),
                ReturningFunction(
                    "abs", 
                    doubleResult,
                    @"Returns the absolute value of a number (the value without regard to whether it is positive or negative). 
For example, the absolute value of -5 is the same as the absolute value of 5.",
                    Parameter("x", "A numeric expression for which the absolute value is needed.")
                ),
                ReturningFunction(
                    "acos", 
                    doubleResult,
                    "Returns the arc cosine (or inverse cosine) of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "asin", 
                    doubleResult,
                    "Returns the arcsine of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "atan", 
                    doubleResult,
                    "Returns the arctangent of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "ceil", 
                    doubleResult,
                    "Returns the smallest number greater than or equal to its numeric argument.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "cos", 
                    doubleResult,
                    "Returns the cosine of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "exp", 
                    doubleResult,
                    "Returns e (the base of natural logarithms) raised to a power.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "floor", 
                    doubleResult,
                    "Returns the greatest number less than or equal to its numeric argument.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "log", 
                    doubleResult,
                    "Returns the natural logarithm (base e) of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "round", 
                    doubleResult,
                    "Returns a supplied numeric expression rounded to the nearest number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "sin", 
                    doubleResult,
                    "Returns the sine of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "sqrt", 
                    doubleResult,
                    "Returns the square root of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "tan", 
                    doubleResult,
                    "Returns the tangent of a number.",
                    Parameter("x", "A numeric expression.")
                ),
                ReturningFunction(
                    "atan2", 
                    doubleResult,
                    "Returns the smallest number greater than or equal to its numeric argument.",
                    Parameter("y", "A numeric expression representing the cartesian y-coordinate."),
                    Parameter("x", "A numeric expression representing the cartesian x-coordinate.")
                ),
                ReturningFunction(
                    "pow", 
                    doubleResult,
                    "Returns the value of a base expression taken to a specified power.",
                    Parameter("x", "The base value of the expression."),
                    Parameter("y", "The exponent value of the expression.")
                ),
                ReturningFunction(
                    "max", 
                    doubleResult,
                    "Returns the larger of a set of supplied numeric expressions.",
                    Parameter("x", "Numeric expressions to be evaluated."),
                    Parameter("y...", "Numeric expressions to be evaluated.")
                ),
                ReturningFunction(
                    "min", 
                    doubleResult,
                    "Returns the smaller of a set of supplied numeric expressions. ",
                    Parameter("x", "Numeric expressions to be evaluated."),
                    Parameter("y...", "Numeric expressions to be evaluated.")
                ),
            };
        }

        private BuiltinFunctionValue NumberFunction(out AnalysisValue numberPrototype) {
            var builtinEntry = _analyzer._builtinEntry;

            var prototype = Member("prototype", 
                new ObjectValue(builtinEntry) {
                    BuiltinFunction("constructor"),
                    ReturningFunction(
                        "toExponential",
                        _analyzer._emptyStringValue,
                        "Returns a string containing a number represented in exponential notation."
                    ),
                    ReturningFunction(
                        "toFixed",
                        _analyzer._emptyStringValue,
                        "Returns a string representing a number in fixed-point notation."
                    ),
                    BuiltinFunction("toLocaleString"),
                    BuiltinFunction(
                        "toPrecision",
                        "Returns a string containing a number represented either in exponential or fixed-point notation with a specified number of digits.",
                        Parameter("precision", "Number of significant digits. Must be in the range 1 - 21, inclusive.", isOptional: true)
                    ),
                    ReturningFunction(
                        "toString",
                        _analyzer._emptyStringValue,
                        "Returns a string representation of an object.",
                        Parameter("radix", "Specifies a radix for converting numeric values to strings. This value is only used for numbers.", isOptional: true)
                    ),
                    BuiltinFunction("valueOf"),
                }
            );
            numberPrototype = prototype.Value;

            return new BuiltinFunctionValue(builtinEntry, "Number", createPrototype: false) { 
                prototype,
                Member("length", _analyzer.GetConstant(1.0)),
                Member("name", _analyzer.GetConstant("Number")),
                Member("arguments", _analyzer._nullInst),
                Member("caller", _analyzer._nullInst),
                Member("MAX_VALUE", _analyzer.GetConstant(Double.MaxValue)),
                Member("MIN_VALUE", _analyzer.GetConstant(Double.MinValue)),
                Member("NaN", _analyzer.GetConstant(Double.NaN)),
                Member("NEGATIVE_INFINITY", _analyzer.GetConstant(Double.NegativeInfinity)),
                Member("POSITIVE_INFINITY", _analyzer.GetConstant(Double.PositiveInfinity)),
                ReturningFunction(
                    "isFinite", 
                    _analyzer._trueInst,
                    "Determines whether a supplied number is finite."
                ),
                ReturningFunction(
                    "isNaN", 
                    _analyzer._falseInst,
                    "Returns a Boolean value that indicates whether a value is the reserved value NaN (not a number)."
                ),
            };
        }

        private BuiltinFunctionValue ObjectFunction() {
            var builtinEntry = _analyzer._builtinEntry;

            return new BuiltinFunctionValue(builtinEntry, "Object") { 
                BuiltinFunction(
                    "getPrototypeOf",
                    "Returns the prototype of an object."
                ),
                BuiltinFunction(
                    "getOwnPropertyDescriptor",
                    @"Gets the own property descriptor of the specified object. 
An own property descriptor is one that is defined directly on the object and is not inherited from the object's prototype. ",
                    Parameter("o", "Object that contains the property."),
                    Parameter("p", "Name of the property.")
                ),
                BuiltinFunction(
                    "getOwnPropertyNames",
                    @"Returns the names of the own properties of an object. The own properties of an object are those that are defined directly 
on that object, and are not inherited from the object's prototype. The properties of an object include both fields (objects) and functions.",
                    Parameter("o", "Object that contains the own properties.")

                ),
                BuiltinFunction(
                    "create",
                    "Creates an object that has the specified prototype, and that optionally contains specified properties.",
                    Parameter("o", "Object to use as a prototype. May be null"),
                    Parameter("properties", "JavaScript object that contains one or more property descriptors.")
                ),
                SpecializedFunction(
                    "defineProperty",
                    DefineProperty,
                    "Adds a property to an object, or modifies attributes of an existing property.",
                    Parameter("o", "Object on which to add or modify the property. This can be a native JavaScript object (that is, a user-defined object or a built in object) or a DOM object."),
                    Parameter("p", "The property name."),
                    Parameter("attributes", "Descriptor for the property. It can be for a data property or an accessor property.")
                ),
                SpecializedFunction(
                    "defineProperties", 
                    DefineProperties,
                    "Adds one or more properties to an object, and/or modifies attributes of existing properties.",
                    Parameter("o", "Object on which to add or modify the properties. This can be a native JavaScript object or a DOM object."),
                    Parameter("properties", "JavaScript object that contains one or more descriptor objects. Each descriptor object describes a data property or an accessor property.")
                ),
                BuiltinFunction(
                    "seal",
                    "Prevents the modification of attributes of existing properties, and prevents the addition of new properties.",
                    Parameter("o", "Object on which to lock the attributes.")
                ),
                BuiltinFunction(
                    "freeze",
                    "Prevents the modification of existing property attributes and values, and prevents the addition of new properties.",
                    Parameter("o", "Object on which to lock the attributes.")
                ),
                BuiltinFunction(
                    "preventExtensions",
                    "Prevents the addition of new properties to an object.",
                    Parameter("o", "Object to make non-extensible.")
                ),
                ReturningFunction(
                    "isSealed",
                    _analyzer._trueInst,
                    "Returns true if existing property attributes cannot be modified in an object and new properties cannot be added to the object.",
                    Parameter("o", "Object to test. ")
                ),
                ReturningFunction(
                    "isFrozen",
                    _analyzer._trueInst,
                    "Returns true if existing property attributes and values cannot be modified in an object, and new properties cannot be added to the object.",
                    Parameter("o", "Object to test.")
                ),
                ReturningFunction(
                    "isExtensible",
                    _analyzer._trueInst,
                    "Returns a value that indicates whether new properties can be added to an object.",
                    Parameter("o", "Object to test.")
                ),
                BuiltinFunction(
                    "keys",
                    "Returns the names of the enumerable properties and methods of an object.",
                    Parameter("o", "Object that contains the properties and methods. This can be an object that you created or an existing Document Object Model (DOM) object.")
                ),
                BuiltinFunction("is"),
            };
        }

        private static IAnalysisSet DefineProperty(FunctionValue func, Node node, AnalysisUnit unit, IAnalysisSet @this, IAnalysisSet[] args) {
            // object, name, property desc
            if (args.Length >= 3) {
                foreach (var obj in args[0]) {
                    ExpandoValue expando = obj as ExpandoValue;
                    if (expando != null) {
                        foreach (var name in args[1]) {
                            string propName = name.GetConstantValueAsString();
                            if (propName != null) {
                                foreach (var desc in args[2]) {
                                    expando.AddProperty(node, unit, propName, desc);
                                }
                            }
                        }
                    }
                }
            }
            if (args.Length > 0) {
                return args[0];
            }
            return AnalysisSet.Empty;
        }

        private static IAnalysisSet DefineProperties(FunctionValue func, Node node, AnalysisUnit unit, IAnalysisSet @this, IAnalysisSet[] args) {
            // object, {propName: {desc}, ...}
            if (args.Length >= 2) {
                foreach (var obj in args[0]) {
                    ExpandoValue target = obj as ExpandoValue;
                    if (target != null) {
                        foreach (var properties in args[1]) {
                            ExpandoValue propsObj = properties as ExpandoValue;
                            if (propsObj != null) {
                                foreach (var keyValue in propsObj.Descriptors) {
                                    foreach (var propValue in propsObj.GetMember(node, unit, keyValue.Key)) {
                                        target.AddProperty(
                                            node,
                                            unit,
                                            keyValue.Key,
                                            propValue
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (args.Length > 0) {
                return args[0];
            }
            return AnalysisSet.Empty;
        }

        private static IAnalysisSet Require(FunctionValue func, Node node, AnalysisUnit unit, IAnalysisSet @this, IAnalysisSet[] args) {
            IAnalysisSet res = AnalysisSet.Empty;
            if (args.Length > 0) {
                foreach (var arg in args[0]) {
                    var moduleName = arg.GetConstantValueAsString();
                    if (moduleName != null) {
                        res = res.Union(
                            unit.Analyzer.Modules.RequireModule(
                                node,
                                unit,
                                moduleName, 
                                unit.DeclaringModuleEnvironment.Name
                            )
                        );
                    }
                }
            }
            return res;
        }

        private BuiltinFunctionValue RegExpFunction() {
            var builtinEntry = _analyzer._builtinEntry;

            return new BuiltinFunctionValue(builtinEntry, "RegExp", createPrototype: false) { 
                Member("prototype", 
                    new ObjectValue(builtinEntry) {
                        BuiltinFunction("compile"),   
                        BuiltinFunction("constructor"),   
                        BuiltinFunction(
                            "exec",
                            "Executes a search on a string using a regular expression pattern, and returns an array containing the results of that search.",
                            Parameter("string", "The String object or string literal on which to perform the search.")
                        ),  
                        BuiltinProperty("global", _analyzer._trueInst),  
                        BuiltinProperty("ignoreCase", _analyzer._trueInst),  
                        BuiltinProperty("lastIndex", _analyzer._zeroIntValue),  
                        BuiltinProperty("multiline", _analyzer._trueInst),  
                        BuiltinProperty("source", _analyzer._emptyStringValue),  
                        ReturningFunction(
                            "test",
                            _analyzer._trueInst,
                            "Returns a Boolean value that indicates whether or not a pattern exists in a searched string.",
                            Parameter("string", "String on which to perform the search.")
                        ),  
                        ReturningFunction("toString", _analyzer._emptyStringValue) 
                    }
                ),
// TODO:   input: [Getter/Setter],
//  lastMatch: [Getter/Setter],
//  lastParen: [Getter/Setter],
//  leftContext: [Getter/Setter],
//  rightContext: [Getter/Setter],
//  '$1': [Getter/Setter],
//  '$2': [Getter/Setter],
//  '$3': [Getter/Setter],
//  '$4': [Getter/Setter],
//  '$5': [Getter/Setter],
//  '$6': [Getter/Setter],
//  '$7': [Getter/Setter],
//  '$8': [Getter/Setter],
//  '$9': [Getter/Setter] }
//[ '$&',
//  '$\'',
//  '$*',
//  '$+',
//  '$_',
//  '$`',
//  '$input',
                BuiltinProperty("multiline", _analyzer._falseInst),
                BuiltinFunction("arguments"),
                BuiltinFunction("caller"),
                BuiltinFunction("input"),
                BuiltinFunction("lastMatch"),
                BuiltinFunction("lastParen"),
                BuiltinFunction("leftContext"),
                BuiltinFunction("length"),
                BuiltinFunction("multiline"),
                BuiltinFunction("name"),
                BuiltinFunction("rightContext") 
            };
        }

        private BuiltinFunctionValue StringFunction(out AnalysisValue stringPrototype) {
            var builtinEntry = _analyzer._builtinEntry;
            var prototype = Member("prototype", 
                new ObjectValue(builtinEntry) {
                    ReturningFunction(
                        "anchor",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with an <a name=...> tag.",
                        Parameter("name", "the name attribute for the anchor")
                    ),
                    ReturningFunction(
                        "big",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <big> tag."
                    ),
                    ReturningFunction(
                        "blink",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <blink> tag."
                    ),
                    ReturningFunction(
                        "bold",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <bold> tag."
                    ),
                    ReturningFunction(
                        "charAt", 
                        _analyzer._emptyStringValue,
                        "Returns the character at the specified index.", 
                        Parameter("pos", "The zero-based index of the desired character.")
                    ),
                    ReturningFunction(
                        "charCodeAt", 
                        _analyzer._zeroIntValue,
                        "Returns the Unicode value of the character at the specified location.", 
                        Parameter("index", "The zero-based index of the desired character. If there is no character at the specified index, NaN is returned.")
                    ),
                    BuiltinFunction("concat", 
                        "Returns a string that contains the concatenation of two or more strings.", 
                        Parameter("...")
                    ),
                    BuiltinFunction("constructor"),
                    ReturningFunction(
                        "fixed",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <tt> tag."
                    ),
                    ReturningFunction(
                        "fontcolor",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <font color=...> tag.",
                        Parameter("color", "the color attribute for the font tag")
                    ),
                    ReturningFunction(
                        "fontsize",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <font size=...> tag.",
                        Parameter("size", "the size attribute for the font tag")
                    ),
                    BuiltinFunction("indexOf", 
                        "Returns the position of the first occurrence of a substring.", 
                        Parameter("searchString", "The substring to search for in the string"), 
                        Parameter("position", "The index at which to begin searching the String object. If omitted, search starts at the beginning of the string.", true)
                    ),
                    ReturningFunction(
                        "italics",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with an <i> tag."
                    ),
                    BuiltinFunction(
                        "lastIndexOf", 
                        "Returns the last occurrence of a substring in the string.", 
                        Parameter("searchString", "The substring to search for."), 
                        Parameter("position", "The index at which to begin searching. If omitted, the search begins at the end of the string.", true)
                    ),
                    BuiltinProperty("length", _analyzer._zeroIntValue),
                    ReturningFunction(
                        "link",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with an <a href=...> tag.",
                        Parameter("href", "the href attribute for the tag")
                    ),
                    BuiltinFunction(
                        "localeCompare", 
                        "Determines whether two strings are equivalent in the current locale.",
                        Parameter("that", "String to compare to target string")
                    ),
                    BuiltinFunction(
                        "match",
                        "Matches a string with a regular expression, and returns an array containing the results of that search.",
                        Parameter("regexp", "A string containing the regular expression pattern and flags or a RegExp.")

                    ),
                    BuiltinFunction(
                        "replace",
                        "Replaces text in a string, using a regular expression or search string.",
                        Parameter("searchValue", "A string that represents the regular expression or a RegExp"),
                        Parameter("replaceValue", "A string containing the text replacement text or a function which returns it.")
                    ),
                    BuiltinFunction(
                        "search",
                        "Finds the first substring match in a regular expression search.",
                        Parameter("regexp", "The regular expression pattern and applicable flags.")
                    ),
                    BuiltinFunction(
                        "slice",
                        "Returns a section of a string.",
                        Parameter("start", "The index to the beginning of the specified portion of stringObj."),
                        Parameter("end", "The index to the end of the specified portion of stringObj. The substring includes the characters up to, but not including, the character indicated by end.  If this value is not specified, the substring continues to the end of stringObj.")
                    ),
                    ReturningFunction(
                        "small",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <small> tag."
                    ),
                    BuiltinFunction(
                        "split",
                        "Split a string into substrings using the specified separator and return them as an array.",
                        Parameter("separator", "A string that identifies character or characters to use in separating the string. If omitted, a single-element array containing the entire string is returned. "),
                        Parameter("limit", "A value used to limit the number of elements returned in the array.", isOptional: true)
                    ),
                    ReturningFunction(
                        "strike",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <strike> tag."
                    ),
                    ReturningFunction(
                        "sub",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <sub> tag."
                    ),
                    BuiltinFunction("substr"),
                    BuiltinFunction(
                        "substring",
                        "Returns the substring at the specified location within a String object. ",
                        Parameter("start", "The zero-based index number indicating the beginning of the substring."),
                        Parameter("end", "Zero-based index number indicating the end of the substring. The substring includes the characters up to, but not including, the character indicated by end.  If end is omitted, the characters from start through the end of the original string are returned.")
                    ),
                    ReturningFunction(
                        "sup",
                        _analyzer._emptyStringValue,
                        "Surrounds the provided string with a <sup> tag."
                    ),
                    BuiltinFunction(
                        "toLocaleLowerCase",
                        "Converts all alphabetic characters to lowercase, taking into account the host environment's current locale."
                    ),
                    BuiltinFunction(
                        "toLocaleUpperCase",
                        "Returns a string where all alphabetic characters have been converted to uppercase, taking into account the host environment's current locale."
                    ),
                    BuiltinFunction(
                        "toLowerCase",
                        "Converts all the alphabetic characters in a string to lowercase."
                    ),
                    BuiltinFunction(
                        "toString",
                        "Returns a string representation of a string."
                    ),
                    BuiltinFunction(
                        "toUpperCase",
                        "Converts all the alphabetic characters in a string to uppercase."
                    ),
                    BuiltinFunction(
                        "trim",
                        "Removes the leading and trailing white space and line terminator characters from a string."
                    ),
                    BuiltinFunction(
                        "trimLeft",
                        "Removes the leading white space and line terminator characters from a string."
                    ),
                    BuiltinFunction(
                        "trimRight",
                        "Removes the trailing white space and line terminator characters from a string."
                    ),
                    BuiltinFunction("valueOf"),
                }
            );
            stringPrototype = prototype.Value;

            return new BuiltinFunctionValue(builtinEntry, "String", createPrototype: false) { 
                prototype,
                ReturningFunction("fromCharCode", _analyzer.GetConstant("")),
            };
        }

        #region Building Helpers

        private static MemberAddInfo Member(string name, AnalysisValue value) {
            return new MemberAddInfo(name, value);
        }

        private ParameterResult Parameter(string name, string doc = null, bool isOptional = false) {
            return new ParameterResult(name, doc, null, isOptional);
        }

        private BuiltinFunctionValue BuiltinFunction(string name, string documentation = null, params ParameterResult[] signature) {
            return new BuiltinFunctionValue(_analyzer._builtinEntry, name, documentation, true, signature);
        }

        private BuiltinFunctionValue ReturningFunction(string name, AnalysisValue value, string documentation = null, params ParameterResult[] parameters) {
            return new ReturningFunctionValue(_analyzer._builtinEntry, name, value, documentation, true, parameters);
        }

        private BuiltinFunctionValue SpecializedFunction(string name, CallDelegate value, string documentation = null, params ParameterResult[] parameters) {
            return new SpecializedFunctionValue(_analyzer._builtinEntry, name, value, documentation, parameters);
        }

        private MemberAddInfo BuiltinProperty(string name, AnalysisValue propertyType, string documentation = null) {
            return new MemberAddInfo(name, propertyType, documentation, isProperty: true);
        }

        #endregion
    }

    class Globals {
        public readonly ObjectValue GlobalObject;
        public readonly AnalysisValue NumberPrototype,
            StringPrototype,
            BooleanPrototype,
            FunctionPrototype;
        public readonly FunctionValue ArrayFunction;

        public Globals(ObjectValue globalObject, AnalysisValue numberPrototype, AnalysisValue stringPrototype, AnalysisValue booleanPrototype, AnalysisValue functionPrototype, FunctionValue arrayFunction) {
            GlobalObject = globalObject;
            NumberPrototype = numberPrototype;
            StringPrototype = stringPrototype;
            BooleanPrototype = booleanPrototype;
            FunctionPrototype = functionPrototype;
            ArrayFunction = arrayFunction;
        }
    }
}
