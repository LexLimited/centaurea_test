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