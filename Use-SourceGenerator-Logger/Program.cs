#define Benchmark

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;

#if Benchmark
_ = BenchmarkRunner.Run<Benchmark>();

[HtmlExporter]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Benchmark
{
    private static readonly ILogger Logger = new EmptyLogger();
    private readonly OrderLogger _orderLogger = new(Logger);
    private readonly Member _user = new("8888","Justin Yu");
    private readonly DateTime _now = DateTime.UtcNow;

    [Benchmark]
    public void LogByStringInterpolation() => _orderLogger.LogByStringInterpolation(_user, _now);

    [Benchmark]
    public void LogByStructure() => _orderLogger.LogByStructure(_user, _now);

    [Benchmark]
    public void LogBySourceGenerator() => OrderLogger.LogBySourceGenerator(Logger, _user, _now);
}
#else

var logger = LoggerFactory.Create(l => l.AddConsole()).CreateLogger(typeof(OrderLogger));
var orderLogger = new OrderLogger(logger);
var member = new Member("8888","Justin Yu");
orderLogger.LogByStringInterpolation(member, DateTime.Now);
orderLogger.LogByStructure(member, DateTime.Now);
OrderLogger.LogBySourceGenerator(logger, member, DateTime.Now);

#endif

/// <summary>
/// 订单日志记录类
/// </summary>
public partial class OrderLogger
{
    private readonly ILogger _logger;
    
    public OrderLogger(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 字符串插值
    /// </summary>
    public void LogByStringInterpolation(Member member, DateTime now)=>
        _logger.LogInformation($"会员[{member}]在[{now:yyyy-MM-dd HH:mm:ss}]充值了一个小目标");

    /// <summary>
    /// 参数化
    /// </summary>
    public void LogByStructure(Member member, DateTime now) =>
        _logger.LogInformation("会员[{Member}]在[{Now:yyyy-MM-dd HH:mm:ss}]充值了一个小目标", member, now);

    /// <summary>
    /// 源代码生成
    /// </summary>
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information, 
        Message = "会员[{member}]在[{Now:yyyy-MM-dd HH:mm:ss}]充值了一个小目标")]
    public static partial void LogBySourceGenerator(ILogger logger, Member member, DateTime now);
}

/// <summary>
/// 会员
/// </summary>
/// <param name="MemberId">会员Id</param>
/// <param name="Name">会员名</param>
public record Member(string MemberId, string Name);

public class EmptyLogger : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    { }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => default;
}