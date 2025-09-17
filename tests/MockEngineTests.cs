using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Zentient.Testing.Internal;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Tests
{
    /// <summary>
    /// Example tests demonstrating MockEngine functionality.
    /// </summary>
    public class MockEngineTests
    {
        /// <summary>
        /// Verifies overloads are supported and It.IsAny wildcard works.
        /// </summary>
        [Fact]
        public void Supports_Overloads_And_IsAny()
        {
            MockBuilder<ICalculator> builder = new MockBuilder<ICalculator>();
            builder.Given(x => x.Add(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(42);
            builder.Given(x => x.Add(It.IsAny<double>(), It.IsAny<double>())).ThenReturns(3.14);

            ICalculator calc = builder.Build(out IMockVerifier<ICalculator> verifier);

            int result1 = calc.Add(1, 2);
            double result2 = calc.Add(5.0, 7.0);

            result1.Should().Be(42);
            result2.Should().Be(3.14);

            verifier.ShouldHaveBeenCalled("Add");
            verifier.ShouldHaveBeenCalledTimes("Add", 2);
        }

        /// <summary>
        /// Verifies that methods returning Task{T} are supported.
        /// </summary>
        [Fact]
        public async Task Supports_Task_Returns()
        {
            MockBuilder<ICalculator> builder = new MockBuilder<ICalculator>();
            builder.Given(x => x.MultiplyAsync(It.IsAny<int>(), It.IsAny<int>())).ThenReturns(Task.FromResult(99));

            ICalculator calc = builder.Build(out IMockVerifier<ICalculator> verifier);

            int result = await calc.MultiplyAsync(10, 10).ConfigureAwait(false);

            result.Should().Be(99);
            verifier.ShouldHaveBeenCalled("MultiplyAsync");
        }
    }

    /// <summary>
    /// Simple calculator interface used by example tests.
    /// </summary>
    public interface ICalculator
    {
        int Add(int a, int b);
        double Add(double a, double b);
        Task<int> MultiplyAsync(int a, int b);
    }
}
