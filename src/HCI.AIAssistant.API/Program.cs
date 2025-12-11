// using HCI.AIAssistant.API.Services;
// using HCI.AIAssistant.API.Models.CustomTypes;
// using Microsoft.Extensions.Options;
// using Azure.Identity;
// using HCI.AIAssistant.API.Managers;


// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: "CORS",
//     policy =>
//     {
//         policy
//         .AllowAnyHeader()
//         .AllowAnyMethod()
//         .AllowAnyOrigin();
//     });
// });

// // Replace appsettings.json values with Key Vault values
// var keyVaultName = builder.Configuration[
//     $"AppConfigurations{ConfigurationPath.KeyDelimiter}KeyVaultName"];

// var secretsPrefix = builder.Configuration[
//     $"AppConfigurations{ConfigurationPath.KeyDelimiter}SecretsPrefix"];

// if (string.IsNullOrWhiteSpace(keyVaultName))
// {
//     throw new ArgumentNullException("KeyVaultName", "KeyVaultName is missing.");
// }

// if (string.IsNullOrWhiteSpace(secretsPrefix))
// {
//     throw new ArgumentNullException("SecretsPrefix", "SecretsPrefix is missing.");
// }

// var keyVaultUri = new Uri(
//     $"https://{keyVaultName}.vault.azure.net/");

// builder.Configuration.AddAzureKeyVault(
//     keyVaultUri,
//     new DefaultAzureCredential(),
//     new CustomSecretManager(secretsPrefix)
// );


// // Configure values based on appsettings.json
// builder.Services.Configure<SecretsService>(builder.Configuration.GetSection("Secrets"));
// builder.Services.Configure<AppConfigurationsService>(builder.Configuration.GetSection("AppConfigurations"));

// // Add services to the container.
// builder.Services.AddSingleton<ISecretsService>(
//     provider => provider.GetRequiredService<IOptions<SecretsService>>().Value
// );
// builder.Services.AddSingleton<IAppConfigurationsService>(
//     provider => provider.GetRequiredService<IOptions<AppConfigurationsService>>().Value
// );

// builder.Services.AddSingleton<IParametricFunctions, ParametricFunctions>();


// // Add services to the container.

// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// app.UseCors("CORS");

// // Configure the HTTP request pipeline.
// // if (app.Environment.IsDevelopment())
// // {
// app.UseSwagger();
// app.UseSwaggerUI();
// //}

// if (app.Environment.IsProduction())
// {
//     app.UseHttpsRedirection();
// }


// app.UseAuthorization();

// app.MapControllers();

// // Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.EndPoint);
// // Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.Key);
// // Console.WriteLine(app.Services.GetService<ISecretsService>()?.AIAssistantSecrets?.Id);

// // Console.WriteLine(app.Services.GetService<ISecretsService>()?.IoTHubSecrets?.ConnectionString);

// // Console.WriteLine(app.Services.GetService<IAppConfigurationsService>()?.KeyVaultName);
// // Console.WriteLine(app.Services.GetService<IAppConfigurationsService>()?.SecretsPrefix);
// // Console.WriteLine(app.Services.GetService<IAppConfigurationsService>()?.IoTDeviceName);


// app.Run();

using Azure.Identity;
using HCI.AIAssistant.API.Managers;
using HCI.AIAssistant.API.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURARE CORS (Permite Frontend-ului să se conecteze)
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CORS",
    policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

// 2. CONECTARE KEY VAULT (Cu protecție la erori)
var keyVaultName = builder.Configuration["AppConfigurations:KeyVaultName"];
var secretsPrefix = builder.Configuration["AppConfigurations:SecretsPrefix"];

Console.WriteLine($"[STARTUP] Connecting to KeyVault: '{keyVaultName}' with Prefix: '{secretsPrefix}'");

if (!string.IsNullOrWhiteSpace(keyVaultName) && !string.IsNullOrWhiteSpace(secretsPrefix))
{
    try
    {
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(
            keyVaultUri,
            new DefaultAzureCredential(),
            new CustomSecretManager(secretsPrefix)
        );
        Console.WriteLine("[SUCCESS] KeyVault connected.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] KeyVault connection failed: {ex.Message}");
    }
}
else
{
    Console.WriteLine("[WARNING] KeyVault settings missing! App will run with empty secrets.");
}

// 3. INJECTARE SERVICII
builder.Services.Configure<SecretsService>(builder.Configuration.GetSection("Secrets"));
builder.Services.Configure<AppConfigurationsService>(builder.Configuration.GetSection("AppConfigurations"));

// Înregistrare Singleton pentru setări (Soluția robustă)
builder.Services.AddSingleton<ISecretsService>(sp =>
    sp.GetRequiredService<IOptions<SecretsService>>().Value);

builder.Services.AddSingleton<IAppConfigurationsService>(sp =>
    sp.GetRequiredService<IOptions<AppConfigurationsService>>().Value);

builder.Services.AddSingleton<IParametricFunctions, ParametricFunctions>();
builder.Services.AddScoped<IAIAssistantService, AIAssistantService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("CORS");
app.UseSwagger();
app.UseSwaggerUI(); // Lăsăm Swagger activ și în producție pentru debug

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();