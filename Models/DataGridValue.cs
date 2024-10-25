namespace CentaureaTest.Models
{

    public abstract class DataGridValue
    {
        public object Value { get; private set; }

        protected DataGridValue(object value)
        {
            Value = value;
        }
    }

    public class DataGridNumber : DataGridValue
    {
        public DataGridNumber(Decimal value) : base(value) {}
    }

    public class DataGridString : DataGridValue
    {
        public DataGridString(string value) : base(value) {}
    }

}