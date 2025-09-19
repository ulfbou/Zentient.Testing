===== src/Zentient.Abstractions.Expressions/ExpressionKind.cs =====
// <copyright file="ExpressionKind.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Zentient.Abstractions.Expressions
{
    /// <summary>
    /// Specifies the kind of expression.
    /// </summary>
    public enum ExpressionKind
    {
        /// <summary>
        /// An expression representing a constant value.
        /// </summary>
        Constant,

        /// <summary>
        /// An expression representing an identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// An expression representing member access.
        /// </summary>
        MemberAccess,

        /// <summary>
        /// An expression representing a method call.
        /// </summary>
        MethodCall,

        /// <summary>
        /// An expression representing a lambda expression.
        /// </summary>
        Lambda
    }
}


===== src/Zentient.Abstractions.Expressions/IExpression.cs =====
// <copyright file="IExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions.Expressions
{
    /// <summary>
    /// Represents an expression in the Zentient abstraction.
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// Gets the kind of the expression.
        /// </summary>
        ExpressionKind Kind { get; }

        /// <summary>
        /// Gets the canonical string representation of the expression.
        /// </summary>
        string Canonical { get; }

        /// <summary>
        /// Gets the operands of the expression.
        /// </summary>
        IReadOnlyList<IExpression> Operands { get; }
    }
}


===== src/Zentient.Abstractions.Expressions/IExpressionEvaluator.cs =====
// <copyright file="IExpressionEvaluator.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions.Expressions
{
    /// <summary>
    /// Evaluates expressions to obtain their numeric value.
    /// </summary>
    public interface IExpressionEvaluator
    {
        /// <summary>
        /// Evaluates the specified expression in the given context.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="context">The context in which to evaluate the expression, or <c>null</c> for no context.</param>
        /// <returns>The numeric value of the expression.</returns>
        object? Evaluate(IExpression expression, object? context = null);
    }
}


===== src/Zentient.Abstractions.Expressions/IExpressionParser.cs =====
// <copyright file="IExpressionParser.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions.Expressions
{
    /// <summary>
    /// Parses expressions from strings.
    /// </summary>
    public interface IExpressionParser
    {
        /// <summary>
        /// Attempts to parse the input string as an expression.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="expression">When this method returns, contains the parsed expression if the parse was successful, or <c>null</c> otherwise.</param>
        /// <param name="diagnostics">When this method returns, contains the list of diagnostics produced during parsing.</param>
        /// <returns><c>true</c> if the parse was successful; otherwise, <c>false</c>.</returns>
        bool TryParse(string input, out IExpression? expression, out IReadOnlyList<ParseDiagnostic> diagnostics);

        /// <summary>
        /// Parses the input string as an expression.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The parsed expression.</returns>
        /// <exception cref="ParseException">Thrown if the input string cannot be parsed as an expression.</exception>
        IExpression Parse(string input);
    }
}


===== src/Zentient.Abstractions.Expressions/ITypedExpression{out T}.cs =====
// <copyright file="ITypedExpression{out T}.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions.Expressions
{
    /// <summary>
    /// Represents a typed expression that can be evaluated to a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value that the expression evaluates to.</typeparam>
    public interface ITypedExpression<out T> : IExpression
    {
        /// <summary>
        /// Evaluates the expression in the specified context.
        /// </summary>
        /// <param name="context">The context in which to evaluate the expression, or <c>null</c> for no context.</param>
        /// <returns>The result of the evaluation.</returns>
        T Evaluate(object? context = null);
    }
}



===== src/Zentient.Abstractions.Expressions/ParseDiagnostic.cs =====
// <copyright file="ParseDiagnostic.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Abstractions.Expressions
{
    /// <summary>
    /// Severity for parse diagnostics.
    /// </summary>
    public enum ParseDiagnosticSeverity
    {
        Error,
        Warning,
        Information
    }

    /// <summary>
    /// Represents a diagnostic message produced during parsing.
    /// </summary>
    /// <param name="Position">The position in the input where the diagnostic occurred.</param>
    /// <param name="Message">The diagnostic message.</param>
    /// <param name="Severity">The diagnostic severity (default: Error).</param>
    public record ParseDiagnostic(int Position, string Message, ParseDiagnosticSeverity Severity = ParseDiagnosticSeverity.Error);
}


===== src/Zentient.Expressions/ConstantExpression.cs =====
// <copyright file="ConstantExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System.Globalization;

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Represents a literal constant expression.
    /// </summary>
    internal sealed class ConstantExpression : ExpressionBase
    {
        /// <summary>
        /// Gets the constant value represented by this node.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Creates a new <see cref="ConstantExpression"/> with the given value.
        /// </summary>
        /// <param name="value">The constant value (may be <c>null</c>).</param>
        public ConstantExpression(object? value) => Value = value;

        /// <inheritdoc />
        public override ExpressionKind Kind => ExpressionKind.Constant;

        /// <inheritdoc />
        public override IReadOnlyList<IExpression> Operands => Array.Empty<IExpression>();

        /// <summary>
        /// Produces a canonical string representation of the value.
        /// Strings are quoted and escape sequences are applied; <c>null</c> renders as "null".
        /// </summary>
        public override string Canonical
            => Value switch
            {
                string s => $"\"{EscapeString(s)}\"",
                null => "null",
                _ => Convert.ToString(Value, CultureInfo.InvariantCulture) ?? "null"
            };

        // Escape backslash first, then other characters
        private static string EscapeString(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var sb = new System.Text.StringBuilder();
            foreach (var ch in s)
            {
                switch (ch)
                {
                    case '\\': sb.Append("\\\\"); break; // backslash => \\\\ in string literal
                    case '"': sb.Append("\\\""); break;   // quote => \"
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}


===== src/Zentient.Expressions/ExpressionBase.cs =====
// <copyright file="ExpressionBase.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Base implementation for expression AST nodes.
    /// </summary>
    internal abstract class ExpressionBase : IExpression
    {
        /// <summary>
        /// Gets the kind of this expression node.
        /// </summary>
        public abstract ExpressionKind Kind { get; }

        /// <summary>
        /// Gets a canonical, language-independent representation of this expression node.
        /// </summary>
        public abstract string Canonical { get; }

        /// <summary>
        /// Gets the child operands of this expression node. Leaf nodes typically return an empty collection.
        /// </summary>
        public virtual IReadOnlyList<IExpression> Operands => Array.Empty<IExpression>();
    }
}


===== src/Zentient.Expressions/ExpressionDebugExtensions.cs =====
// <copyright file="ExpressionDebugExtensions.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Debugging and utility extensions for expressions.
    /// </summary>
    public static class ExpressionDebugExtensions
    {
        /// <summary>
        /// Returns the canonical string representation of the specified expression.
        /// </summary>
        /// <param name="expr">The expression to obtain the canonical representation for.</param>
        /// <returns>A canonical, language-independent string for <paramref name="expr"/>.</returns>
        public static string ToCanonicalString(this IExpression expr)
            => expr.Canonical;
    }
}


===== src/Zentient.Expressions/ExpressionParser.cs =====
// <copyright file="ExpressionParser.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zentient.Expressions
{
    /// <summary>
    /// Internal parsing helpers that produce an <see cref="IExpression"/> and diagnostics.
    /// </summary>
    internal static class ExpressionParser
    {
        /// <summary>
        /// Attempts to parse <paramref name="input"/> into an <see cref="IExpression"/> and collects diagnostics.
        /// </summary>
        /// <param name="input">The input expression text to parse.</param>
        /// <param name="expression">When this method returns, contains the parsed expression if parsing succeeded; otherwise <c>null</c>.</param>
        /// <param name="diagnostics">When this method returns, contains a read-only list of diagnostics observed during parsing.</param>
        /// <returns><c>true</c> when parsing succeeded with no diagnostics; otherwise <c>false</c>.</returns>
        public static bool TryParse(
            string input,
            out IExpression? expression,
            out IReadOnlyList<ParseDiagnostic> diagnostics)
        {
            var diags = new List<ParseDiagnostic>();

            if (string.IsNullOrWhiteSpace(input))
            {
                diags.Add(new ParseDiagnostic(0, "Expression is empty or whitespace."));
                expression = null;
                diagnostics = diags;
                return false;
            }

            var parser = new Parser(input);
            var expr = parser.ParseExpression();
            diags.AddRange(parser.Diagnostics);

            diagnostics = diags;

            if (expr is null || diags.Count > 0)
            {
                expression = expr;
                return false;
            }

            ExpressionRegistry.RaiseParsed(expr);
            expression = expr;
            diagnostics = diags;
            return true;
        }

        /// <summary>
        /// Parses the specified input and returns an <see cref="IExpression"/>.
        /// </summary>
        /// <param name="input">The input expression text to parse.</param>
        /// <returns>The parsed expression.</returns>
        /// <exception cref="ArgumentException">Thrown when parsing fails. The exception message contains concatenated diagnostic messages.</exception>
        public static IExpression Parse(string input)
        {
            if (!TryParse(input, out var expr, out var diags))
            {
                var message = string.Join("; ", diags.Select(d => d.Message));
                throw new ArgumentException($"Failed to parse expression: {message}");
            }

            return expr!;
        }
    }
}


===== src/Zentient.Expressions/ExpressionRegistry.cs =====
// <copyright file="ExpressionRegistry.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Central registry that exposes default expression parser and evaluator implementations
    /// and lifecycle events for parsed and evaluated expressions.
    /// </summary>
    /// <remarks>
    /// This static type is thread-safe for replacing the default parser/evaluator and for
    /// subscribing/unsubscribing to events. Consumers may replace the defaults or subscribe
    /// to <see cref="OnParsed"/> and <see cref="OnEvaluated"/> to receive notifications.
    /// </remarks>
    public static class ExpressionRegistry
    {
        private static readonly object _sync = new();
        private static IExpressionParser _defaultParser = new DefaultExpressionParser();

        /// <summary>
        /// Gets or sets the default <see cref="IExpressionParser"/> used by the library.
        /// </summary>
        /// <remarks>
        /// Setting the value is thread-safe. Attempting to assign <c>null</c> will throw
        /// <see cref="ArgumentNullException"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public static IExpressionParser DefaultParser
        {
            get => _defaultParser;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                lock (_sync) { _defaultParser = value; }
            }
        }

        private static IExpressionEvaluator _defaultEvaluator = new DefaultExpressionEvaluator();

        /// <summary>
        /// Gets or sets the default <see cref="IExpressionEvaluator"/> used by the library.
        /// </summary>
        /// <remarks>
        /// Setting the value is thread-safe. Attempting to assign <c>null</c> will throw
        /// <see cref="ArgumentNullException"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public static IExpressionEvaluator DefaultEvaluator
        {
            get => _defaultEvaluator;
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                lock (_sync) { _defaultEvaluator = value; }
            }
        }

        private static Action<IExpression>? _onParsed;

        /// <summary>
        /// Event raised after an <see cref="IExpression"/> has been successfully parsed.
        /// </summary>
        /// <remarks>
        /// Event handlers will be invoked with the parsed expression. Subscription and
        /// unsubscription are performed under a lock to ensure thread-safety.
        /// </remarks>
        public static event Action<IExpression> OnParsed
        {
            add { lock (_sync) { _onParsed += value; } }
            remove { lock (_sync) { _onParsed -= value; } }
        }

        private static Action<IExpression, object?>? _onEvaluated;

        /// <summary>
        /// Event raised after an <see cref="IExpression"/> has been evaluated.
        /// </summary>
        /// <remarks>
        /// Handlers receive the expression and the evaluation result (which may be <c>null</c>).
        /// Subscription and unsubscription are thread-safe.
        /// </remarks>
        public static event Action<IExpression, object?> OnEvaluated
        {
            add { lock (_sync) { _onEvaluated += value; } }
            remove { lock (_sync) { _onEvaluated -= value; } }
        }

        /// <summary>
        /// Invokes the <see cref="OnParsed"/> event for a parsed expression.
        /// </summary>
        /// <param name="expr">The parsed expression to publish to subscribers.</param>
        internal static void RaiseParsed(IExpression expr)
            => _onParsed?.Invoke(expr);

        /// <summary>
        /// Invokes the <see cref="OnEvaluated"/> event for an evaluated expression.
        /// </summary>
        /// <param name="expr">The expression that was evaluated.</param>
        /// <param name="result">The result of the evaluation; may be <c>null</c>.</param>
        internal static void RaiseEvaluated(IExpression expr, object? result)
            => _onEvaluated?.Invoke(expr, result);

        // Adapter implementations for the public parser/evaluator

        /// <summary>
        /// Internal adapter that delegates parsing calls to the concrete parser implementation.
        /// </summary>
        private class DefaultExpressionParser : IExpressionParser
        {
            /// <inheritdoc />
            public bool TryParse(string input, out IExpression? expression, out IReadOnlyList<ParseDiagnostic> diagnostics)
                => ExpressionParser.TryParse(input, out expression, out diagnostics);

            /// <inheritdoc />
            public IExpression Parse(string input)
                => ExpressionParser.Parse(input);
        }

        /// <summary>
        /// Internal adapter that delegates evaluation calls to the concrete evaluator implementation.
        /// </summary>
        private class DefaultExpressionEvaluator : IExpressionEvaluator
        {
            /// <inheritdoc />
            public object? Evaluate(IExpression expression, object? context = null)
            {
                var result = StubEvaluator.Evaluate(expression, context);
                ExpressionRegistry.RaiseEvaluated(expression, result);
                return result;
            }
        }

        /// <summary>
        /// Creates a typed view over an existing expression using the registry's evaluator.
        /// </summary>
        /// <typeparam name="T">The expected result type of the expression.</typeparam>
        /// <param name="expr">The expression to wrap.</param>
        /// <returns>An <see cref="ITypedExpression{T}"/> that evaluates the underlying expression and casts the result to <typeparamref name="T"/>.</returns>
        public static ITypedExpression<T> AsTyped<T>(IExpression expr)
        {
            if (expr is null) throw new ArgumentNullException(nameof(expr));
            return new TypedExpression<T>(expr);
        }

        // Internal wrapper implementing the typed expression contract.
        private sealed class TypedExpression<T> : ITypedExpression<T>
        {
            private readonly IExpression _inner;

            public TypedExpression(IExpression inner) => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

            public ExpressionKind Kind => _inner.Kind;

            public string Canonical => _inner.Canonical;

            public IReadOnlyList<IExpression> Operands => _inner.Operands;

            public T Evaluate(object? context = null)
            {
                var result = ExpressionRegistry.DefaultEvaluator.Evaluate(_inner, context);
                if (result is null && default(T) is null)
                    return (T)result!; // allow null for reference types
                if (result is T t) return t;

                // Try convert common primitive/numeric types
                try
                {
                    var converted = Convert.ChangeType(result, typeof(T));
                    return (T)converted!;
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Cannot cast evaluator result of type '{result?.GetType()}' to '{typeof(T)}'.", ex);
                }
            }
        }
    }
}


===== src/Zentient.Expressions/Extensions.cs =====
// <copyright file="Extensions.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zentient.Abstractions.Expressions;

namespace Zentient.Extensions.Expressions
{
    /// <summary>
    /// Ergonomic extension methods for common expression workflows.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Parses and evaluates the provided expression text using the default registry parser and evaluator.
        /// Returns the evaluation result or throws when parsing fails.
        /// </summary>
        public static object? EvaluateExpression(this string expressionText, object? context = null)
        {
            if (expressionText is null) throw new System.ArgumentNullException(nameof(expressionText));

            var parser = Zentient.Expressions.ExpressionRegistry.DefaultParser;
            if (!parser.TryParse(expressionText, out var expr, out var diags))
            {
                var msg = string.Join(';', System.Linq.Enumerable.Select(diags, d => d.Message));
                throw new System.ArgumentException($"Failed to parse expression: {msg}");
            }

            return Zentient.Expressions.ExpressionRegistry.DefaultEvaluator.Evaluate(expr!, context);
        }

        /// <summary>
        /// Returns the canonical form of an expression.
        /// </summary>
        public static string ToCanonicalString(this IExpression expr)
            => expr?.Canonical ?? string.Empty;

        /// <summary>
        /// Returns a brief debug string describing the expression kind, operand count, and canonical form.
        /// </summary>
        public static string ToDebugString(this IExpression expr)
            => $"Kind={expr.Kind}; Operands={expr.Operands?.Count ?? 0}; Canonical={expr.Canonical}";
    }
}


===== src/Zentient.Expressions/IdentifierExpression.cs =====
// <copyright file="IdentifierExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Represents an identifier (variable or parameter name).
    /// </summary>
    internal sealed class IdentifierExpression : ExpressionBase
    {
        /// <summary>
        /// Gets the identifier name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentifierExpression"/>.
        /// </summary>
        /// <param name="name">The identifier name.</param>
        public IdentifierExpression(string name) => Name = name;

        /// <inheritdoc />
        public override ExpressionKind Kind => ExpressionKind.Identifier;

        /// <inheritdoc />
        public override IReadOnlyList<IExpression> Operands => Array.Empty<IExpression>();

        /// <inheritdoc />
        public override string Canonical => Name;
    }
}


===== src/Zentient.Expressions/LambdaExpression.cs =====
// <copyright file="LambdaExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Represents a lambda (anonymous function) expression with parameters and a body.
    /// </summary>
    internal sealed class LambdaExpression : ExpressionBase
    {
        /// <summary>
        /// Gets the lambda parameter names.
        /// </summary>
        public IReadOnlyList<string> Parameters { get; }

        /// <summary>
        /// Gets the lambda body expression.
        /// </summary>
        public IExpression Body { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="LambdaExpression"/>.
        /// </summary>
        /// <param name="parameters">The parameter names used by the lambda.</param>
        /// <param name="body">The body expression.</param>
        public LambdaExpression(IEnumerable<string> parameters, IExpression body)
            => (Parameters, Body) = (parameters.ToArray(), body);

        /// <inheritdoc />
        public override ExpressionKind Kind => ExpressionKind.Lambda;

        /// <inheritdoc />
        public override IReadOnlyList<IExpression> Operands => new[] { Body };

        /// <inheritdoc />
        public override string Canonical
            => $"{string.Join(", ", Parameters)} => {Body.Canonical}";
    }
}


===== src/Zentient.Expressions/Lexer.cs =====
// <copyright file="Lexer.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Expressions
{
    /// <summary>
    /// Simple lexer that converts an input source string into a stream of <see cref="Token"/> instances.
    /// </summary>
    internal sealed class Lexer
    {
        private readonly string src;
        private int pos;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lexer"/> for the specified source.
        /// </summary>
        /// <param name="src">The input text to tokenize.</param>
        public Lexer(string src) => this.src = src;

        /// <summary>
        /// Reads and returns the next token from the input. Whitespace is skipped.
        /// Returns a <see cref="TokenType.End"/> token when the end of input is reached.
        /// </summary>
        /// <returns>The next <see cref="Token"/> read from the input stream.</returns>
        public Token Next()
        {
            SkipWhitespace();
            if (pos >= src.Length) return new Token(TokenType.End, "", pos);

            int start = pos;
            char c = src[pos];

            if (char.IsLetter(c) || c == '_')
                return ReadIdentifier(start);

            if (char.IsDigit(c))
                return ReadNumber(start);

            if (c == '"')
                return ReadString(start);

            // handle multi-character token '=>' first
            if (c == '=' && pos + 1 < src.Length && src[pos + 1] == '>')
            {
                pos += 2;
                return new Token(TokenType.Arrow, "=>", start);
            }

            // single-character punctuation
            pos++;
            return c switch
            {
                '.' => new Token(TokenType.Dot, ".", start),
                ',' => new Token(TokenType.Comma, ",", start),
                '(' => new Token(TokenType.LParen, "(", start),
                ')' => new Token(TokenType.RParen, ")", start),
                _ => new Token(TokenType.End, "", start)
            };
        }

        /// <summary>
        /// Reads an identifier token beginning at <paramref name="start"/>.
        /// Identifiers may contain letters, digits and underscore.
        /// </summary>
        /// <param name="start">Start index in the source where the identifier begins.</param>
        /// <returns>A token of type <see cref="TokenType.Identifier"/>.</returns>
        private Token ReadIdentifier(int start)
        {
            while (pos < src.Length && (char.IsLetterOrDigit(src[pos]) || src[pos] == '_'))
                pos++;
            string text = src[start..pos];
            return new Token(TokenType.Identifier, text, start);
        }

        /// <summary>
        /// Reads a numeric token (digits and optional decimal point).
        /// The lexer performs only lexical recognition; numeric validation occurs during parsing.
        /// </summary>
        /// <param name="start">Start index in the source where the number begins.</param>
        /// <returns>A token of type <see cref="TokenType.Number"/>.</returns>
        private Token ReadNumber(int start)
        {
            while (pos < src.Length && (char.IsDigit(src[pos]) || src[pos] == '.'))
                pos++;
            string text = src[start..pos];
            return new Token(TokenType.Number, text, start);
        }

        /// <summary>
        /// Reads a double-quoted string literal and returns its unquoted content as the token text.
        /// Supports common escape sequences: \\, \" , \n, \r, \t. Reports unterminated strings by returning a token with IsComplete=false.
        /// </summary>
        /// <param name="start">Start index pointing at the opening quote.</param>
        /// <returns>A token of type <see cref="TokenType.String"/> containing the unquoted string content.</returns>
        private Token ReadString(int start)
        {
            pos++; // skip opening "
            var sb = new System.Text.StringBuilder();
            bool terminated = false;
            while (pos < src.Length)
            {
                char ch = src[pos];
                if (ch == '"')
                {
                    terminated = true;
                    pos++; // consume closing quote
                    break;
                }

                if (ch == '\\' && pos + 1 < src.Length)
                {
                    // handle escape sequences
                    char next = src[pos + 1];
                    switch (next)
                    {
                        case '\\': sb.Append('\\'); break;
                        case '"': sb.Append('"'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        default:
                            // unknown escape: preserve the character as-is
                            sb.Append(next);
                            break;
                    }
                    pos += 2;
                    continue;
                }

                sb.Append(ch);
                pos++;
            }

            // If not terminated, capture the remainder as text and mark incomplete
            if (!terminated)
            {
                string textRem = src.Substring(start + 1);
                pos = src.Length;
                return new Token(TokenType.String, textRem, start, isComplete: false);
            }

            return new Token(TokenType.String, sb.ToString(), start, isComplete: true);
        }

        /// <summary>
        /// Peeks one character ahead of the current position, returning '\0' when at or past the end.
        /// Used for recognizing multi-character tokens like "=>".
        /// </summary>
        /// <returns>The next character or '\0' if none.</returns>
        private char Peek() => pos + 1 < src.Length ? src[pos + 1] : '\0';

        /// <summary>
        /// Advances the internal cursor past any contiguous whitespace characters.
        /// </summary>
        private void SkipWhitespace()
        {
            while (pos < src.Length && char.IsWhiteSpace(src[pos]))
                pos++;
        }
    }
}


===== src/Zentient.Expressions/MemberAccessExpression.cs =====
// <copyright file="MemberAccessExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Represents accessing a member on a target expression (for example: target.Member).
    /// </summary>
    internal sealed class MemberAccessExpression : ExpressionBase
    {
        /// <summary>
        /// Gets the target expression whose member is accessed.
        /// </summary>
        public IExpression Target { get; }

        /// <summary>
        /// Gets the member name being accessed.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="MemberAccessExpression"/>.
        /// </summary>
        /// <param name="target">The target expression.</param>
        /// <param name="memberName">The member name.</param>
        public MemberAccessExpression(IExpression target, string memberName)
            => (Target, MemberName) = (target, memberName);

        /// <inheritdoc />
        public override ExpressionKind Kind => ExpressionKind.MemberAccess;

        /// <inheritdoc />
        public override IReadOnlyList<IExpression> Operands => new[] { Target };

        /// <inheritdoc />
        public override string Canonical => $"{Target.Canonical}.{MemberName}";
    }
}


===== src/Zentient.Expressions/MethodCallExpression.cs =====
// <copyright file="MethodCallExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Represents a method call expression on a target with arguments.
    /// </summary>
    internal sealed class MethodCallExpression : ExpressionBase
    {
        /// <summary>
        /// Gets the target expression on which the method is invoked.
        /// </summary>
        public IExpression Target { get; }

        /// <summary>
        /// Gets the method name to invoke.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Gets the call arguments.
        /// </summary>
        public IReadOnlyList<IExpression> Arguments { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="MethodCallExpression"/>.
        /// </summary>
        /// <param name="target">The target expression.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="args">The arguments to the method.</param>
        public MethodCallExpression(
            IExpression target,
            string methodName,
            IEnumerable<IExpression> args)
        {
            Target = target;
            MethodName = methodName;
            Arguments = args.ToArray();
        }

        /// <inheritdoc />
        public override ExpressionKind Kind => ExpressionKind.MethodCall;

        /// <inheritdoc />
        public override IReadOnlyList<IExpression> Operands
            => new[] { Target }.Concat(Arguments).ToArray();

        /// <inheritdoc />
        public override string Canonical
            => $"{Target.Canonical}.{MethodName}({string.Join(", ", Arguments.Select(a => a.Canonical))})";
    }
}


===== src/Zentient.Expressions/Parser.cs =====
// <copyright file="Parser.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System.Globalization;

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Recursive-descent parser that converts a token stream into an <see cref="IExpression"/> AST and collects diagnostics.
    /// </summary>
    internal sealed class Parser
    {
        private readonly List<Token> tokens = new();
        private int idx;
        private readonly List<ParseDiagnostic> diagnostics = new();

        /// <summary>
        /// Gets the diagnostics produced while parsing. The collection may be empty when parsing succeeds.
        /// </summary>
        public IReadOnlyList<ParseDiagnostic> Diagnostics => diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> that tokenizes the provided source string.
        /// </summary>
        /// <param name="src">The expression source text to parse.</param>
        public Parser(string src)
        {
            var lexer = new Lexer(src);
            Token tok;
            do
            {
                tok = lexer.Next();
                tokens.Add(tok);
            } while (tok.Type != TokenType.End);
        }

        /// <summary>
        /// Parses an expression from the current token stream. When parsing fails, diagnostics are recorded.
        /// </summary>
        /// <returns>The parsed <see cref="IExpression"/> when successful; otherwise <c>null</c>.</returns>
        public IExpression? ParseExpression()
        {
            if (Peek().Type == TokenType.End)
            {
                diagnostics.Add(new ParseDiagnostic(0, "Empty expression"));
                return null;
            }

            var expr = ParseLambdaOrMemberOrCall();

            if (Peek().Type != TokenType.End)
            {
                var tok = Peek();
                diagnostics.Add(new ParseDiagnostic(tok.Position, $"Unexpected token '{tok.Text}'"));
            }

            return expr;
        }

        /// <summary>
        /// Parses either a lambda expression, a member access chain, or a method call starting at the current token.
        /// Handles primary expressions (identifiers, numbers, strings) and folds trailing member/access or invocation.
        /// </summary>
        /// <returns>The parsed expression or <c>null</c> on error.</returns>
        private IExpression? ParseLambdaOrMemberOrCall()
        {
            if (IsIdentifierList() && PeekNext().Type == TokenType.Arrow)
                return ParseLambda();

            IExpression? expr = Peek().Type switch
            {
                TokenType.Identifier => new IdentifierExpression(Consume().Text),
                TokenType.Number => TryParseNumber(),
                TokenType.String => TryParseString(),
                _ => null
            };

            if (expr is null)
            {
                var tok = Peek();
                diagnostics.Add(new ParseDiagnostic(tok.Position, $"Unexpected token '{tok.Text}'"));
                return null;
            }

            while (Peek().Type == TokenType.Dot)
            {
                Consume(); // '.'
                if (Peek().Type != TokenType.Identifier)
                {
                    var err = Peek();
                    diagnostics.Add(new ParseDiagnostic(err.Position, "Identifier expected after '.'"));
                    break;
                }

                var name = Consume().Text;
                expr = Peek().Type == TokenType.LParen
                    ? ParseMethodCall(expr, name)
                    : new MemberAccessExpression(expr, name);
            }

            return expr;
        }

        /// <summary>
        /// Determines whether the token sequence beginning at the current index represents
        /// a comma-separated identifier list suitable for lambda parameters (e.g. "x, y =>").
        /// </summary>
        /// <returns><c>true</c> when a comma-separated identifier list is followed by an arrow token.</returns>
        private bool IsIdentifierList()
        {
            int i = idx;
            if (tokens[i].Type != TokenType.Identifier) return false;
            i++;
            while (i < tokens.Count && tokens[i].Type == TokenType.Comma)
                i += 2;
            return i < tokens.Count && tokens[i].Type == TokenType.Arrow;
        }

        /// <summary>
        /// Parses a lambda expression in the form "param, ... => body".
        /// Parameters are consumed from the token stream and the body is parsed.
        /// </summary>
        /// <returns>A <see cref="LambdaExpression"/> representing the parsed lambda.</returns>
        private IExpression? ParseLambda()
        {
            var parameters = new List<string>();
            do
            {
                parameters.Add(Consume().Text);
            } while (Peek().Type == TokenType.Comma && Consume().Type == TokenType.Comma);

            Consume(); // Arrow
            var body = ParseLambdaOrMemberOrCall() ?? new ConstantExpression(null);
            return new LambdaExpression(parameters, body);
        }

        /// <summary>
        /// Parses a method call expression given a previously parsed target and method name.
        /// Expects to be positioned at the opening '(' when called.
        /// </summary>
        /// <param name="target">The target expression on which the method is invoked.</param>
        /// <param name="name">The method name.</param>
        /// <returns>A <see cref="MethodCallExpression"/> representing the invocation.</returns>
        private MethodCallExpression ParseMethodCall(IExpression target, string name)
        {
            Consume(); // LParen
            var args = new List<IExpression>();
            if (Peek().Type != TokenType.RParen)
            {
                do
                {
                    var arg = ParseLambdaOrMemberOrCall();
                    if (arg != null) args.Add(arg);
                } while (Peek().Type == TokenType.Comma && Consume().Type == TokenType.Comma);
            }
            Consume(); // RParen
            return new MethodCallExpression(target, name, args);
        }

        /// <summary>
        /// Attempts to parse the current numeric token into a numeric constant expression using invariant culture.
        /// When parsing fails a diagnostic is recorded and a <see cref="ConstantExpression"/> with a null value is returned.
        /// </summary>
        /// <returns>An <see cref="IExpression"/> representing the numeric constant (or null constant on error).</returns>
        private IExpression TryParseNumber()
        {
            var tok = Consume();
            if (double.TryParse(tok.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                return new ConstantExpression(d);

            diagnostics.Add(new ParseDiagnostic(tok.Position, $"Invalid number '{tok.Text}'"));
            return new ConstantExpression(null);
        }

        /// <summary>
        /// Consumes the current string token and returns a <see cref="ConstantExpression"/> containing its text.
        /// </summary>
        /// <returns>An <see cref="IExpression"/> representing the string constant.</returns>
        private IExpression TryParseString()
        {
            var tok = Consume();
            if (!tok.IsComplete)
            {
                diagnostics.Add(new ParseDiagnostic(tok.Position, "Unterminated string literal"));
                return new ConstantExpression(null);
            }

            return new ConstantExpression(tok.Text);
        }

        /// <summary>
        /// Returns the token at the current parser index without consuming it.
        /// </summary>
        /// <returns>The current <see cref="Token"/>.</returns>
        private Token Peek() => tokens[idx];

        /// <summary>
        /// Returns the token immediately after the current parser index without consuming it.
        /// If the lookahead is out of range the current token is returned.
        /// </summary>
        /// <returns>The next <see cref="Token"/> or the current token if lookahead is not available.</returns>
        private Token PeekNext() => idx + 1 < tokens.Count ? tokens[idx + 1] : Peek();

        /// <summary>
        /// Consumes and returns the token at the current index, advancing the parser position by one.
        /// </summary>
        /// <returns>The consumed <see cref="Token"/>.</returns>
        private Token Consume() => tokens[idx++];
    }
}


===== src/Zentient.Expressions/StubEvaluator.cs =====
// <copyright file="StubEvaluator.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// Minimal evaluator used internally by tests/stubs. Supports constant expressions and
    /// simple identifier/member lookup from IDictionary<string, object?> contexts.
    /// </summary>
    internal static class StubEvaluator
    {
        /// <summary>
        /// Evaluates the specified expression and returns a result.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <param name="context">An optional evaluation context. If a IDictionary&lt;string, object?&gt; is provided identifiers and member access will be resolved from it.</param>
        /// <returns>The evaluated value for supported node types; otherwise <c>null</c>.</returns>
        public static object? Evaluate(IExpression expr, object? context)
        {
            return expr switch
            {
                ConstantExpression c => c.Value,
                IdentifierExpression id => ResolveIdentifier(id.Name, context),
                MemberAccessExpression m => ResolveMember(m.Target, m.MemberName, context),
                MethodCallExpression mc => EvaluateMethodCall(mc, context),
                _ => null
            };
        }

        private static object? ResolveIdentifier(string name, object? context)
        {
            if (context is System.Collections.IDictionary dict)
            {
                if (dict.Contains(name)) return dict[name];
            }
            else if (context is System.Collections.Generic.IDictionary<string, object?> gen)
            {
                if (gen.TryGetValue(name, out var v)) return v;
            }
            return null;
        }

        private static object? ResolveMember(IExpression targetExpr, string memberName, object? context)
        {
            var targetVal = Evaluate(targetExpr, context);
            if (targetVal is System.Collections.IDictionary dict)
            {
                if (dict.Contains(memberName)) return dict[memberName];
            }
            else if (targetVal is System.Collections.Generic.IDictionary<string, object?> gen)
            {
                if (gen.TryGetValue(memberName, out var v)) return v;
            }

            // As a fallback, reflect public properties
            if (targetVal is not null)
            {
                var t = targetVal.GetType();
                var prop = t.GetProperty(memberName);
                if (prop != null) return prop.GetValue(targetVal);
            }

            return null;
        }

        private static object? EvaluateMethodCall(MethodCallExpression mc, object? context)
        {
            var target = Evaluate(mc.Target, context);
            var rawArgs = mc.Arguments.Select(a => Evaluate(a, context)).ToArray();

            // If target is a delegate, invoke with coerced args
            if (target is Delegate d)
            {
                var adapted = AdaptArgumentsForDelegate(d, rawArgs);
                return adapted is null ? null : InvokeDelegate(d, adapted);
            }

            // If target is a dictionary containing the method name as a delegate, invoke it
            if (target is System.Collections.Generic.IDictionary<string, object?> targetGen && targetGen.TryGetValue(mc.MethodName, out var maybeDel) && maybeDel is Delegate del2)
            {
                var adapted = AdaptArgumentsForDelegate(del2, rawArgs);
                return adapted is null ? null : InvokeDelegate(del2, adapted);
            }

            if (target is System.Collections.IDictionary targetDict && targetDict.Contains(mc.MethodName) && targetDict[mc.MethodName] is Delegate del3)
            {
                var adapted = AdaptArgumentsForDelegate(del3, rawArgs);
                return adapted is null ? null : InvokeDelegate(del3, adapted);
            }

            // Try lookup in context by method name when target didn't provide it
            if (context is System.Collections.Generic.IDictionary<string, object?> dict && dict.TryGetValue(mc.MethodName, out var maybeDel2) && maybeDel2 is Delegate dd)
            {
                var adapted = AdaptArgumentsForDelegate(dd, rawArgs);
                return adapted is null ? null : InvokeDelegate(dd, adapted);
            }

            if (context is System.Collections.IDictionary ctxDict && ctxDict.Contains(mc.MethodName) && ctxDict[mc.MethodName] is Delegate dd2)
            {
                var adapted = AdaptArgumentsForDelegate(dd2, rawArgs);
                return adapted is null ? null : InvokeDelegate(dd2, adapted);
            }

            // As a final fallback, try reflection on the target for a delegate-valued property
            if (target is not null)
            {
                var t = target.GetType();
                var prop = t.GetProperty(mc.MethodName);
                if (prop != null)
                {
                    var val = prop.GetValue(target);
                    if (val is Delegate ddd)
                    {
                        var adapted = AdaptArgumentsForDelegate(ddd, rawArgs);
                        return adapted is null ? null : InvokeDelegate(ddd, adapted);
                    }
                }
            }

            // Deep search: find delegate by key in nested dictionaries or properties
            var found = FindDelegateInObject(target, mc.MethodName) ?? FindDelegateInObject(context, mc.MethodName);
            if (found is Delegate fdel)
            {
                var adapted = AdaptArgumentsForDelegate(fdel, rawArgs);
                return adapted is null ? null : InvokeDelegate(fdel, adapted);
            }

            return null;
        }

        private static object? InvokeDelegate(Delegate del, object[] args)
        {
            // Allow exceptions to surface so calling tests can detect invocation issues
            return del.DynamicInvoke(args);
        }

        private static object[]? AdaptArgumentsForDelegate(Delegate del, object?[] rawArgs)
        {
            var parameters = del.Method.GetParameters();
            if (parameters.Length != rawArgs.Length) return null;
            var adapted = new object?[rawArgs.Length];
            for (int i = 0; i < rawArgs.Length; i++)
            {
                var targetType = parameters[i].ParameterType;
                var val = rawArgs[i];
                if (val == null)
                {
                    adapted[i] = null;
                    continue;
                }

                if (targetType.IsInstanceOfType(val))
                {
                    adapted[i] = val;
                    continue;
                }

                try
                {
                    adapted[i] = Convert.ChangeType(val, targetType);
                }
                catch
                {
                    // Failed to convert
                    return null;
                }
            }

            return adapted.Cast<object>().ToArray();
        }

        private static object? FindDelegateInObject(object? obj, string methodName)
        {
            if (obj is null) return null;
            if (obj is Delegate d) return d;

            if (obj is System.Collections.Generic.IDictionary<string, object?> gen)
            {
                if (gen.TryGetValue(methodName, out var v) && v is Delegate dv) return dv;
                foreach (var val in gen.Values)
                {
                    var found = FindDelegateInObject(val, methodName);
                    if (found is Delegate) return found;
                }
            }

            if (obj is System.Collections.IDictionary nonGen)
            {
                if (nonGen.Contains(methodName) && nonGen[methodName] is Delegate dv2) return dv2;
                foreach (System.Collections.DictionaryEntry entry in nonGen)
                {
                    var found = FindDelegateInObject(entry.Value, methodName);
                    if (found is Delegate) return found;
                }
            }

            // reflect properties
            var t = obj.GetType();
            foreach (var prop in t.GetProperties())
            {
                var val = prop.GetValue(obj);
                var found = FindDelegateInObject(val, methodName);
                if (found is Delegate) return found;
            }

            return null;
        }
    }
}


===== src/Zentient.Expressions/StubTypedExpression.cs =====
// <copyright file="StubTypedExpression.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using Zentient.Abstractions.Expressions;

namespace Zentient.Expressions
{
    /// <summary>
    /// A lightweight wrapper implementing <see cref="ITypedExpression{T}"/> around an existing <see cref="IExpression"/>.
    /// The evaluation uses the internal <see cref="StubEvaluator"/> and not a full evaluator.
    /// </summary>
    /// <typeparam name="T">The target CLR type for evaluation.</typeparam>
    internal sealed class StubTypedExpression<T> : ITypedExpression<T>
    {
        private readonly IExpression _inner;

        /// <summary>
        /// Initializes a new instance that wraps <paramref name="inner"/>.
        /// </summary>
        /// <param name="inner">The inner expression to evaluate.</param>
        public StubTypedExpression(IExpression inner) => _inner = inner;

        /// <inheritdoc />
        public ExpressionKind Kind => _inner.Kind;

        /// <inheritdoc />
        public string Canonical => _inner.Canonical;

        /// <inheritdoc />
        public IReadOnlyList<IExpression> Operands => _inner.Operands;

        /// <summary>
        /// Evaluates the wrapped expression using the internal stub evaluator and returns a value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="context">An optional context object passed to the evaluator.</param>
        /// <returns>The evaluated value converted to <typeparamref name="T"/> when possible; otherwise the default of <typeparamref name="T"/>.</returns>
        public T Evaluate(object? context = null)
        {
            var result = StubEvaluator.Evaluate(_inner, context);
            ExpressionRegistry.RaiseEvaluated(_inner, result);
            return result is T t ? t : default!;
        }
    }
}


===== src/Zentient.Expressions/Token.cs =====
// <copyright file="Token.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Expressions
{
    /// <summary>
    /// A single lexical token with its type, textual content and start position.
    /// </summary>
    internal readonly struct Token
    {
        /// <summary>Gets the token kind.</summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets the exact text for the token as it appeared in the source.
        /// For string tokens this value is the unquoted content.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the zero-based character index in the source where the token begins.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// For string tokens, indicates whether the closing quote was found.
        /// For other tokens this is always true.
        /// </summary>
        public bool IsComplete { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Token"/>.
        /// </summary>
        /// <param name="type">The token kind.</param>
        /// <param name="text">The token text.</param>
        /// <param name="position">Zero-based start position within the source.</param>
        /// <param name="isComplete">For string tokens, whether the closing quote was found. Defaults to true.</param>
        public Token(TokenType type, string text, int position, bool isComplete = true)
            => (Type, Text, Position, IsComplete) = (type, text, position, isComplete);
    }
}


===== src/Zentient.Expressions/TokenType.cs =====
// <copyright file="TokenType.cs" authors="Zentient Framework Team">
// Copyright Â© 2025 Zentient Framework Team. All rights reserved.
// </copyright>

namespace Zentient.Expressions
{
    /// <summary>
    /// Represents the kinds of lexical tokens produced by the expression lexer.
    /// </summary>
    internal enum TokenType
    {
        Identifier,
        Number,
        String,
        Dot,
        Comma,
        LParen,
        RParen,
        Arrow,
        End
    }
}
