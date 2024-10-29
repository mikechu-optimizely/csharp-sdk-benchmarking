using System.Collections.Concurrent;
using System.Diagnostics;
using OptimizelySDK;
using OptimizelySDK.Config;
using OptimizelySDK.Entity;

namespace benchmark_testing;

internal static class ProgramAsync
{
    // Best Practice: Initialize the OptimizelyInstance only once
    private static readonly Optimizely OptimizelyInstance;

    static ProgramAsync()
    {
        DotNetEnv.Env.Load();
        var sdkKey = Environment.GetEnvironmentVariable("SDK_KEY_WITH_200_FLAGS");

        var configManager = new HttpProjectConfigManager.Builder()
            .WithSdkKey(sdkKey)
            .Build(false); // sync mode

        OptimizelyInstance = OptimizelyFactory.NewDefaultInstance(configManager);
    }

    public static async Task Main()
    {
        const int iterations = 1000;
        await RunBenchmarkAsync(iterations);
    }

    private static async Task RunBenchmarkAsync(int iterations)
    {
        var runStopwatch = Stopwatch.StartNew();

        var tasks = new List<Task>();
        var results = new ConcurrentDictionary<int, int>();

        Console.WriteLine($"Running {iterations} iterations, with timing output at the end...");

        for (int i = 0; i < iterations; i++)
        {
            int iteration = i;
            tasks.Add(Task.Run(async () =>
            {
                var taskStopwatch = Stopwatch.StartNew();

                var attributes = new UserAttributes
                {
                    { "accountIdGuid", $"mike-chu-testing-{iteration}" },
                    { "ring", "us" }
                };

                var userContext = OptimizelyInstance.CreateUserContext($"user{iteration}", attributes);
                _ = await Task.Run(() => userContext?.DecideAll());

                taskStopwatch.Stop();

                results.TryAdd(iteration, (int)taskStopwatch.ElapsedMilliseconds);
            }));
        }

        await Task.WhenAll(tasks);

        runStopwatch.Stop();
        Console.WriteLine(
            $"Finished running {iterations} iterations after {runStopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine(
            $"Average time per iteration: {results.Values.Average()} ms");
        Console.WriteLine(
            $"Median time per iteration: {results.Values.OrderBy(x => x).ElementAt(iterations / 2)} ms");
        Console.WriteLine(
            $"Fastest time per iteration: {results.Values.Min()} ms");
        Console.WriteLine(
            $"Slowest time per iteration: {results.Values.Max()} ms");
    }
}
