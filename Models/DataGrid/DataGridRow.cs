using System.Text;

namespace CentaureaTest.Models
{

    public sealed class DataGridRow
    {
        public List<DataGridValue> Items { get; set; }

        public DataGridRow(IEnumerable<DataGridValue> items)
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
    }

}