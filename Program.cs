using OptimizelySDK;
using OptimizelySDK.Config;
using OptimizelySDK.Entity;

DotNetEnv.Env.Load();
var sdkKey = Environment.GetEnvironmentVariable("SDK_KEY_WITH_200_FLAGS");

var configManager = new HttpProjectConfigManager.Builder()
    .WithSdkKey(sdkKey)
    .Build(false); // sync mode

var optimizelyInstance = OptimizelyFactory.NewDefaultInstance(configManager);

var attributes = new UserAttributes
{
    { "accountIdGuid", "mike-chu-testing" },
    { "ring", "us" }
};
var output = optimizelyInstance.CreateUserContext("user123", attributes).DecideAll();

Console.WriteLine($"{output.Count} decisions made at {DateTime.Now}");
