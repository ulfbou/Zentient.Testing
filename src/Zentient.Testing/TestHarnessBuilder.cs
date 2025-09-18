// <copyright file="TestHarnessBuilder.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Builder used to configure and produce a <see cref="TestHarness"/> instance for a single test.
    /// </summary>
    internal sealed class TestHarnessBuilder : ITestHarnessBuilder
    {
        private readonly Dictionary<Type, object> _registrations = new Dictionary<Type, object>();

        /// <inheritdoc />
        public ITestHarnessBuilder WithDependency<T>(T instance)
        {
            _registrations[typeof(T)] = instance!;
            return this;
        }

        /// <inheritdoc />
        public ITestHarnessBuilder WithMock<T>(Action<Zentient.Abstractions.Testing.IMockBuilder<T>> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new MockBuilder<T>();
            configure(builder);
            IMockVerifier<T> verifier;
            T mock = builder.Build(out verifier);
            _registrations[typeof(T)] = mock!;
            return this;
        }

        /// <inheritdoc />
        public ITestHarnessBuilder WithMock<T>(Action<Zentient.Abstractions.Testing.IMockBuilder<T>> configure, out IMockVerifier<T> verifier)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new MockBuilder<T>();
            configure(builder);
            T mock = builder.Build(out verifier);
            _registrations[typeof(T)] = mock!;
            return this;
        }

        /// <inheritdoc />
        public ITestHarnessBuilder Replace<T>(T instance)
        {
            _registrations[typeof(T)] = instance!;
            return this;
        }

        /// <inheritdoc />
        public ITestHarness Build()
        {
            return new TestHarness(new Dictionary<Type, object>(_registrations));
        }
    }
}
