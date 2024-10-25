using System.ComponentModel.DataAnnotations;

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

    public class DataGridNumeric : DataGridValue
    {
        public DataGridNumeric(Decimal value) : base(value) {}
    }

    public class DataGridString : DataGridValue
    {
        public DataGridString(string value) : base(value) {}
    }

    public class DataGridRegex : DataGridValue
    {
        public DataGridRegex(string value) : base(value) {}
    }

}