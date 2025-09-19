using System.Threading.Tasks;
using Xunit;
using Zentient.Testing;
using Zentient.Abstractions.Testing;
using Zentient.Testing.Internal;

namespace Samples.SyncHandler.Tests
{
    public class SynchronousHandlerTests
    {
        public interface ICalculator
        {
            int Add(int a, int b);
        }

        public class AdderHandler
        {
            private readonly ICalculator _calc;
            public AdderHandler(ICalculator calc) => _calc = calc;
            public int Handle((int a, int b) input) => _calc.Add(input.a, input.b);
        }

        private class CalculatorStub : ICalculator
        {
            public int Add(int a, int b) => 123;
        }

        [Fact]
        public async Task AdderHandler_Returns_MockedResult()
        {
            var handler = new AdderHandler(new CalculatorStub());

            int result = handler.Handle((2, 3));

            Xunit.Assert.Equal(123, result);
            await Task.CompletedTask;
        }
    }
}
