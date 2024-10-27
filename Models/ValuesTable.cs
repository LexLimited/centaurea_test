using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "values")]
    public class ValuesTable
    {
        [Key]
        public int Id { get; set; }
        public int GridId { get; set; }
        public int FieldId { get; set; }
        public int RowId { get; set; }
    }

    public sealed class NumericValuesTable : ValuesTable
    {
        public Decimal NumericValue { get; set; }
    }

    public sealed class StringValuesTable : ValuesTable
    {
        public string StringValue { get; set; }
    }

    public sealed class RegexValuesTable : ValuesTable
    {
        public string RegexValue { get; set; }
    }

}