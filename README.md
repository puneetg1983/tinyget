# TinyGet

A simple HTTP load testing tool built with .NET 8 that allows you to make parallel HTTP requests and analyze response times.

## Features

- Make HTTP GET requests to any URL
- Execute multiple requests in parallel for load testing
- Optional response body logging
- Performance metrics and timing analysis
- Cross-platform compatibility (.NET 8)
- Professional command-line interface with help system

## Installation

### Prerequisites
- .NET 8 SDK installed on your machine

### Build from Source
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd tinyget
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build -c Release
   ```

4. (Optional) Create a standalone executable:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true
   ```

## Usage

### Basic Syntax
```
tinyget <url> [options]
```

### Arguments
- `<url>` - The URL to make HTTP requests to (required)

### Options
- `-x, --threads <number>` - Number of requests to make in parallel (default: 1)
- `-t, --trace` - Log the response body to console
- `-?, -h, --help` - Show help and usage information
- `--version` - Show version information

### Examples

#### Single Request
```bash
tinyget https://example.com
```

#### Load Testing with 50 Parallel Requests
```bash
tinyget https://api.example.com/health -x 50
```

#### Load Testing with Response Body Logging
```bash
tinyget https://api.example.com/data -x 10 -t
```

#### Using Long Form Options
```bash
tinyget https://httpbin.org/json --threads 25 --trace
```

### Sample Output

#### Without Trace (-t)
```
Making 5 parallel requests to: https://httpbin.org/status/200
Log response body: No
--------------------------------------------------
[002] OK (345ms)
[004] OK (385ms)
[001] OK (454ms)
[005] OK (412ms)
[003] OK (534ms)
--------------------------------------------------
All 5 requests completed in 591 ms
Average time per request: 118.20 ms
```

#### With Trace (-t)
```
Making 2 parallel requests to: https://httpbin.org/json
Log response body: Yes
--------------------------------------------------
[001] OK (384ms) - Content Length: 429
Response Body:
{
  "slideshow": {
    "author": "Yours Truly",
    ...
  }
}
------------------------------
[002] OK (361ms) - Content Length: 429
Response Body:
{
  "slideshow": {
    "author": "Yours Truly",
    ...
  }
}
------------------------------
--------------------------------------------------
All 2 requests completed in 418 ms
Average time per request: 209.00 ms
```

## Running the Application

### Development Mode
```bash
cd tinyget
dotnet run -- https://example.com -x 10
```

### Standalone Executable
After publishing, you can run the executable directly:
```bash
# Windows
.\bin\Release\net8.0\win-x64\publish\tinyget.exe https://example.com -x 10

# Linux/macOS (after publishing for respective platform)
./bin/Release/net8.0/linux-x64/publish/tinyget https://example.com -x 10
```

## Error Handling

TinyGet handles various error scenarios gracefully:

- **Invalid URLs** - Validates URL format before making requests
- **HTTP Errors** - Displays HTTP error messages with timing
- **Network Timeouts** - 30-second timeout with clear timeout messages
- **General Exceptions** - Catches and displays unexpected errors

## Performance Metrics

The tool provides comprehensive performance information:
- Individual request timing for each parallel request
- Total execution time for all requests
- Average time per request
- Request numbering for easy tracking
- HTTP status codes for each request

## Use Cases

- **API Load Testing** - Test how your API handles concurrent requests
- **Performance Benchmarking** - Compare response times across different endpoints
- **Health Checks** - Verify service availability and response times
- **Stress Testing** - Identify bottlenecks with high concurrency
- **Development Testing** - Quick testing during development cycles

## Technical Details

- **Framework**: .NET 8
- **HTTP Client**: Built-in `HttpClient` with connection reuse
- **Command Line Parsing**: `System.CommandLine` library for robust argument handling
- **Concurrency**: True parallel execution using `Task.WhenAll()`
- **Timeout**: 30-second timeout per request
- **Output Format**: Structured console output with timing metrics

## Dependencies

- `System.CommandLine` (2.0.0-beta4.22272.1) - Command line argument parsing

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

[Add your license information here]

## Support

For issues, questions, or contributions, please [create an issue](link-to-issues) in the repository.