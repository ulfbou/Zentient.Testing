using System;
using System.Threading.Tasks;
using Xunit;
using Zentient.Abstractions.Testing;
using Zentient.Testing;

namespace Samples.Features.Tests
{
    public class MockingTests
    {
        [Fact]
        public async Task Arrange_WithMock_ReturnsConfiguredValue()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithMock<ISimpleService>(mb => mb.Given(s => s.Do(It.IsAny<string>())).ThenReturns("mocked")));
            var result = await scenario.ActAsync("in");
            Xunit.Assert.Equal("mocked", result);
        }

        [Fact]
        public async Task Arrange_WithMock_ThenThrowsException()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithMock<ISimpleService>(mb => mb.Given(s => s.Do("bad")).ThenThrows(new InvalidOperationException("boom"))));

            await Xunit.Assert.ThrowsAsync<InvalidOperationException>(() => scenario.ActAsync("bad"));
        }

        [Fact]
        public async Task Mock_Verifier_Records_Calls_OutParam()
        {
            IMockVerifier<ISimpleService> verifier = null!;
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));

            await scenario
                .Arrange(b => b.WithMock<ISimpleService>(mb => mb.Given(s => s.Do(It.IsAny<string>())).ThenReturns("done"), out verifier))
                .ActAsync("hello");

            verifier.ShouldHaveBeenCalled(nameof(ISimpleService.Do));
            verifier.ShouldHaveBeenCalledTimes(nameof(ISimpleService.Do), 1);
        }

        [Fact]
        public async Task Mock_MultipleBehaviors_LaterSpecificOverridesEarlier()
        {
            var scenario = TestScenario.ForHandler<SimpleHandler, string, string>((h, i, ct) => Task.FromResult(h.Handle(i)));
            scenario.Arrange(b => b.WithMock<ISimpleService>(mb => {
                mb.Given(s => s.Do("specific")).ThenReturns("spec");
                mb.Given(s => s.Do(It.IsAny<string>())).ThenReturns("any");
            }));

            var r1 = await scenario.ActAsync("specific");
            var r2 = await scenario.ActAsync("other");

            Xunit.Assert.Equal("spec", r1);
            Xunit.Assert.Equal("any", r2);
        }

        [Fact]
        public async Task Mock_Void_Method_VerifiedCallCount()
        {
            var scenario = TestScenario.ForHandler<VoidCaller, string, string>((h, i, ct) => Task.FromResult(h.Run(i)));
            IMockVerifier<IVoidService> verifier = null!;

            await scenario
                .Arrange(b => b.WithMock<IVoidService>(mb => mb.Given(s => s.Do(It.IsAny<int>())).ThenReturns(null), out verifier))
                .ActAsync("1");

            verifier.ShouldHaveBeenCalledTimes(nameof(IVoidService.Do), 1);
        }

        private interface IVoidService { void Do(int value); }
        private class VoidCaller { private readonly IVoidService _svc; public VoidCaller(IVoidService svc) => _svc = svc; public string Run(string input) { if (int.TryParse(input, out var v)) _svc.Do(v); return "ok"; } }
    }
}
