using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

using Zentient.Abstractions.Testing;
using Zentient.Testing;
using Zentient.Testing.Internal;

namespace Samples.Features.Tests
{
    public class CallerFeatureTests
    {
        [Fact]
        public async Task Caller_Calls_Service_And_SpyRecordsCall()
        {
            var spy = new ServiceSpy();
            var caller = new Caller(spy);

            var result = caller.Run("in");

            Xunit.Assert.Equal("done", result);
            Xunit.Assert.True(spy.WasCalled);
            Xunit.Assert.Equal("in", spy.LastArg);

            await Task.CompletedTask;
        }

        private interface IService { string DoWork(string arg); }
        private class Caller { private readonly IService _svc; public Caller(IService svc) => _svc = svc; public string Run(string input) => _svc.DoWork(input); }
        private class ServiceSpy : IService
        {
            public bool WasCalled { get; private set; }
            public string? LastArg { get; private set; }

            public string DoWork(string arg)
            {
                WasCalled = true;
                LastArg = arg;
                return "done";
            }
        }
    }
}
