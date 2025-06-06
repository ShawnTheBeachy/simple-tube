using FluentValidation;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

public sealed record YouTubeOptions
{
    public string ApiKey { get; set; } = "";
    public const string SectionName = "YouTube";

    internal sealed class Validator : AbstractValidator<YouTubeOptions>
    {
        public Validator()
        {
            RuleFor(x => x.ApiKey).NotEmpty();
        }
    }
}
