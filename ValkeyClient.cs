using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ValkeyLatencyDemo;

public class ValkeyClient
{
    private readonly IDatabase _db;

    public ValkeyClient(string endpoint)
    {
        var config = new ConfigurationOptions
        {
            EndPoints = { endpoint },
            Ssl = true,
            AbortOnConnectFail = false
        };
        
        var redis = ConnectionMultiplexer.Connect(config);
        _db = redis.GetDatabase();
    }

    public async Task WriteInefficientAsync(string keyPrefix)
    {
        for (int i = 0; i < 100; i++)
        {
            var payload = new
            {
                playerId = $"player-{i}",
                action = "ATTACK",
                target = "AncientRedDragon",
                damage = 200,
                weapon = "Sword of a Thousand Truths",
                timestamp = DateTime.UtcNow.ToString("O"),
                location = new { x = 124.56, y = 78.91, zone = "ShadowRealm" },
                inventory = Enumerable.Range(0, 10).Select(n => $"item-{n}").ToArray(),
                buffs = new { haste = true, shield = false },
                metadata = new { session = Guid.NewGuid().ToString(), server = "NA-East-14" }
            };

            string json = JsonSerializer.Serialize(payload);
            await _db.StringSetAsync($"{keyPrefix}:{i}", json);
        }
    }

    public async Task WriteEfficientAsync(string keyPrefix)
    {
        for (int i = 0; i < 100; i++)
        {
            // Just what’s necessary for the system to function
            var compactPayload = new
            {
                id = i,
                dmg = 200,
                ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            string json = JsonSerializer.Serialize(compactPayload);
            await _db.StringSetAsync($"{keyPrefix}:{i}", json);
        }
    }
    
    public async Task ReadInefficientAsync(string key)
    {
        await SetupReadTestData(key);
        
        await _db.HashGetAllAsync(key);
    }

    public async Task ReadEfficientAsync(string key)
    {
        await SetupReadTestData(key);
        
        var fields = new[] { "field1", "field3" };
        
        var redisFields = Array.ConvertAll(fields, f => (RedisValue)f);
        await _db.HashGetAsync(key, redisFields);
    }

    private async Task SetupReadTestData(string key)
    {
        var entries = new List<HashEntry>();
        
        for (int i = 0; i < 200; i++)
        {
            var entry = new HashEntry($"field{i}", $"val{i}");
            entries.Add(entry);
        }
        
        await _db.HashSetAsync($"{key}", entries.ToArray());
    }
}
