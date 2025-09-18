// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zentient.Testing.Internal;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Tests
{
    public class MoreMockTests
    {
        public interface ICalc
        {
            int Add(int a, int b);
            int Divide(int a, int b);
            Task<int> MultiplyAsync(int a, int b);
            string? GetText();
        }

        [Fact]
        public void Behavior_priority_and_ordering()
        {
            var builder = new MockBuilder<ICalc>();

            // Add specific behavior after wildcard - wildcard should win because it was added first
            builder.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(42);
            builder.Given(x => x.Add(1, 2)).ThenReturns(5);

            var calc = builder.Build(out var verifier);

            calc.Add(1, 2).Should().Be(42, because: "wildcard was registered before the specific rule");
            calc.Add(3, 4).Should().Be(42);

            // Now register specific first then wildcard
            var builder2 = new MockBuilder<ICalc>();
            builder2.Given(x => x.Add(1, 2)).ThenReturns(5);
            builder2.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(42);

            var calc2 = builder2.Build(out var verifier2);
            calc2.Add(1, 2).Should().Be(5, because: "specific rule registered before wildcard");
            calc2.Add(9, 9).Should().Be(42);
        }

        [Fact]
        public void ThenThrows_throws_expected_exception()
        {
            var builder = new MockBuilder<ICalc>();
            builder.Given(x => x.Divide(1, 0)).ThenThrows(new DivideByZeroException("boom"));
            var calc = builder.Build(out var v);

            Action act = () => calc.Divide(1, 0);
            act.Should().Throw<DivideByZeroException>().WithMessage("boom");
            v.Should().NotBeNull();
        }

        [Fact]
        public async Task Async_Task_return_and_exception_behavior()
        {
            var builder = new MockBuilder<ICalc>();
            builder.Given(x => x.MultiplyAsync(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(Task.FromResult(7));
            var calc = builder.Build(out var v);

            var r = await calc.MultiplyAsync(3, 4);
            r.Should().Be(7);

            // now throw from async method
            var builder2 = new MockBuilder<ICalc>();
            builder2.Given(x => x.MultiplyAsync(It.IsAny<int>(), It.IsAny<int>())).ThenThrows(new InvalidOperationException("async fail"));
            var calc2 = builder2.Build(out var v2);

            Func<Task> act = async () => await calc2.MultiplyAsync(1, 2);
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("async fail");
        }

        [Fact]
        public void Default_behavior_returns_defaults()
        {
            var builder = new MockBuilder<ICalc>();
            var calc = builder.Build(out var v);

            calc.Add(10, 20).Should().Be(0);
            calc.GetText().Should().BeNull();
        }

        [Fact]
        public void ThenReturns_before_Given_throws()
        {
            var builder = new MockBuilder<ICalc>();
            Action act = () => builder.ThenReturns(1);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task Concurrency_multiple_calls_recorded()
        {
            var builder = new MockBuilder<ICalc>();
            builder.Given(x => x.MultiplyAsync(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(Task.FromResult(2));
            var calc = builder.Build(out var v);

            var tasks = Enumerable.Range(0, 50).Select(i => calc.MultiplyAsync(i, i));
            var results = await Task.WhenAll(tasks);
            results.Should().OnlyContain(x => x == 2);

            // verify call count recorded in the verifier via MockEngine internals
            // verifier is not exposing counts per-method except ShouldHaveBeenCalledTimes
            v.Should().NotBeNull();
            v.ShouldHaveBeenCalledTimes("MultiplyAsync", 50);
        }
    }
}
