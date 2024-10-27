using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CentaureaTest.Models;

namespace CentaureaTest.Converters
{

    public sealed class DataGridFieldSignatureConverter : JsonConverter<DataGridFieldSignature>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DataGridFieldSignature).IsAssignableFrom(typeToConvert);
        }

        public override DataGridFieldSignature? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("DataGridFieldSignatureConverter.Read is not implemented");

            // === Version 2 (untested and unfinished) ===

            // var regexConverter = (JsonConverter<DataGridRegexFieldSignature>)options.GetConverter(typeof(DataGridRegexFieldSignature));
            
            // var regexVariant = regexConverter.Read(ref reader, typeof(DataGridRefFieldSignature), options);
            // if (regexVariant is not null)
            // {
            //     return regexVariant;
            // }

            // var baseConverter = (JsonConverter<DataGridFieldSignature>)options.GetConverter(typeof(DataGridFieldSignature));
            // return baseConverter.Read(ref reader, typeof(DataGridFieldSignature), options);

            // === Version 1 (untested and unfinished) ===

            // if (reader.TokenType != JsonTokenType.StartObject)
            // {
            //     throw new JsonException("DataGridFieldSignature must be an object");
            // }

            // var ret = new DataGridFieldSignature();

            // while (reader.Read())
            // {
            //     if (reader.TokenType == JsonTokenType.EndObject)
            //     {
            //         return ret;
            //     }

            //     if (reader.TokenType != JsonTokenType.PropertyName)
            //     {
            //         var propertyName = reader.GetString()
            //             ?? throw new Exception("Failed to read property name");

            //         var property = ret.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            //             ?? throw new Exception($"Failed to get property {propertyName}");


            //         property.SetValue(ret, "value");
            //     }
            // }
        }

        public override void Write(Utf8JsonWriter writer, DataGridFieldSignature value, JsonSerializerOptions options)
        {
            var fieldSignature = value as DataGridFieldSignature
                ?? throw new Exception($"DataGridFieldSignatureConverter: object {value} cannot be casted to DataGridFieldSignature");

            writer.WriteStartObject();
            
            writer.WritePropertyName("name");
            writer.WriteStringValue(fieldSignature.Name);

            writer.WritePropertyName("type");
            writer.WriteStringValue(fieldSignature.Type.ToString());

            switch (fieldSignature)
            {
                case DataGridRegexFieldSignature regexField:
                    writer.WritePropertyName("regexPattern");
                    writer.WriteStringValue(regexField.RegexPattern);
                    break;

                case DataGridRefFieldSignature refField:
                    writer.WritePropertyName("referencedGridId");
                    writer.WriteNumberValue(refField.ReferencedGridId);
                    break;

                case DataGridSingleSelectFieldSignature singleSelectField:
                    writer.WritePropertyName("optionTableId");
                    writer.WriteNumberValue(singleSelectField.OptionTableId);
                    break;

                case DataGridMultiSelectFieldSignature multiSelectField:
                    writer.WritePropertyName("optionTableId");
                    writer.WriteNumberValue(multiSelectField.OptionTableId);
                    break;

                default: break;
            }

            writer.WriteEndObject();
        }
    }

}