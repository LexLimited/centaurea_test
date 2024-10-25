namespace CentaureaTest.Models
{

    public struct DataGridFieldSignature
    {
        public string Name;
        public DataGridValueType Type;
    }

    public sealed class DataGridSignature
    {
        public List<DataGridFieldSignature> Fields { get; private set; }

        public DataGridSignature()
        {
            Fields = new List<DataGridFieldSignature>();
        }

        public  DataGridSignature(IEnumerable<DataGridFieldSignature> fields)
        {
            Fields = fields.ToList();
        }
    }

}