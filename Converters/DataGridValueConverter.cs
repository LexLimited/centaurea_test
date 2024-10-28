using System.Text.Json;
using System.Text.Json.Serialization;
using CentaureaTest.Models;

namespace CentaureaTest.Converters
{

    public sealed class DataGridValueConverter : JsonConverter<DataGridValue>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DataGridValue).IsAssignableFrom(typeToConvert);
        }

        public override DataGridValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("DataGridValueConverter.Read is not implemented");
        }

        public override void Write(Utf8JsonWriter writer, DataGridValue value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("id");
            writer.WriteNumberValue(value.Id);

            writer.WritePropertyName("type");
            writer.WriteStringValue(value.Type.ToString());

            writer.WritePropertyName("fieldId");
            writer.WriteNumberValue(value.FieldId);

            writer.WritePropertyName("rowIndex");
            writer.WriteNumberValue(value.RowIndex);

            switch (value)
            {
                case DataGridNumericValue numericValue:
                    writer.WritePropertyName("numericValue");
                    writer.WriteNumberValue(numericValue.Value);
                    break;

                case DataGridStringValue stringValue:
                    writer.WritePropertyName("stringValue");
                    writer.WriteStringValue(stringValue.Value);
                    break;

                case DataGridEmailValue emailValue:
                    writer.WritePropertyName("stringValue");
                    writer.WriteStringValue(emailValue.Value);
                    break;

                case DataGridRegexValue regexValue:
                    writer.WritePropertyName("stringValue");
                    writer.WriteStringValue(regexValue.Value);
                    break;

                case DataGridRefValue refValue:
                    writer.WritePropertyName("referencedFieldId");
                    writer.WriteNumberValue(refValue.ReferencedFieldId);
                    break;

                case DataGridSingleSelectValue singleSelectValue:
                    writer.WritePropertyName("optiondId");
                    writer.WriteNumberValue(singleSelectValue.OptionId);
                    break;

                case DataGridMultiSelectValue multiSelectValue:
                    writer.WritePropertyName("optionIds");
                    writer.WriteStringValue(multiSelectValue.OptionIds.ToString());
                    break;

                default: break;
            }

            writer.WriteEndObject();
        }
    }

}