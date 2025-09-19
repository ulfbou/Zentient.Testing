using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samples.Features.Tests
{
    // Shared types used across multiple feature test files

    public interface ISimpleService { string Do(string input); }
    public class SimpleServiceImpl : ISimpleService { public string Do(string input) => "impl:" + input; }
    public class SimpleHandler { private readonly ISimpleService _s; public SimpleHandler(ISimpleService s) => _s = s; public string Handle(string input) => _s.Do(input); }

    public class ResultDto { public int Value { get; set; } }
    public interface IResultProvider { ResultDto Get(int value); }
    public class ResultProvider : IResultProvider { public ResultDto Get(int value) => new ResultDto { Value = value }; }
    public class ResultHandler { private readonly IResultProvider _p; public ResultHandler(IResultProvider p) => _p = p; public ResultDto Handle(int input) => _p.Get(input); }

    public interface ICalculator { int Add(int a, int b); }
    public class AdderHandler { private readonly ICalculator _c; public AdderHandler(ICalculator c) => _c = c; public int Handle((int a,int b) input) => _c.Add(input.a, input.b); }
}
