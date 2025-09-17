using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Engine that records calls and matches configured behaviors for mocked methods.
    /// </summary>
    public sealed class MockEngine
    {
        private readonly Dictionary<MethodInfo, List<Behavior>> _behaviors = new();
        private readonly List<CallInfo> _calls = new();

        /// <summary>
        /// Adds a behavior to the engine for a given method.
        /// </summary>
        /// <param name="behavior">The behavior to add.</param>
        public void AddBehavior(Behavior behavior)
        {
            if (!_behaviors.TryGetValue(behavior.Method, out List<Behavior>? list))
            {
                list = new List<Behavior>();
                _behaviors[behavior.Method] = list;
            }

            list.Add(behavior);
        }

        /// <summary>
        /// Invokes the mock engine for a method call, returning the configured result or default value.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="args">The call arguments.</param>
        /// <returns>The configured return value or a default instance for value types.</returns>
        public object? Invoke(MethodInfo method, object?[] args)
        {
            _calls.Add(new CallInfo(method, args));

            if (_behaviors.TryGetValue(method, out List<Behavior>? list))
            {
                foreach (Behavior behavior in list)
                {
                    if (behavior.Predicate(args))
                    {
                        if (behavior.ExceptionToThrow is not null)
                        {
                            throw behavior.ExceptionToThrow;
                        }

                        if (behavior.Action is not null)
                        {
                            return behavior.Action(args);
                        }

                        return behavior.ReturnValue;
                    }
                }
            }

            return method.ReturnType.IsValueType ? Activator.CreateInstance(method.ReturnType) : null;
        }

        /// <summary>
        /// Gets the recorded calls.
        /// </summary>
        public IReadOnlyList<CallInfo> Calls => _calls;
    }
}
