using System.CommandLine;
using System.Diagnostics;
using System.Net;

namespace tinyget
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var urlArgument = new Argument<string>(
                "url",
                "The URL to make HTTP requests to")
            {
                Arity = ArgumentArity.ExactlyOne
            };

            var threadsOption = new Option<int>(
                aliases: new[] { "-x", "--threads" },
                description: "Number of requests to make in parallel",
                getDefaultValue: () => 1);

            var traceOption = new Option<bool>(
                aliases: new[] { "-t", "--trace" },
                description: "Log the response body to console");

            var rootCommand = new RootCommand("TinyGet - A simple HTTP load testing tool")
            {
                urlArgument,
                threadsOption,
                traceOption
            };

            rootCommand.SetHandler(async (url, threads, trace) =>
            {
                await ExecuteRequests(url, threads, trace);
            }, urlArgument, threadsOption, traceOption);

            return await rootCommand.InvokeAsync(args);
        }

        static async Task ExecuteRequests(string url, int parallelCount, bool logResponse)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.WriteLine($"Error: '{url}' is not a valid URL.");
                return;
            }

            Console.WriteLine($"Making {parallelCount} parallel requests to: {url}");
            Console.WriteLine($"Log response body: {(logResponse ? "Yes" : "No")}");
            Console.WriteLine(new string('-', 50));

            var stopwatch = Stopwatch.StartNew();
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            try
            {
                // Create tasks for parallel execution
                var tasks = new List<Task>();
                for (int i = 0; i < parallelCount; i++)
                {
                    int requestId = i + 1;
                    tasks.Add(MakeHttpRequest(httpClient, url, requestId, logResponse));
                }

                // Wait for all requests to complete
                await Task.WhenAll(tasks);

                stopwatch.Stop();
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"All {parallelCount} requests completed in {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Average time per request: {stopwatch.ElapsedMilliseconds / (double)parallelCount:F2} ms");
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        static async Task MakeHttpRequest(HttpClient httpClient, string url, int requestId, bool logResponse)
        {
            var requestStopwatch = Stopwatch.StartNew();
            
            try
            {
                var response = await httpClient.GetAsync(url);
                requestStopwatch.Stop();

                var statusCode = response.StatusCode;
                var elapsed = requestStopwatch.ElapsedMilliseconds;

                if (logResponse)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[{requestId:D3}] {statusCode} ({elapsed}ms) - Content Length: {content.Length}");
                    Console.WriteLine($"Response Body:\n{content}\n{new string('-', 30)}");
                }
                else
                {
                    Console.WriteLine($"[{requestId:D3}] {statusCode} ({elapsed}ms)");
                }
            }
            catch (HttpRequestException ex)
            {
                requestStopwatch.Stop();
                Console.WriteLine($"[{requestId:D3}] HTTP Error ({requestStopwatch.ElapsedMilliseconds}ms): {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                requestStopwatch.Stop();
                Console.WriteLine($"[{requestId:D3}] Timeout ({requestStopwatch.ElapsedMilliseconds}ms)");
            }
            catch (Exception ex)
            {
                requestStopwatch.Stop();
                Console.WriteLine($"[{requestId:D3}] Error ({requestStopwatch.ElapsedMilliseconds}ms): {ex.Message}");
            }
        }
    }
}
