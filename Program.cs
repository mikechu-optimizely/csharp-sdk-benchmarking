using System.Diagnostics;
using OptimizelySDK;
using OptimizelySDK.Config;
using OptimizelySDK.Entity;

namespace benchmark_testing;

internal static class Program
{
    // Best Practice: Initialize the OptimizelyInstance only once
    private static readonly Optimizely? OptimizelyInstance;

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
        var runStopwatch = new Stopwatch();
        runStopwatch.Start();

        for (int i = 0; i < iterations; i++)
        {
            int iteration = i;
            var taskStopwatch = new Stopwatch();
            taskStopwatch.Start();

            var attributes = new UserAttributes
            {
                { "accountIdGuid", $"mike-chu-testing-{iteration}" },
                { "ring", "us" }
            };

            var userContext =
                OptimizelyInstance?.CreateUserContext($"user{iteration}", attributes);
            var decisions = userContext?.DecideAll();

            taskStopwatch.Stop();

            if (decisions != null)
            {
                Console.WriteLine(
                    $"DecideAll for user {iteration}, Decision Count {decisions.Count} after {taskStopwatch.ElapsedMilliseconds} ms");
            }
        }

        runStopwatch.Stop();
        Console.WriteLine(
            $"Finished running {iterations} iterations after {runStopwatch.ElapsedMilliseconds} ms");
    }
}
