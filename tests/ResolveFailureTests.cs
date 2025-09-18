// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using FluentAssertions;
using Xunit;
using Zentient.Testing.Internal;

namespace Zentient.Testing.Tests
{
    public class ResolveFailureTests
    {
        private interface INonRegistered { }
        private class NeedsDep
        {
            public NeedsDep(INonRegistered dep) { }
        }

        [Fact]
        public void Resolve_MissingDependency_ThrowsDescriptiveException()
        {
            var builder = new TestHarnessBuilder();
            using var harness = (TestHarness)builder.Build();

            Action act = () => harness.Resolve<NeedsDep>();

            act.Should().Throw<InvalidOperationException>()
                .Where(ex => ex.Message.Contains("Could not resolve type") && ex.Message.Contains("Register the following dependency types"));
        }
    }
}
