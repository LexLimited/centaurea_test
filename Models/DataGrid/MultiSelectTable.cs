using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "multi_select")]
    public sealed class MultiSelectTable
    {
        [Key]
        public int Id { get; set; }
        public int TableId { get; set; }
        public int FieldId { get; set; }
        public string Option { get; set; }

        public MultiSelectTable(int fieldId, string option)
        {
            FieldId = fieldId;
            Option = option;
        }
    }

}