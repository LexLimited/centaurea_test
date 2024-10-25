using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "values")]
    public sealed class ValuesTable
    {
        [Key]
        public int Id { get; set; }
        public int GridId { get; set; }
        public int FieldId { get; set; }
        public int RowId { get; set; }
        public Decimal? NumericValue { get; set; }
        public string? TextValue { get; set; }
        public string[]? ArrayValue { get; set; }
    }

}