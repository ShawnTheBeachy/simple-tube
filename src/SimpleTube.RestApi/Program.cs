using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Infrastructure;
using SimpleTube.RestApi.Queries;
using SimpleTube.RestApi.Rest;
using SimpleTube.RestApi.Rest.Channels;
using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Mediator;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder
    .Services.AddInfrastructure(builder.Configuration)
    .AddMediator()
    .AddCommands()
    .AddQueries()
    .AddValidatorsFromAssemblies(
        [typeof(Program).Assembly, typeof(IMediator).Assembly],
        includeInternalTypes: true
    );

builder.Services.AddOpenApi();
builder.Services.AddResponseCaching().AddResponseCompression().AddOutputCache();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.Configure<JsonSerializerOptions>(opts =>
{
    opts.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.UseOutputCache();
app.UseResponseCaching();
app.UseResponseCompression();
app.UseStaticFiles();
app.MapAppEndpoints().MapOpenApi();
app.Run();

[JsonSerializable(typeof(AppEndpoints.Bookmark[]))]
[JsonSerializable(typeof(Channel))]
[JsonSerializable(typeof(Channel[]))]
[JsonSerializable(typeof(SubscribeCommand))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;
