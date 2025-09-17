using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Default mock builder implementation.
    /// </summary>
    /// <typeparam name="T">The mocked interface type.</typeparam>

    internal sealed class MockBuilder<T> : IMockBuilder<T>
    {
        private readonly MockEngine _engine = new();
        private MethodInfo? _currentMethod;
        private Func<object?[], bool>? _currentPredicate;

        private static MethodCallExpression? GetMethodCall(Expression expr)
        {
            if (expr is MethodCallExpression mce) return mce;
            if (expr is UnaryExpression ue && ue.Operand is MethodCallExpression inner) return inner;
            return null;
        }

        /// <inheritdoc />
        public IMockBuilder<T> Given(Expression<Func<T, object>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);
            var mce = GetMethodCall(expression.Body) ?? throw new InvalidOperationException("Given must be a method call expression");

            _currentMethod = mce.Method;

            // Minimal support: treat any It.IsAny<T>() as wildcard.
            ReadOnlyCollection<Expression> args = mce.Arguments;
            _currentPredicate = callArgs =>
            {
                if (callArgs.Length != args.Count)
                {
                    return false;
                }

                for (int i = 0; i < args.Count; i++)
                {
                    if (args[i] is MethodCallExpression argMce && argMce.Method.DeclaringType == typeof(It))
                    {
                        continue;
                    }

                    object? expected = Expression.Lambda(args[i]).Compile().DynamicInvoke();
                    if (!Equals(expected, callArgs[i]))
                    {
                        return false;
                    }
                }

                return true;
            };

            return this;
        }

        public IMockBuilder<T> Given(Expression<Action<T>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);
            var mce = GetMethodCall(expression.Body) ?? throw new InvalidOperationException("Given must be a method call expression");

            _currentMethod = mce.Method;
            ReadOnlyCollection<Expression> args = mce.Arguments;
            _currentPredicate = callArgs =>
            {
                if (callArgs.Length != args.Count)
                {
                    return false;
                }

                for (int i = 0; i < args.Count; i++)
                {
                    if (args[i] is MethodCallExpression argMce && argMce.Method.DeclaringType == typeof(It))
                    {
                        continue;
                    }

                    object? expected = Expression.Lambda(args[i]).Compile().DynamicInvoke();
                    if (!Equals(expected, callArgs[i]))
                    {
                        return false;
                    }
                }

                return true;
            };

            return this;
        }

        /// <inheritdoc />
        public IMockBuilder<T> ThenReturns(object? result)
        {
            if (_currentMethod is null)
            {
                throw new InvalidOperationException("ThenReturns called before Given");
            }

            _engine.AddBehavior(new Behavior(
                _currentMethod,
                _currentPredicate ?? (_ => true),
                null,
                result,
                null));

            _currentMethod = null;
            _currentPredicate = null;

            return this;
        }

        /// <inheritdoc />
        public IMockBuilder<T> ThenThrows(Exception ex)
        {
            if (_currentMethod is null)
            {
                throw new InvalidOperationException("ThenThrows called before Given");
            }

            _engine.AddBehavior(new Behavior(
                _currentMethod,
                _currentPredicate ?? (_ => true),
                null,
                null,
                ex));

            _currentMethod = null;
            _currentPredicate = null;

            return this;
        }

        /// <inheritdoc />
        public T Build(out IMockVerifier<T> verifier)
        {
            T proxy = ProxyGenerator.CreateProxyInstance<T>(_engine);
            verifier = new MockVerifier<T>(_engine);
            return proxy;
        }
    }
}
