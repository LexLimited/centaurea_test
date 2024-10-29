namespace CentaureaTest.Models.Dto
{

    public static class DtoExtensions
    {
        public static DataGridValue ToDataGridValue(this DataGridValueDto valueDto)
        {
            DataGridValue value = valueDto.Type switch
            {
                DataGridValueType.String => new DataGridStringValue(
                    valueDto.StringValue ?? throw new Exception("'StringValue' field expected for type 'String'")
                ),
                DataGridValueType.Numeric => new DataGridNumericValue(
                    valueDto.NumericValue ?? throw new Exception("'NumericValue' field expected for type 'Numeric'")
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

            value.RowIndex = valueDto.RowIndex;
            return value;
        }

        public static DataGridRow ToDataGridRow(this DataGridRowDto rowDto)
        {
            return new DataGridRow(rowDto.Items.Select(ToDataGridValue));
        }

        public static IEnumerable<DataGridRow> ToDataGridRow(this IEnumerable<DataGridRowDto> rowDtoEnumerable)
        {
            return rowDtoEnumerable.Select(ToDataGridRow);
        }

        public static IEnumerable<DataGridValue> ToDataGridValues(this IEnumerable<DataGridValueDto> valueDtos)
        {
            return valueDtos.Select(ToDataGridValue);
        }
    }

}