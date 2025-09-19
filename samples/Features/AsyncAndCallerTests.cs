using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Zentient.Abstractions.Testing;
using Zentient.Testing;

namespace Samples.Features.Tests
{
    public class AsyncTests
    {
        [Fact]
        public async Task Async_Mock_Returns_TaskResult()
        {
            var scenario = TestScenario.ForHandler<AsyncHandlerDemo, string, string>((h, i, ct) => h.HandleAsync(i, ct));
            scenario.Arrange(b => b.WithMock<IAsyncService>(mb => mb.Given(s => s.GetAsync(It.IsAny<string>())).ThenReturns(Task.FromResult("ok-async"))));

            var result = await scenario.ActAsync("input");
            Xunit.Assert.Equal("ok-async", result);
        }

        [Fact]
        public async Task AsyncHandler_Uses_WorkerToProduceResult()
        {
            var scenario = TestScenario.ForHandler<AsyncHandler, string, string>(
                (handler, input, ct) => handler.HandleAsync(input, ct)
            );

            scenario.Arrange(b => b.WithMock<IWorker>(mb => mb.Given(x => x.WorkAsync(It.IsAny<string>())).ThenReturns(Task.FromResult("ok"))));

            var result = await scenario.ActAsync("input");

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("ok", result);
        }

        // grouped utilities
        private interface IAsyncService { Task<string> GetAsync(string input); }
        private class AsyncHandlerDemo { private readonly IAsyncService _svc; public AsyncHandlerDemo(IAsyncService svc) => _svc = svc; public Task<string> HandleAsync(string input, CancellationToken ct) => _svc.GetAsync(input); }

        private interface IWorker { Task<string> WorkAsync(string input); }
        private class AsyncHandler { private readonly IWorker _worker; public AsyncHandler(IWorker worker) => _worker = worker; public Task<string> HandleAsync(string input, CancellationToken ct) => _worker.WorkAsync(input); }
    }
}
