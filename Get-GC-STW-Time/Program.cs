using System.Diagnostics.Tracing;

// 开启GC事件监听
var gc = new GcStwMetricsCollector();
// 创建一些对象
var array = Enumerable.Range(0, 1000).Select(s => (decimal)s).ToArray();
// 手动执行GC
GC.Collect();
Console.WriteLine($"API STW:{GC.GetTotalPauseDuration().TotalMilliseconds}ms");
Console.ReadLine();

public class GcStwMetricsCollector : EventListener
{
    // GC关键字
    private const int GC_KEYWORD = 0x0000001;
    
    // 我们要关注的GC事件
    private const int GCSuspendEEBegin = 8;
    private const int GCRestartEEEnd = 3;

    private EventSource? _eventSource;
    
    public void Stop()
    {
        if (_eventSource == null)
            return;

        DisableEvents(_eventSource);
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        _eventSource = eventSource;
        // GC 事件在 Microsoft-Windows-DotNETRuntime 名称空间下 
        if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
        {
            // 启用事件，事件级别为Informational， 只监听GC事件
            EnableEvents(eventSource, EventLevel.Informational, (EventKeywords) (GC_KEYWORD));
        }
    }

    
    private long _currentStwStartTime = 0;
    
    protected override void OnEventWritten(EventWrittenEventArgs e)
    {
        switch (e.EventId)
        {
            // 冻结托管线程开始，记录当前时间
            case GCSuspendEEBegin:
                _currentStwStartTime = e.TimeStamp.Ticks;
                break;
            
            // 恢复托管线程结束，计算当前时间与冻结托管线程开始时间的差值
            case GCRestartEEEnd:
                if (_currentStwStartTime > 0)
                {
                    var ms = TimeSpan.FromTicks(e.TimeStamp.Ticks - _currentStwStartTime).TotalMilliseconds;
                    _currentStwStartTime = 0;
                    // 输出结果
                    Console.WriteLine($"Event STW: {ms}ms");
                }
                break;
        }
    }
}