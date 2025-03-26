# MemoryDB Latency Demo: Optimize Redis Operations for Better Performance

A .NET application that demonstrates efficient vs inefficient Redis operations when working with Amazon MemoryDB. The demo shows how to optimize read/write patterns to reduce latency and improve overall performance.

The application implements two modes of operation to showcase Redis best practices:
- Efficient mode: Uses optimized data structures and targeted field retrieval
- Inefficient mode: Uses verbose data structures and overfetches data

## Repository Structure
```
.
├── Program.cs                 # Main entry point, handles command line args and runs demo modes
├── ValkeyClient.cs           # Core Redis client implementation with efficient/inefficient operations
├── ValkeyLatencyDemo.csproj  # Project file with dependencies and build settings
└── _assets/                  # Documentation assets and screenshots
```

## Usage Instructions
### Prerequisites
- .NET 9.0 SDK
- Access to an Amazon MemoryDB cluster
- StackExchange.Redis NuGet package

### Installation
1. Clone the repository
```bash
git clone [repository-url]
cd ValkeyLatencyDemo
```

2. Restore dependencies
```bash 
dotnet restore
```

3. Build the project
```bash
dotnet build
```

### Quick Start
1. Update the Redis endpoint in Program.cs to point to your MemoryDB cluster

2. Run in efficient mode:
```bash
dotnet run efficient
```

3. Run in inefficient mode:
```bash
dotnet run
```

### More Detailed Examples
The demo implements two key patterns:

**Efficient Write Operations:**
```csharp
// Only store necessary fields
var compactPayload = new {
    id = i,
    dmg = 200,
    ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
};
```

**Efficient Read Operations:**
```csharp
// Only fetch required fields
var fields = new[] { "field1", "field3" };
await _db.HashGetAsync(key, redisFields);
```

### Troubleshooting
Common issues and solutions:

- Connection failures
    - Verify MemoryDB endpoint is correct
    - Check security group allows access
    - Ensure SSL is enabled

- High latency
    - Monitor CloudWatch metrics
    - Check for network issues
    - Verify efficient patterns are used

- Memory usage spikes
    - Review data structures
    - Implement data expiration
    - Monitor CloudWatch metrics

## Data Flow
The application demonstrates Redis operation patterns by writing and reading test data using efficient and inefficient approaches.

```ascii
[Client] --> [MemoryDB Cluster]
     |              |
     |-- Write --> Hash Storage
     |              |
     |-- Read  --> Field Retrieval
```

Key interactions:
- Client connects to MemoryDB using StackExchange.Redis
- Writes test data using string or hash data structures
- Reads data using targeted or full hash retrieval
- Measures operation latency for comparison
- Reports metrics to CloudWatch for analysis
- Demonstrates impact of data structure choices
- Shows benefits of targeted field retrieval