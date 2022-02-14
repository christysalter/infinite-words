using System.IO;
using infinite_words.api.Configuration;
using infinite_words.api.Service;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(infinite_words.api.Startup))]

namespace infinite_words.api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("local.settings.json", true, true)
            .Build();

        builder.Services.AddScoped<ISecretConfig>(_ => new SecretConfig(config["salt"]));
        builder.Services.AddScoped<IWordService, WordService>();
        
        builder.Services.AddTransient<IWordService, WordService>();
        builder.Services.AddTransient<IContinuationTokenService, ContinuationTokenService>();
        builder.Services.AddTransient<IValidationService, ValidationService>();
        builder.Services.AddTransient<IColourService, ColourService>();
        builder.Services.AddTransient<IGuessService, GuessService>();
        builder.Services.AddTransient<IKeyboardService, KeyboardService>();
        builder.Services.AddTransient<IEncryptionService, EncryptionService>();
        builder.Services.AddTransient<IGameService, GameService>();
        builder.Services.AddTransient<IValidWordsService, ValidWordsService>();
    }
}