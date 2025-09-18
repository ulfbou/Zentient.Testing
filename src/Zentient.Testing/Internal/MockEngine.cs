// <copyright file="MockEngine.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Engine responsible for storing configured behaviors and replaying them for proxy calls.
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
        /// Invokes the engine for a method call, records the call, and returns a configured result.
        /// </summary>
        /// <param name="method">The invoked <see cref="MethodInfo"/>.</param>
        /// <param name="args">Call arguments.</param>
        /// <returns>The configured return value when a behavior matches; otherwise a default value for the method's return type.</returns>
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
        /// Gets the recorded calls for verification purposes.
        /// </summary>
        public IReadOnlyList<CallInfo> Calls => _calls;
    }
}
