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
        }

        public override void Write(Utf8JsonWriter writer, DataGridFieldSignature value, JsonSerializerOptions options)
        {
            var fieldSignature = value as DataGridFieldSignature
                ?? throw new Exception($"DataGridFieldSignatureConverter: object {value} cannot be casted to DataGridFieldSignature");

            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteNumberValue(fieldSignature.Id);
            
            writer.WritePropertyName("name");
            writer.WriteStringValue(fieldSignature.Name);

            writer.WritePropertyName("type");
            writer.WriteStringValue(fieldSignature.Type.ToString());

            writer.WritePropertyName("order");
            writer.WriteNumberValue(fieldSignature.Order);

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

                default: break;
            }

            writer.WriteEndObject();
        }
    }

}