using StackExchange.Redis;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var connString = "equal-sawfish-133598.upstash.io:6379,password=gQAAAAAAAgneAAIgcDFlNWU3Mjg5ZmRjM2Q0YTA5OTU0NDc5MDNmNzRjYWFhNA,ssl=true,abortConnection=False";
        var config = ConfigurationOptions.Parse(connString, true);
        config.Ssl = true;
        config.AbortOnConnectFail = false;
        config.ConnectRetry = 3;
        using var mux = await ConnectionMultiplexer.ConnectAsync(config);
        var db = mux.GetDatabase();
        var key = "test-redis-write";
        await db.StringSetAsync(key, "hello-world", TimeSpan.FromMinutes(10));
        var value = await db.StringGetAsync(key);
        Console.WriteLine($"Wrote key={key}, value={value}");
        await db.KeyDeleteAsync(key);
        Console.WriteLine("Deleted test key");
    }
}
