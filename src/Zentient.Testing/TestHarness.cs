// <copyright file="TestHarness.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Lightweight per-test resolver that provides simple registration-based resolution
    /// and fallback constructor-based instantiation for types.
    /// </summary>
    internal sealed class TestHarness : ITestHarness, IDisposable
    {
        private readonly Dictionary<Type, object> _registrations;

        /// <summary>
        /// Initializes a new instance of <see cref="TestHarness"/> using the supplied registrations.
        /// </summary>
        /// <param name="registrations">A dictionary of pre-populated service registrations keyed by type.</param>
        public TestHarness(Dictionary<Type, object> registrations)
        {
            _registrations = registrations ?? new Dictionary<Type, object>();
        }

        /// <inheritdoc />
        public T Resolve<T>()
        {
            var t = typeof(T);
            if (_registrations.TryGetValue(t, out var instance))
                return (T)instance;

            var ctors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToArray();

            var attempted = new List<string>();
            var missingAcross = new HashSet<string>();

            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                var args = new object?[parameters.Length];
                var ok = true;
                var missingForThis = new List<string>();
                for (int i = 0; i < parameters.Length; i++)
                {
                    var pType = parameters[i].ParameterType;
                    if (_registrations.TryGetValue(pType, out var dep))
                    {
                        args[i] = dep;
                    }
                    else
                    {
                        ok = false;
                        missingForThis.Add(pType.FullName ?? pType.Name);
                        missingAcross.Add(pType.FullName ?? pType.Name);
                    }
                }

                var sig = parameters.Length == 0
                    ? "()"
                    : "(" + string.Join(", ", parameters.Select(p => p.ParameterType.Name + " " + p.Name)) + ")";

                if (ok)
                {
                    return (T)ctor.Invoke(args);
                }

                attempted.Add($"ctor {sig} - missing: {string.Join(", ", missingForThis)}");
            }

            var message = $"Could not resolve type '{t.FullName}'.\n" +
                          "Attempted public constructors:\n" +
                          string.Join("\n", attempted) + "\n" +
                          (missingAcross.Count > 0
                              ? "Register the following dependency types with the harness using WithDependency<T>(instance): " + string.Join(", ", missingAcross)
                              : "No suitable public constructor found. Consider adding a public constructor or register an instance with the harness.");

            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Dispose the harness, disposing any registered <see cref="IDisposable"/> instances and clearing registrations.
        /// </summary>
        public void Dispose()
        {
            foreach (var kv in _registrations)
            {
                if (kv.Value is IDisposable d)
                    d.Dispose();
            }

            _registrations.Clear();
        }
    }
}
