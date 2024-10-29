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
        public decimal? NumericValue { get; set; }
        public int? IntValue { get; set; }
        public int RowIndex { get; set; }
        public List<int>? IntListValue { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            
            builder.AppendLine($"\tType: {Type}");
            builder.AppendLine($"\tStringValue: {StringValue}");
            builder.AppendLine($"\tNumericValue: {NumericValue}");
            builder.AppendLine($"\tIntValue: {IntValue}");
            builder.AppendLine($"\tIntListValue: {IntListValue}");

            return builder.ToString();
        }
    }

}