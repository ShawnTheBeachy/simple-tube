using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleTube.RestApi.Rest.Channels;

namespace SimpleTube.RestApi.Rest;

internal static class JsonSerialization
{
    public static void AddRestApiJsonConverters(this JsonSerializerOptions jsonOptions)
    {
        jsonOptions.Converters.Add(new RestEntityArrayConverter<ChannelRestEntity>());
        jsonOptions.Converters.Add(new RestEntityConverter<ChannelRestEntity>());
    }

    private sealed class RestEntityConverter<T> : JsonConverter<RestEntity<T>>
        where T : class
    {
        public override RestEntity<T> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            throw new NotSupportedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            RestEntity<T> value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStartObject("#entity");
            writer.WriteString("url", value.Url);
            writer.WriteEndObject();
            var entityTypeInfo = options.GetTypeInfo(typeof(T));

            foreach (var property in entityTypeInfo.Properties)
            {
                var propertyValue = property.Get?.Invoke(value.Entity);

                if (propertyValue is null)
                    continue;

                var propertyTypeInfo = options.GetTypeInfo(property.PropertyType);
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, propertyValue, propertyTypeInfo);
            }
        }
    }

    private sealed class RestEntityArrayConverter<T> : JsonConverter<RestEntity<T>[]>
        where T : class
    {
        public override RestEntity<T>[] Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            throw new NotSupportedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            RestEntity<T>[] values,
            JsonSerializerOptions options
        )
        {
            var entityTypeInfo = options.GetTypeInfo(typeof(RestEntity<T>));
            writer.WriteStartArray();

            foreach (var value in values)
            {
                writer.WriteStartObject();
                JsonSerializer.Serialize(writer, value, entityTypeInfo);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
