// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Xunit;
using Zentient.Expressions;
using Zentient.Abstractions.Expressions;

namespace Zentient.Testing.Tests.Unit
{
    public class ParserTests
    {
        [Fact]
        public void Parse_SimpleIdentifier()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.True(parser.TryParse("name", out var expr, out var diags));
            Assert.Equal(ExpressionKind.Identifier, expr!.Kind);
            Assert.Equal("name", expr.Canonical);
        }

        [Fact]
        public void Parse_MemberAccess_And_MethodCall()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.True(parser.TryParse("obj.add(1, 2)", out var expr, out var diags));
            Assert.Equal(ExpressionKind.MethodCall, expr!.Kind);
            Assert.Equal("obj.add(1, 2)", expr.Canonical);
        }

        [Fact]
        public void Parse_Lambda_SingleParameter()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.True(parser.TryParse("x => x", out var expr, out var diags));
            Assert.Equal(ExpressionKind.Lambda, expr!.Kind);
            Assert.Contains("=>", expr.Canonical);
        }

        [Fact]
        public void Parse_Empty_ReturnsDiagnostic()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.False(parser.TryParse("", out var expr, out var diags));
            Assert.True(diags.Any());
        }
    }
}
