// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Xunit;
using Zentient.Abstractions.Expressions;
using Zentient.Expressions;

namespace Zentient.Testing.Tests.Unit
{
    public class LexerTests
    {
        [Fact]
        public void EmptyInput_ProducesDiagnostic()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.False(parser.TryParse("", out var expr, out var diags));
            Assert.True(diags.Any());
        }

        [Fact]
        public void Parse_Number_String_Identifier_And_Lambda()
        {
            var parser = ExpressionRegistry.DefaultParser;

            Assert.True(parser.TryParse("abc", out var idExpr, out var d1));
            Assert.Equal(ExpressionKind.Identifier, idExpr!.Kind);

            Assert.True(parser.TryParse("123", out var numExpr, out var d2));
            Assert.Equal(ExpressionKind.Constant, numExpr!.Kind);

            Assert.True(parser.TryParse("\"hi\"", out var strExpr, out var d3));
            Assert.Equal(ExpressionKind.Constant, strExpr!.Kind);

            Assert.True(parser.TryParse("x => x", out var lambdaExpr, out var d4));
            Assert.Equal(ExpressionKind.Lambda, lambdaExpr!.Kind);
        }

        [Fact]
        public void UnterminatedString_ProducesDiagnostic()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.False(parser.TryParse("\"unterminated", out var expr, out var diags));
            var msg = string.Join(';', diags.Select(d => d.Message)).ToLowerInvariant();
            Assert.Contains("unterminated", msg);
        }
    }
}
