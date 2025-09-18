// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Xunit;
using Zentient.Expressions;
using Zentient.Abstractions.Expressions;

namespace Zentient.Testing.Tests.Unit
{
    public class StubEvaluatorTests
    {
        [Fact]
        public void ResolveIdentifier_FromDictionary_ReturnsValue()
        {
            var expr = ExpressionRegistry.DefaultParser.Parse("name");
            var ctx = new Dictionary<string, object?> { ["name"] = 99 };
            var res = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, ctx);
            Assert.Equal(99, res);
        }

        [Fact]
        public void ResolveMember_FromDictionary_ReturnsValue()
        {
            var expr = ExpressionRegistry.DefaultParser.Parse("obj.value");
            var obj = new Dictionary<string, object?> { ["value"] = "x" };
            var ctx = new Dictionary<string, object?> { ["obj"] = obj };
            var res = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, ctx);
            Assert.Equal("x", res);
        }

        private class Foo { public int Number { get; set; } = 7; public int Get(int x) => x + 1; }

        [Fact]
        public void ResolveMember_ReflectsProperty_WhenNotDictionary()
        {
            var expr = ExpressionRegistry.DefaultParser.Parse("obj.Number");
            var ctx = new Dictionary<string, object?> { ["obj"] = new Foo() };
            var res = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, ctx);
            Assert.Equal(7, res);
        }

        [Fact]
        public void EvaluateMethodCall_DelegateOnTarget_Invokes()
        {
            var expr = ExpressionRegistry.DefaultParser.Parse("obj.add(2,3)");
            var obj = new Dictionary<string, object?>();
            obj["add"] = new Func<int,int,int>((a,b)=>a+b);
            var ctx = new Dictionary<string, object?> { ["obj"] = obj };
            var res = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, ctx);
            Assert.Equal(5, res);
        }

        [Fact]
        public void EvaluateMethodCall_ContextLookup_InvokesDelegate()
        {
            var expr = ExpressionRegistry.DefaultParser.Parse("obj.sum(4,6)");
            var inner = new Dictionary<string, object?> { ["sum"] = new Func<double,double,double>((a,b)=>a+b) };
            var ctx = new Dictionary<string, object?> { ["obj"] = inner };
            var res = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, ctx);
            Assert.Equal(10.0, res);
        }

        [Fact]
        public void EvaluateMethodCall_ArgConversion_Works()
        {
            var expr = ExpressionRegistry.DefaultParser.Parse("obj.mul(2, 3.0)");
            var obj = new Dictionary<string, object?>();
            obj["mul"] = new Func<double,double,double>((a,b)=>a*b);
            var ctx = new Dictionary<string, object?> { ["obj"] = obj };
            var res = ExpressionRegistry.DefaultEvaluator.Evaluate(expr, ctx);
            Assert.Equal(6.0, res);
        }
    }
}
