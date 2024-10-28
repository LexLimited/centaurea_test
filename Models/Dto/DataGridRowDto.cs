using System.Text;

namespace CentaureaTest.Models.Dto
{

    public sealed class DataGridRowDto
    {
        public List<DataGridValueDto> Items { get; set; }

        public DataGridRowDto()
        {
            Items = new List<DataGridValueDto>();
        }

        public DataGridRowDto(IEnumerable<DataGridValueDto> items)
        {
            Items = items.ToList();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("{{");
            for (int i = 0; i < Items.Count - 1; ++i)
            {
                builder.Append($"{Items[i]}, ");
            }

            builder.Append(Items.Last());
            builder.Append("}}");

            return builder.ToString();
        }

        public DataGridRow ToDataGridRow()
        {
            return new DataGridRow(Items.Select(valueDto => valueDto.ToDataGridValue()));
        }
    }

}