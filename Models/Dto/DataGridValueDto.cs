using System.Text;

namespace CentaureaTest.Models.Dto
{

    /// <summary>
    /// This class exist because I couldn't figure out how to deeserialize
    /// polymorphic objects.
    /// Not used internally, only used to receive jsons.
    /// </summary>
    public sealed class DataGridValueDto
    {
        public int FieldId { get; set; }
        public DataGridValueType Type { get; set; }
        public string? StringValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public string? RegexValue { get; set; }
        public int? ReferencedFieldId { get; set; }
        public int? OptionId { get; set; }
        public List<int>? OptionIds { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            
            builder.AppendLine($"\tFieldId: {FieldId}");
            builder.AppendLine($"\tType: {Type}");
            builder.AppendLine($"\tStringValue: {StringValue}");
            builder.AppendLine($"\tDecimalValue: {DecimalValue}");
            builder.AppendLine($"\tRegexValue: {RegexValue}");
            builder.AppendLine($"\tReferenceFieldId: {ReferencedFieldId}");
            builder.AppendLine($"\tOptionId: {OptionId}");
            builder.AppendLine($"\tOptionIds: {OptionIds}");

            return builder.ToString();
        }
    }

    public static class DataGridValueExtension
    {
        public static DataGridValue ToDataGridValue(this DataGridValueDto valueDto)
        {
            return valueDto.Type switch
            {
                DataGridValueType.String => new DataGridStringValue(
                    valueDto.StringValue ?? throw new Exception("'StringValue' field expected for type 'String'")
                ),
                DataGridValueType.Numeric => new DataGridNumericValue(
                    valueDto.DecimalValue ?? throw new Exception("'DecimalValue' field expected for type 'Numeric'")
                ),
                DataGridValueType.Regex => new DataGridRegexValue(
                    valueDto.RegexValue ?? throw new Exception("'RegexValue' field expected for type 'Regex'")
                ),
                DataGridValueType.Ref => new DataGridRefValue(
                    valueDto.ReferencedFieldId ?? throw new Exception("'RefencedFieldId' field expected for type 'Ref'")
                ),
                DataGridValueType.SingleSelect => new DataGridSingleSelectValue(
                    valueDto.OptionId ?? throw new Exception("'OptionId' field expected for type 'SingleSelect'")
                ),
                DataGridValueType.MultiSelect => new DataGridMultiSelectValue(
                    valueDto.OptionIds ?? throw new Exception("'OptionIds' field expected for type 'MultiSelect'")
                ),
                _ => throw new Exception($"ToDataGridValue unhandled type {valueDto.Type}"),
            };
        }

        public static IEnumerable<DataGridValue> ToDataGridValues(this IEnumerable<DataGridValueDto> valueDtos)
        {
            return valueDtos.Select(ToDataGridValue);
        }
    }

}