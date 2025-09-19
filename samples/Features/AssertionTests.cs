using System.Threading.Tasks;
using Xunit;
using Zentient.Abstractions.Testing;
using Zentient.Testing;

namespace Samples.Features.Tests
{
    public class AssertionTests
    {
        [Fact]
        public async Task ResultAssertion_HaveValue_Works()
        {
            var scenario = TestScenario.ForHandler<ResultHandler, int, ResultDto>((h, input, ct) => Task.FromResult(h.Handle(input)));
            scenario.Arrange(b => b.WithDependency<IResultProvider>(new ResultProvider()));

            var result = await scenario.ActAsync(42);

            // Use the HaveValue helper to assert the result equals a ResultDto with Value=42
            scenario.Assert(a => a.HaveValue(new ResultDto { Value = 42 }));
        }

        [Fact]
        public async Task ResultAssertion_AndAlso_Chaining_Works()
        {
            var scenario = TestScenario.ForHandler<ResultHandler, int, ResultDto>((h, input, ct) => Task.FromResult(h.Handle(input)));
            scenario.Arrange(b => b.WithDependency<IResultProvider>(new ResultProvider()));

            var result = await scenario.ActAsync(7);

            // Demonstrate chaining: initial assertions then AndAlso to continue
            scenario.Assert(a => {
                a.NotBeNull();
                a.AndAlso.WithProperty(r => r.Value, 7);
            });
        }
    }
}
