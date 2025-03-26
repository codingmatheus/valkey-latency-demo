using System.Diagnostics;
using ValkeyLatencyDemo;

class Program
{
    private static readonly string _runId = DateTime.UtcNow.Ticks.ToString(); 
    
    static async Task Main(string[] args)
    {
        
        var redisEndpoint = "clustercfg.valkey-demo.5slvnd.memorydb.us-west-2.amazonaws.com:6379";
        var client = new ValkeyClient(redisEndpoint);

        
        if (args is ["efficient", ..] )
        {
            
            await RunEfficientMode(client);
        }
        else
        {
            await RunInefficientMode(client);
        }
        
        Console.WriteLine("Done. Go check your MemoryDB latency metrics!");
    }

    static async Task RunEfficientMode(ValkeyClient client)
    {
        Console.WriteLine("== Running Efficient Write ==");
        await MeasureAsync(() => client.WriteEfficientAsync($"efficient:write:{_runId}"));
        
        Console.WriteLine("== Running Efficient Read ==");
        await MeasureAsync(() => client.ReadEfficientAsync($"test:read:{_runId}"));
    }

    static async Task RunInefficientMode(ValkeyClient client)
    {
        Console.WriteLine("== Running Inefficient Write ==");
        await MeasureAsync(() => client.WriteInefficientAsync($"inefficient:write:{_runId}"  + DateTime.UtcNow.Ticks));
        
        Console.WriteLine("== Running Inefficient Read ==");
        await MeasureAsync(() => client.ReadInefficientAsync($"test:read:{_runId}"));
    }

    static async Task MeasureAsync(Func<Task> action)
    {
        var sw = Stopwatch.StartNew();
        await action();
        sw.Stop();
        Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
    }
}