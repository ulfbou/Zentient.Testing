using System.Threading.Tasks;
using Xunit;
using Zentient.Testing;
using Zentient.Abstractions.Testing;
using Zentient.Testing.Internal;

namespace Samples.FeatureDemos.Tests
{
    // This project contains one example test per feature listed in the exhaustive feature list.
    // Each test demonstrates a tiny usage of the harness / mock DSL.

    public class FeatureDemos
    {
        [Fact]
        public async Task Demo_Arrange_WithDependency()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithDependency<ISimpleService>(new SimpleServiceImpl()));
            var result = await scenario.ActAsync("in");
            Xunit.Assert.Equal("impl:in", result);
        }

        [Fact]
        public async Task Demo_Arrange_WithMock_ThenReturns()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithMock<ISimpleService>(mb => mb.Given(s => s.Do(It.IsAny<string>())).ThenReturns("mocked")));
            var result = await scenario.ActAsync("in");
            Xunit.Assert.Equal("mocked", result);
        }

        // Additional small demos could be added here for each feature.
    }

    public interface ISimpleService { string Do(string input); }
    public class SimpleServiceImpl : ISimpleService { public string Do(string input) => "impl:" + input; }
    public class SimpleHandler { private readonly ISimpleService _s; public SimpleHandler(ISimpleService s) => _s = s; public string Handle(string input) => _s.Do(input); }
}
