using System.Text.Json;
using System.Text.Json.Serialization;

namespace DAL.ModelView.Settings;

public class FileUploadDTO
{
    public string? Name { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class FileUploadDtoJsonConverter : JsonConverter<FileUploadDTO?>
{
    public override FileUploadDTO? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            return new FileUploadDTO { Data = reader.GetString() ?? string.Empty, Extension = "png" };
        }

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected object or string for file upload.");

        string? name = null;
        string extension = string.Empty;
        string data = string.Empty;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName?.ToLowerInvariant())
            {
                case "name":
                    name = reader.GetString();
                    break;
                case "extension":
                    extension = reader.GetString() ?? string.Empty;
                    break;
                case "data":
                    data = reader.TokenType == JsonTokenType.String ? reader.GetString() ?? string.Empty : string.Empty;
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        return new FileUploadDTO
        {
            Name = name,
            Extension = extension,
            Data = data
        };
    }

    public override void Write(Utf8JsonWriter writer, FileUploadDTO? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("name", value.Name);
        writer.WriteString("extension", value.Extension);
        writer.WriteString("data", value.Data);
        writer.WriteEndObject();
    }
}
