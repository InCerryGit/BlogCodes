using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<ListBenchmark>();

[MemoryDiagnoser]
public class ListBenchmark
{
    private List<int> _list = default!;

    // [Params(10, 1_000, 10_000, 100_000, 1_000_000)]
    [Params(1000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _list = new List<int>(Size);
        for (var i = 0; i < Size; i++)
        {
            _list.Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public void Foreach()
    {
        foreach (var item in _list)
        {
        }
    }

    [Benchmark]
    public void List_Foreach()
    {
        _list.ForEach(_ => { });
    }
    

    [Benchmark]
    public void For()
    {
        for (var i = 0; i < _list.Count; i++)
        {
            _ = _list[i];
        }
    }

    [Benchmark]
    public void Foreach_Span()
    {
        foreach (var item in CollectionsMarshal.AsSpan(_list))
        {
        }
    }

    [Benchmark]
    public void For_Span()
    {
        var span = CollectionsMarshal.AsSpan(_list);
        for (int i = 0; i < span.Length; i++)
        {
            _ = span[i];
        }
    }
}