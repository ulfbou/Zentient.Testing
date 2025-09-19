// <copyright file="ExpressionIntegrationTests.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using FluentAssertions;

using System.Threading;
using System.Threading.Tasks;

using Xunit;

using Zentient.Testing;
using Zentient.Testing.Internal;

using static Zentient.Testing.Tests.HarnessMockVerifierTests;

namespace Zentient.Testing.Tests
{
    public class HarnessTests
    {
        public interface ICalculator
        {
            int Add(int a, int b);
        }

        internal class AdderHandler
        {
            private readonly ICalculator _calc;
            public AdderHandler(ICalculator calc) => _calc = calc;
            public int Handle((int a, int b) input) => _calc.Add(input.a, input.b);
        }

        [Fact]
        public async Task Handler_resolves_dependencies_and_mock()
        {
            var scenario = TestScenario.ForHandler<AdderHandler, (int a, int b), int>((h, input, ct) => Task.FromResult(h.Handle(input)));

            scenario.Arrange(builder =>
            {
                builder.WithMock<ICalculator>(mb => mb.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(100));
            });

            int result = await scenario.ActAsync((1, 2));

            result.Should().Be(100);
        }
    }
}
