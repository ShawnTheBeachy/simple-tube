using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.ResponseCompression;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Commands.Internal;
using SimpleTube.RestApi.Exceptions;
using SimpleTube.RestApi.Infrastructure;
using SimpleTube.RestApi.Infrastructure.Images;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Queries.Internal;
using SimpleTube.RestApi.Rest;
using SimpleTube.RestApi.Rest.Entities;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;

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

builder.Services.AddSlimMessageBus(opts =>
{
    opts.AddAspNet().WithProviderMemory().AutoDeclareFromAssemblyContaining<Program>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();
builder.Services.AddResponseCompression(opts =>
{
    opts.EnableForHttps = true;
    opts.Providers.Add<BrotliCompressionProvider>();
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream", "font/woff2"]
    );
});
builder.Services.AddResponseCaching().AddOutputCache();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.Configure<JsonSerializerOptions>(opts =>
{
    opts.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler(opts =>
{
    opts.StatusCodeSelector = ex =>
        ex switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };
});
builder.Services.AddCors();

var app = builder.Build();
app.UseExceptionHandler();
app.UseCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseOutputCache();
app.UseResponseCaching();
app.UseResponseCompression();
app.UseImages();
app.UseStaticFiles();
app.MapAppEndpoints().MapOpenApi();
app.Run();

[JsonSerializable(typeof(AppEndpoints.Bookmark[]))]
[JsonSerializable(typeof(Channel))]
[JsonSerializable(typeof(Channel[]))]
[JsonSerializable(typeof(DownloadVideoCommand))]
[JsonSerializable(typeof(IgnoreVideoCommand))]
[JsonSerializable(typeof(SubscribeCommand))]
[JsonSerializable(typeof(Video))]
[JsonSerializable(typeof(Video[]))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;
