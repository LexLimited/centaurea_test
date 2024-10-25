namespace CentaureaTest.Models
{

    public sealed class DataGridRow
    {
        public List<DataGridValue> Items { get; private set; }

        public DataGridRow(IEnumerable<DataGridValue> items)
        {
            Items = items.ToList();
        }
    }

}