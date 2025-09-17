using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Zentient.Testing.Internal;
using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Tests
{
    public class HarnessMockVerifierTests
    {
        public interface IService
        {
            void Do();
        }

        public class Handler
        {
            private readonly IService _svc;
            public Handler(IService svc) => _svc = svc;
            public void Handle() => _svc.Do();
        }

        [Fact]
        public void WithMock_out_returns_verifier()
        {
            IMockVerifier<IService> verifier;

            var builder = new TestHarnessBuilder();
            builder.WithMock<IService>(mb => { mb.Given(x => x.Do()).ThenReturns((object?)null); }, out verifier);

            using var harness = (TestHarness)builder.Build();
            var handler = harness.Resolve<Handler>();
            handler.Handle();

            verifier.ShouldHaveBeenCalled("Do");
        }
    }
}
