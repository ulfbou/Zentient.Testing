// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Zentient.Abstractions.Expressions;
using Zentient.Expressions;
using Zentient.Testing;
using FluentAssertions;

namespace Zentient.Testing.Tests
{
    public class ExpressionIntegrationTests
    {
        [Fact]
        public void ParseAndEvaluateConstant_NumberAndString()
        {
            var parser = ExpressionRegistry.DefaultParser;

            Assert.True(parser.TryParse("123", out var numExpr, out var diags1));
            var numResult = ExpressionRegistry.DefaultEvaluator.Evaluate(numExpr!, null);
            Assert.Equal(123.0, numResult);

            Assert.True(parser.TryParse("\"hello\\nworld\"", out var strExpr, out var diags2));
            var strResult = ExpressionRegistry.DefaultEvaluator.Evaluate(strExpr!, null);
            Assert.Equal("hello\nworld", strResult);
        }

        [Fact]
        public void ParseAndEvaluate_Identifier_FromContext()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.True(parser.TryParse("name", out var expr, out var diags));

            var ctx = new Dictionary<string, object?> { ["name"] = 42 };
            var result = ExpressionRegistry.DefaultEvaluator.Evaluate(expr!, ctx);
            Assert.Equal(42, result);
        }

        [Fact]
        public void ParseAndEvaluate_MemberAccess_And_MethodCall_OnTargetDictionary()
        {
            var parser = ExpressionRegistry.DefaultParser;
            // expression: obj.add(10, 5)
            string text = "obj.add(10, 5)";
            Assert.True(parser.TryParse(text, out var expr, out var diags));

            // Prepare target object with delegate under key "add"
            var obj = new Dictionary<string, object?>();
            obj["add"] = new Func<double, double, double>((a, b) => a + b);

            var ctx = new Dictionary<string, object?> { ["obj"] = obj };
            var result = ExpressionRegistry.DefaultEvaluator.Evaluate(expr!, ctx);

            Assert.Equal(15.0, result);
        }

        [Fact]
        public void TypedExpression_AsTyped_ConvertsNumbersToInt()
        {
            var parser = ExpressionRegistry.DefaultParser;
            Assert.True(parser.TryParse("42", out var expr, out var diags));

            var typed = ExpressionRegistry.AsTyped<int>(expr!);
            var val = typed.Evaluate();
            Assert.Equal(42, val);
        }

        [Fact]
        public void ExpressionRegistry_OnParsedAndOnEvaluated_FiresEvents()
        {
            int parsedCount = 0;
            int evaluatedCount = 0;

            Action<IExpression> onParsed = _ => parsedCount++;
            Action<IExpression, object?> onEval = (_, __) => evaluatedCount++;

            ExpressionRegistry.OnParsed += onParsed;
            ExpressionRegistry.OnEvaluated += onEval;

            try
            {
                var parser = ExpressionRegistry.DefaultParser;
                Assert.True(parser.TryParse("100", out var expr, out var diags));
                // Parse should have raised event at least once
                parsedCount.Should().BeGreaterThanOrEqualTo(1);

                var r = ExpressionRegistry.DefaultEvaluator.Evaluate(expr!, null);
                evaluatedCount.Should().BeGreaterThanOrEqualTo(1);
            }
            finally
            {
                ExpressionRegistry.OnParsed -= onParsed;
                ExpressionRegistry.OnEvaluated -= onEval;
            }
        }

        private class HandlerThatUsesParser
        {
            private readonly IExpressionParser _parser;

            public HandlerThatUsesParser(IExpressionParser parser)
            {
                _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            }

            public int Handle(string expression)
            {
                var expr = _parser.Parse(expression);
                var val = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, null);
                if (val is double d) return (int)d;
                if (val is int i) return i;
                throw new InvalidOperationException("Unexpected evaluation result");
            }
        }

        [Fact]
        public async Task TestHarness_Resolves_Handler_WithRegisteredParser()
        {
            var scenario = TestScenario.ForHandler<HandlerThatUsesParser, string, int>((handler, input, ct) => Task.FromResult(handler.Handle(input)));

            scenario.Arrange(builder => builder.WithDependency<IExpressionParser>(ExpressionRegistry.DefaultParser));

            var result = await scenario.ActAsync("7");
            Assert.Equal(7, result);
        }
    }
}
