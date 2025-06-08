using FluentValidation;

namespace SimpleTube.RestApi.Infrastructure.Images;

public sealed record ImageOptions
{
    public string Location { get; set; } = "";
    public const string SectionName = "Images";

    internal sealed class Validator : AbstractValidator<ImageOptions>
    {
        public Validator()
        {
            RuleFor(x => x.Location).NotEmpty();
        }
    }
}
