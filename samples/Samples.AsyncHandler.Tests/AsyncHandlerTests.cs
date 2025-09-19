using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Zentient.Testing;
using Zentient.Abstractions.Testing;
using Zentient.Testing.Internal;

namespace Samples.AsyncHandler.Tests
{
    public class AsyncHandlerTests
    {
        public interface IWorker
        {
            Task<string> WorkAsync(string input);
        }

        public class AsyncHandler
        {
            private readonly IWorker _worker;
            public AsyncHandler(IWorker worker) => _worker = worker;
            public Task<string> HandleAsync(string input, CancellationToken ct) => _worker.WorkAsync(input);
        }

        [Fact]
        public async Task AsyncHandler_Uses_Worker()
        {
            var scenario = TestScenario.ForHandler<AsyncHandler, string, string>(
                (handler, input, ct) => handler.HandleAsync(input, ct)
            );

            scenario.Arrange(b => b.WithMock<IWorker>(mb => mb.Given(x => x.WorkAsync(It.IsAny<string>())).ThenReturns(Task.FromResult("ok"))));

            var result = await scenario.ActAsync("input");

            // assertions
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("ok", result);
        }
    }
}
