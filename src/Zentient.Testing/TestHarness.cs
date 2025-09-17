using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Lightweight per-test resolver implementing <see cref="ITestHarness"/>.
    /// </summary>
    internal sealed class TestHarness : ITestHarness, IDisposable
    {
        private readonly Dictionary<Type, object> _registrations;

        /// <summary>
        /// Initializes a new instance of <see cref="TestHarness"/> with the provided registrations.
        /// </summary>
        /// <param name="registrations">Prepopulated registrations.</param>
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

            // Try to construct via constructors using registered dependencies
            var ctors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Length);

            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                var args = new object?[parameters.Length];
                var ok = true;
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
                        break;
                    }
                }

                if (!ok)
                    continue;

                return (T)ctor.Invoke(args);
            }

            throw new InvalidOperationException($"Could not resolve type {t.FullName}. Register it with the harness or provide constructor dependencies.");
        }

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
