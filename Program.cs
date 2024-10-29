using System.Diagnostics;
using OptimizelySDK;
using OptimizelySDK.Config;
using OptimizelySDK.Entity;

namespace benchmark_testing;

internal static class Program
{
    // Best Practice: Initialize the OptimizelyInstance only once
    private static readonly Optimizely OptimizelyInstance;

    static Program()
    {
        DotNetEnv.Env.Load();
        var sdkKey = Environment.GetEnvironmentVariable("SDK_KEY_WITH_200_FLAGS");

        var configManager = new HttpProjectConfigManager.Builder()
            .WithSdkKey(sdkKey)
            .Build(false); // sync mode

        OptimizelyInstance = OptimizelyFactory.NewDefaultInstance(configManager);
    }

    public static void Main()
    {
        const int iterations = 1000;
        RunBenchmark(iterations);
    }

    private static void RunBenchmark(int iterations)
    {
        var runStopwatch = Stopwatch.StartNew();

        var results = new Dictionary<int, int>();

        Console.WriteLine($"Running {iterations} iterations, with timing output at the end...");

        for (int i = 0; i < iterations; i++)
        {
            int iteration = i;

            var taskStopwatch = Stopwatch.StartNew();

            var attributes = new UserAttributes
            {
                { "accountIdGuid", $"mike-chu-testing-{iteration}" },
                { "ring", "us" }
            };

            var userContext = OptimizelyInstance.CreateUserContext($"user{iteration}", attributes);
            _ = userContext?.DecideAll();

            taskStopwatch.Stop();

            results.TryAdd(iteration, (int)taskStopwatch.ElapsedMilliseconds);
        }

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
