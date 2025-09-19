using System.Threading.Tasks;
using Xunit;
using Zentient.Abstractions.Testing;
using Zentient.Testing;

namespace Samples.Features.Tests
{
    public class AdderTests
    {
        [Fact]
        public async Task UnmatchedMock_Returns_DefaultValue_ForValueType()
        {
            var scenario = TestScenario.ForHandler<AdderHandler, (int a, int b), int>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithMock<ICalculator>(mb => { /* no behaviors configured */ }));

            var result = await scenario.ActAsync((2,3));
            Xunit.Assert.Equal(0, result);
        }

        [Fact]
        public async Task Mock_ItIsAny_WithNumerics_ReturnsConfiguredValue()
        {
            var scenario = TestScenario.ForHandler<AdderHandler, (int a, int b), int>((h, input, ct) => Task.FromResult(h.Handle(input)));
            scenario.Arrange(b => b.WithMock<ICalculator>(mb => mb.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(42)));

            var result = await scenario.ActAsync((3,4));
            Xunit.Assert.Equal(42, result);
        }

        [Fact]
        public async Task AdderHandler_Returns_MockedResult_WhenUsingStub()
        {
            var handler = new AdderHandler(new CalculatorStub());

            int result = handler.Handle((2, 3));

            Xunit.Assert.Equal(123, result);
            await Task.CompletedTask;
        }

        private class CalculatorStub : ICalculator { public int Add(int a, int b) => 123; }
    }
}
