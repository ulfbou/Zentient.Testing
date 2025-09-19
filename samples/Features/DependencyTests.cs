using System.Threading.Tasks;
using Xunit;
using Zentient.Abstractions.Testing;
using Zentient.Testing;

namespace Samples.Features.Tests
{
    public class DependencyTests
    {
        [Fact]
        public async Task Arrange_WithDependency_UsesConcreteImplementation()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithDependency<ISimpleService>(new SimpleServiceImpl()));
            var result = await scenario.ActAsync("in");
            Xunit.Assert.Equal("impl:in", result);
        }

        [Fact]
        public async Task Replace_Registration_ReplacesDependency()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithDependency<ISimpleService>(new SimpleServiceImpl()));
            scenario.Arrange(b => b.Replace<ISimpleService>(new OverrideService()));

            var result = await scenario.ActAsync("x");
            Xunit.Assert.Equal("override:x", result);
        }

        private class OverrideService : ISimpleService { public string Do(string input) => "override:" + input; }
    }
}
