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
        public DataGridValueType Type { get; set; }
        public string? StringValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public int? IntValue { get; set; }
        public List<int>? IntListValue { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            
            builder.AppendLine($"\tType: {Type}");
            builder.AppendLine($"\tStringValue: {StringValue}");
            builder.AppendLine($"\tDecimalValue: {DecimalValue}");
            builder.AppendLine($"\tIntValue: {IntValue}");
            builder.AppendLine($"\tIntListValue: {IntListValue}");

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
                DataGridValueType.Email => new DataGridEmailValue(
                    valueDto.StringValue ?? throw new Exception("'StringValue' field expected for type 'Email'")
                ),
                DataGridValueType.Regex => new DataGridRegexValue(
                    valueDto.StringValue ?? throw new Exception("'StringValue' field expected for type 'Regex'")
                ),
                DataGridValueType.Ref => new DataGridRefValue(
                    valueDto.IntValue ?? throw new Exception("'IntValue' field expected for type 'Ref'")
                ),
                DataGridValueType.SingleSelect => new DataGridSingleSelectValue(
                    valueDto.IntValue ?? throw new Exception("'IntValue' field expected for type 'SingleSelect'")
                ),
                DataGridValueType.MultiSelect => new DataGridMultiSelectValue(
                    valueDto.IntListValue ?? throw new Exception("'IntListValue' field expected for type 'MultiSelect'")
                ),
                _ => throw new NotImplementedException($"ToDataGridValue doesn't handle type {valueDto.Type}"),
            };
        }

        public static IEnumerable<DataGridValue> ToDataGridValues(this IEnumerable<DataGridValueDto> valueDtos)
        {
            return valueDtos.Select(ToDataGridValue);
        }
    }

}