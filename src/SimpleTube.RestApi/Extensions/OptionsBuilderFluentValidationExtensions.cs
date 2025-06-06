using FluentValidation;
using Microsoft.Extensions.Options;

namespace SimpleTube.RestApi.Extensions;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateWithFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder
    )
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            services => new FluentValidationValidateOptions<TOptions>(
                optionsBuilder.Name,
                services.CreateScope().ServiceProvider.GetRequiredService<IValidator<TOptions>>()
            )
        );
        return optionsBuilder;
    }

    public static OptionsBuilder<TOptions> ValidateWithFluentValidation<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder,
        IValidator<TOptions> validator
    )
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            new FluentValidationValidateOptions<TOptions>(optionsBuilder.Name, validator)
        );
        return optionsBuilder;
    }
}

file sealed class FluentValidationValidateOptions<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    public FluentValidationValidateOptions(string? name, IValidator<TOptions> validator)
    {
        _name = name;
        _validator = validator;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // Null name is used to configure all named options.
        if (_name != null && _name != name)
        {
            // Ignored if not validating this instance.
            return ValidateOptionsResult.Skip;
        }

        // Ensure options are provided to validate against
        ArgumentNullException.ThrowIfNull(options);

        var validationResult = _validator.Validate(options);

        if (validationResult.IsValid)
            return ValidateOptionsResult.Success;

        var typeName = options.GetType().Name;

        return ValidateOptionsResult.Fail(
            validationResult.Errors.Select(error =>
                $"FluentValidation validation failed for '{typeName}' member: '{error.PropertyName}' with the error: '{error.ErrorMessage}'."
            )
        );
    }

    private readonly string? _name;
    private readonly IValidator<TOptions> _validator;
}
